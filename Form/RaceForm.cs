using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using NitroRivals.Drivers;
using NitroRivals.Hurdles;
using NitroRivals.Models;
using NitroRivals.Repositories;
using NitroRivals.Services;

namespace NitroRivals.Forms;

/// <summary>The race screen: top-down GDI+ view, timer-driven game loop.</summary>
public class RaceForm : Form
{
    private static readonly Font HudTitle = new("Segoe UI", 12f, FontStyle.Bold);
    private static readonly Font HudText = new("Segoe UI", 11f);
    private static readonly Font HudSmall = new("Segoe UI", 9f);
    private static readonly Font BigText = new("Segoe UI", 64f, FontStyle.Bold);
    private static readonly Color[] RivalColors =
        { Color.DodgerBlue, Color.MediumSeaGreen, Color.MediumOrchid };

    private readonly RaceRequest _request;
    private readonly RaceEngine _engine;
    private readonly int _playerCarId;
    private readonly RaceCanvas _canvas;
    private readonly System.Windows.Forms.Timer _timer;
    private readonly Stopwatch _clock = new();

    private double _lastTime;
    private double _countdown = 3.5;     // seconds before the lights go out
    private bool _saved;

    /// <summary>Result of sp_SaveRaceResult, or null if the race was abandoned.</summary>
    public DataRow? Outcome { get; private set; }

    /// <summary>Builds and shows the race. Returns the saved result, or null.</summary>
    public static DataRow? Launch(IWin32Window owner, RaceRequest request)
    {
        RaceEngine engine;
        int carId;
        try
        {
            engine = RaceSetup.Build(request, out carId);
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not start the race: " + ex.Message);
            return null;
        }

        using var form = new RaceForm(request, engine, carId);
        form.ShowDialog(owner);
        return form.Outcome;
    }

    private RaceForm(RaceRequest request, RaceEngine engine, int playerCarId)
    {
        _request = request;
        _engine = engine;
        _playerCarId = playerCarId;

        FormUi.Setup(this, "Nitro Rivals - " + request.Title, 900, 700);
        KeyPreview = true;

        _canvas = new RaceCanvas { Dock = DockStyle.Fill };
        _canvas.Paint += (s, e) => Draw(e.Graphics, _canvas.ClientSize);
        Controls.Add(_canvas);

        _timer = new System.Windows.Forms.Timer { Interval = 16 };
        _timer.Tick += OnTick;

        Shown += (s, e) => { _clock.Start(); _timer.Start(); };
        FormClosing += (s, e) => _timer.Stop();
        KeyUp += (s, e) =>
        {
            if (e.KeyCode == Keys.Space) _engine.Player.NitroPressed = false;
        };
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Left:   _engine.Player.MoveLane(-1); return true;
            case Keys.Right:  _engine.Player.MoveLane(1);  return true;
            case Keys.Space:  _engine.Player.NitroPressed = true; return true;
            case Keys.Escape: Close(); return true;
        }
        return base.ProcessCmdKey(ref msg, keyData);
    }

    // ------------------------------------------------------------ game loop
    private void OnTick(object? sender, EventArgs e)
    {
        double now = _clock.Elapsed.TotalSeconds;
        double dt = Math.Min(now - _lastTime, 0.05);
        _lastTime = now;

        if (_countdown > 0)
            _countdown -= dt;
        else if (!_engine.IsFinished)
            _engine.Tick(dt);

        _canvas.Invalidate();

        if (_engine.IsFinished && !_saved)
            FinishRace();
    }

    private void FinishRace()
    {
        _saved = true;
        _timer.Stop();

        int position = _request.Mode == RaceModes.TimeTrial ? 1 : _engine.GetPosition(_engine.Player);
        double time = _engine.Player.FinishTime;

        try
        {
            Outcome = new RaceRepository().SaveRaceResult(
                _request, Session.Id, _playerCarId, position, time);
            Session.Coins = (int)Outcome["NewCoins"];

            string text = _request.Mode == RaceModes.TimeTrial
                ? $"Time: {time:F2} s"
                : $"You finished {Ordinal(position)} in {time:F2} s";

            text += $"\n\nCoins earned: +{Outcome["CoinsEarned"]}";
            text += _request.Mode == RaceModes.Cup
                ? $"\nCup points: +{Outcome["PointsEarned"]}"
                : $"\nRank points: +{Outcome["PointsEarned"]}  ({Outcome["RankTier"]})";
            if ((bool)Outcome["UnlockedNextLevel"])
                text += "\n\nNext level unlocked!";

            FormUi.Info(text);
        }
        catch (Exception ex)
        {
            FormUi.Error("Your result could not be saved: " + ex.Message);
        }
        Close();
    }

    private static string Ordinal(int n) => n switch { 1 => "1st", 2 => "2nd", 3 => "3rd", _ => n + "th" };

    // ------------------------------------------------------------ drawing
    private void Draw(Graphics g, Size size)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Color.FromArgb(30, 70, 40));            // grass

        Track track = _engine.Context.Track;
        PlayerDriver player = _engine.Player;

        const int hudWidth = 230;
        int roadWidth = Math.Min(size.Width - hudWidth - 40, 360);
        int roadX = (size.Width - hudWidth - roadWidth) / 2;
        int laneWidth = roadWidth / Track.LaneCount;
        roadWidth = laneWidth * Track.LaneCount;

        double scale = size.Height / 220.0;              // pixels per metre
        double camera = player.Distance - 30;            // player sits near the bottom
        float Y(double distance) => (float)(size.Height - (distance - camera) * scale);

        // road
        using (var road = new SolidBrush(Color.FromArgb(55, 55, 62)))
            g.FillRectangle(road, roadX, 0, roadWidth, size.Height);

        // corner zones
        using (var cornerBrush = new SolidBrush(Color.FromArgb(70, 255, 140, 0)))
            foreach (double start in track.CornerPositions)
            {
                float top = Y(start + Track.CornerLength);
                float bottom = Y(start);
                if (bottom < 0 || top > size.Height) continue;
                g.FillRectangle(cornerBrush, roadX, top, roadWidth, bottom - top);
                g.DrawString("CORNER", HudSmall, Brushes.Orange, roadX + 6, top + 4);
            }

        // lane lines and edges
        using (var dash = new Pen(Color.FromArgb(120, 255, 255, 255), 2) { DashStyle = DashStyle.Dash })
            for (int i = 1; i < Track.LaneCount; i++)
                g.DrawLine(dash, roadX + i * laneWidth, 0, roadX + i * laneWidth, size.Height);
        g.DrawLine(Pens.White, roadX, 0, roadX, size.Height);
        g.DrawLine(Pens.White, roadX + roadWidth, 0, roadX + roadWidth, size.Height);

        // start and finish lines
        DrawLine(g, Y(0), roadX, roadWidth, Color.White, size.Height);
        DrawLine(g, Y(track.LengthMeters), roadX, roadWidth, Color.Gold, size.Height);

        // hurdles
        foreach (Hurdle h in _engine.Context.Hurdles)
        {
            float y = Y(h.Position + Hurdle.Length / 2);
            if (y < -40 || y > size.Height + 40) continue;
            float cx = roadX + h.Lane * laneWidth + laneWidth / 2f;
            DrawHurdle(g, h, cx, y);
        }

        // cars (furthest ahead drawn last)
        foreach (IDriver d in _engine.Drivers.OrderBy(d => d.Distance))
        {
            float y = Y(d.Distance);
            if (y < -60 || y > size.Height + 60) continue;
            float cx = roadX + d.Lane * laneWidth + laneWidth / 2f;

            Color color = d == player
                ? FormUi.Accent
                : RivalColors[Math.Max(0, _engine.Drivers.ToList().IndexOf(d) - 1) % RivalColors.Length];
            DrawCar(g, cx, y, color, d.IsNitroActive);
        }

        DrawHud(g, size, hudWidth);

        if (_countdown > 0)
        {
            string text = _countdown > 2.5 ? "3" : _countdown > 1.5 ? "2" : _countdown > 0.5 ? "1" : "GO!";
            SizeF s = g.MeasureString(text, BigText);
            g.DrawString(text, BigText, Brushes.White,
                roadX + (roadWidth - s.Width) / 2, size.Height / 2f - s.Height / 2);
        }
    }

    private static void DrawLine(Graphics g, float y, int roadX, int roadWidth, Color color, int height)
    {
        if (y < -10 || y > height + 10) return;
        using var brush = new SolidBrush(color);
        g.FillRectangle(brush, roadX, y - 3, roadWidth, 6);
    }

    private static void DrawHurdle(Graphics g, Hurdle hurdle, float cx, float y)
    {
        switch (hurdle)
        {
            case TyreHurdle:
                g.FillEllipse(Brushes.Black, cx - 22, y - 11, 44, 22);
                g.FillEllipse(Brushes.White, cx - 12, y - 6, 24, 12);
                g.FillEllipse(Brushes.Black, cx - 6, y - 3, 12, 6);
                break;
            case OilPatch:
                g.FillEllipse(Brushes.Black, cx - 28, y - 16, 56, 32);
                g.FillEllipse(Brushes.MidnightBlue, cx - 18, y - 9, 24, 14);
                break;
            case StoppedCarHurdle:
                g.FillRectangle(Brushes.DimGray, cx - 14, y - 24, 28, 48);
                g.FillRectangle(Brushes.LightGray, cx - 9, y - 12, 18, 10);
                g.FillRectangle(Brushes.Red, cx - 12, y + 18, 8, 4);
                g.FillRectangle(Brushes.Red, cx + 4, y + 18, 8, 4);
                break;
            default:
                g.FillRectangle(Brushes.Red, cx - 15, y - 8, 30, 16);
                break;
        }
    }

    private static void DrawCar(Graphics g, float cx, float y, Color color, bool nitro)
    {
        if (nitro)
        {
            PointF[] flame = { new(cx - 8, y + 24), new(cx + 8, y + 24), new(cx, y + 48) };
            g.FillPolygon(Brushes.OrangeRed, flame);
        }
        using var body = new SolidBrush(color);
        g.FillRectangle(body, cx - 13, y - 24, 26, 48);
        g.FillRectangle(Brushes.Black, cx - 9, y - 12, 18, 10);       // windscreen
        g.FillRectangle(Brushes.Black, cx - 16, y - 18, 4, 12);       // wheels
        g.FillRectangle(Brushes.Black, cx + 12, y - 18, 4, 12);
        g.FillRectangle(Brushes.Black, cx - 16, y + 8, 4, 12);
        g.FillRectangle(Brushes.Black, cx + 12, y + 8, 4, 12);
    }

    private void DrawHud(Graphics g, Size size, int hudWidth)
    {
        PlayerDriver p = _engine.Player;
        Track track = _engine.Context.Track;
        int x = size.Width - hudWidth;

        using (var panel = new SolidBrush(Color.FromArgb(230, 24, 24, 32)))
            g.FillRectangle(panel, x, 0, hudWidth, size.Height);

        int left = x + 16;
        int y = 16;
        g.DrawString(_request.Title, HudTitle, Brushes.White, new RectangleF(left, y, hudWidth - 24, 50));
        y += 60;

        g.DrawString($"Speed  {p.Speed * 3.6:F0} km/h", HudText, Brushes.White, left, y); y += 30;
        if (_request.Mode != RaceModes.TimeTrial)
        {
            g.DrawString($"Position  {_engine.GetPosition(p)} / {_engine.Drivers.Count}", HudText, Brushes.White, left, y);
            y += 30;
        }
        g.DrawString($"Time  {_engine.Elapsed:F1} s", HudText, Brushes.White, left, y); y += 30;
        g.DrawString($"Hurdles hit  {p.HurdlesHit}", HudText, Brushes.White, left, y); y += 40;

        // nitro bar
        g.DrawString("Nitro", HudSmall, Brushes.Silver, left, y); y += 20;
        float nitro = (float)(p.NitroRemaining / Math.Max(0.01, p.Vehicle.NitroSeconds));
        g.FillRectangle(Brushes.DimGray, left, y, hudWidth - 32, 16);
        using (var fill = new SolidBrush(p.IsNitroActive ? Color.OrangeRed : Color.DeepSkyBlue))
            g.FillRectangle(fill, left, y, (hudWidth - 32) * Math.Clamp(nitro, 0f, 1f), 16);
        y += 36;

        // progress bar
        g.DrawString("Progress", HudSmall, Brushes.Silver, left, y); y += 20;
        float progress = (float)(p.Distance / track.LengthMeters);
        g.FillRectangle(Brushes.DimGray, left, y, hudWidth - 32, 16);
        g.FillRectangle(Brushes.MediumSeaGreen, left, y, (hudWidth - 32) * Math.Clamp(progress, 0f, 1f), 16);
        y += 40;

        g.DrawString(p.Vehicle.ToString(), HudSmall, Brushes.Silver, new RectangleF(left, y, hudWidth - 24, 40));

        g.DrawString("Left / Right  change lane\nSpace  nitro\nEsc  quit race",
            HudSmall, Brushes.Silver, left, size.Height - 70);
    }

    /// <summary>Panel with double buffering so the race does not flicker.</summary>
    private class RaceCanvas : Panel
    {
        public RaceCanvas()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.FromArgb(30, 70, 40);
        }
    }
}
