using NitroRivals.Exceptions;
using NitroRivals.Services;

namespace NitroRivals.Forms;

public class OtpForm : Form
{
    private readonly int _playerId;
    private readonly string _email;
    private readonly TextBox _txtOtp;
    private readonly AuthService _auth = new();

    public OtpForm(int playerId, string email)
    {
        _playerId = playerId;
        _email = email;

        FormUi.Setup(this, "Nitro Rivals - Verify Email", 380, 310);

        FormUi.AddLabel(this, "VERIFY EMAIL", 50, 20, 18f, true);
        FormUi.AddLabel(this, $"Enter the 6-digit code sent to\n{email}", 50, 62, 9.5f, muted: true);

        _txtOtp = FormUi.AddTextBox(this, 50, 120, 280);
        _txtOtp.MaxLength = 6;
        _txtOtp.Font = new Font("Segoe UI", 16f);
        _txtOtp.TextAlign = HorizontalAlignment.Center;

        var btnVerify = FormUi.AddButton(this, "VERIFY", 50, 175, 280);
        var btnResend = FormUi.AddButton(this, "Resend code", 50, 225, 135, primary: false);
        var btnLater = FormUi.AddButton(this, "Later", 195, 225, 135, primary: false);

        AcceptButton = btnVerify;
        btnVerify.Click += OnVerify;
        btnResend.Click += OnResend;
        btnLater.Click += (s, e) => Close();
    }

    private void OnVerify(object? sender, EventArgs e)
    {
        try
        {
            _auth.VerifyOtp(_playerId, _email, _txtOtp.Text.Trim());
            FormUi.Info("Account verified!\nYou received 500 coins and a starter car.\nYou can log in now.");
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (OtpExpiredException ex) { FormUi.Error(ex.Message); }
        catch (InvalidOtpException ex) { FormUi.Error(ex.Message); }
        catch (ArgumentException ex)   { FormUi.Error(ex.Message); }
        catch (Exception ex)           { FormUi.Error("Verification failed: " + ex.Message); }
    }

    private void OnResend(object? sender, EventArgs e)
    {
        try
        {
            _auth.ResendOtp(_playerId, _email);
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not resend the code: " + ex.Message);
        }
    }
}
