using System.Drawing;
using System.Windows.Forms;

namespace NitroRivals.Forms;

/// <summary>Helpers so every screen has the same look.</summary>
public static class FormUi
{
    public static readonly Color Bg = Color.FromArgb(24, 24, 32);
    public static readonly Color Field = Color.FromArgb(40, 40, 54);
    public static readonly Color Accent = Color.FromArgb(255, 106, 0);
    public static readonly Color Muted = Color.FromArgb(160, 160, 175);

    public static void Setup(Form f, string title, int width, int height)
    {
        f.Text = title;
        f.ClientSize = new Size(width, height);
        f.StartPosition = FormStartPosition.CenterScreen;
        f.FormBorderStyle = FormBorderStyle.FixedDialog;
        f.MaximizeBox = false;
        f.BackColor = Bg;
        f.ForeColor = Color.White;
        f.Font = new Font("Segoe UI", 10f);
    }

    public static Label AddLabel(Control parent, string text, int x, int y,
        float size = 10f, bool bold = false, bool muted = false)
    {
        var l = new Label
        {
            Text = text, AutoSize = true, Location = new Point(x, y),
            Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
            ForeColor = muted ? Muted : Color.White
        };
        parent.Controls.Add(l);
        return l;
    }

    public static TextBox AddTextBox(Control parent, int x, int y, int width = 280, bool password = false)
    {
        var t = new TextBox
        {
            Location = new Point(x, y), Width = width,
            BackColor = Field, ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            UseSystemPasswordChar = password
        };
        parent.Controls.Add(t);
        return t;
    }

    public static NumericUpDown AddNumeric(Control parent, int x, int y, int width,
        int min, int max, int value)
    {
        var n = new NumericUpDown
        {
            Location = new Point(x, y), Width = width,
            Minimum = min, Maximum = max, Value = value,
            BackColor = Field, ForeColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        parent.Controls.Add(n);
        return n;
    }

    public static ComboBox AddCombo(Control parent, int x, int y, int width)
    {
        var c = new ComboBox
        {
            Location = new Point(x, y), Width = width,
            DropDownStyle = ComboBoxStyle.DropDownList,
            BackColor = Field, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        parent.Controls.Add(c);
        return c;
    }

    public static Button AddButton(Control parent, string text, int x, int y, int width,
        int height = 38, bool primary = true)
    {
        var b = new Button
        {
            Text = text, Location = new Point(x, y), Size = new Size(width, height),
            FlatStyle = FlatStyle.Flat,
            BackColor = primary ? Accent : Field,
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };
        b.FlatAppearance.BorderSize = 0;
        parent.Controls.Add(b);
        return b;
    }

    public static LinkLabel AddLink(Control parent, string text, int x, int y)
    {
        var l = new LinkLabel
        {
            Text = text, AutoSize = true, Location = new Point(x, y),
            LinkColor = Accent, ActiveLinkColor = Color.White
        };
        parent.Controls.Add(l);
        return l;
    }

    public static DataGridView AddGrid(Control parent, int x, int y, int width, int height)
    {
        var g = new DataGridView
        {
            Location = new Point(x, y), Size = new Size(width, height),
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            BackgroundColor = Field,
            BorderStyle = BorderStyle.None,
            GridColor = Bg,
            EnableHeadersVisualStyles = false
        };
        g.ColumnHeadersDefaultCellStyle.BackColor = Accent;
        g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Accent;
        g.DefaultCellStyle.BackColor = Field;
        g.DefaultCellStyle.ForeColor = Color.White;
        g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(90, 90, 120);
        g.DefaultCellStyle.SelectionForeColor = Color.White;
        parent.Controls.Add(g);
        return g;
    }

    public static void HideColumns(DataGridView grid, params string[] names)
    {
        foreach (string name in names)
            if (grid.Columns.Contains(name))
                grid.Columns[name]!.Visible = false;
    }

    public static void Header(DataGridView grid, string column, string text)
    {
        if (grid.Columns.Contains(column))
            grid.Columns[column]!.HeaderText = text;
    }

    public static System.Data.DataRow? SelectedRow(DataGridView grid) =>
        (grid.CurrentRow?.DataBoundItem as System.Data.DataRowView)?.Row;

    public static void Error(string message) =>
        MessageBox.Show(message, "Nitro Rivals", MessageBoxButtons.OK, MessageBoxIcon.Error);

    public static void Info(string message) =>
        MessageBox.Show(message, "Nitro Rivals", MessageBoxButtons.OK, MessageBoxIcon.Information);
}
