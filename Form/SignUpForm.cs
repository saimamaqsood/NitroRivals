using NitroRivals.Exceptions;
using NitroRivals.Services;

namespace NitroRivals.Forms;

public class SignUpForm : Form
{
    private readonly TextBox _txtUser;
    private readonly TextBox _txtEmail;
    private readonly TextBox _txtPass;
    private readonly TextBox _txtConfirm;
    private readonly Button _btnSignUp;
    private readonly AuthService _auth = new();

    public SignUpForm()
    {
        FormUi.Setup(this, "Nitro Rivals - Sign Up", 380, 520);

        FormUi.AddLabel(this, "CREATE ACCOUNT", 50, 20, 18f, true);
        FormUi.AddLabel(this, "Internet is needed only for this step.", 50, 58, 9f, muted: true);

        FormUi.AddLabel(this, "Username", 50, 95);
        _txtUser = FormUi.AddTextBox(this, 50, 120);

        FormUi.AddLabel(this, "Email", 50, 160);
        _txtEmail = FormUi.AddTextBox(this, 50, 185);

        FormUi.AddLabel(this, "Password (min 6 characters)", 50, 225);
        _txtPass = FormUi.AddTextBox(this, 50, 250, password: true);

        FormUi.AddLabel(this, "Confirm password", 50, 290);
        _txtConfirm = FormUi.AddTextBox(this, 50, 315, password: true);

        _btnSignUp = FormUi.AddButton(this, "SIGN UP", 50, 375, 280);
        var btnBack = FormUi.AddButton(this, "Back to login", 50, 425, 280, primary: false);

        AcceptButton = _btnSignUp;
        _btnSignUp.Click += OnSignUp;
        btnBack.Click += (s, e) => Close();
    }

    private void OnSignUp(object? sender, EventArgs e)
    {
        try
        {
            _btnSignUp.Enabled = false;
            Cursor = Cursors.WaitCursor;

            int playerId = _auth.Register(_txtUser.Text, _txtEmail.Text, _txtPass.Text, _txtConfirm.Text);

            using var otpForm = new OtpForm(playerId, _txtEmail.Text.Trim());
            otpForm.ShowDialog(this);
            Close();   // back to LoginForm either way
        }
        catch (ArgumentException ex)            { FormUi.Error(ex.Message); }
        catch (DuplicateAccountException ex)    { FormUi.Error(ex.Message); }
        catch (Exception ex)                    { FormUi.Error("Sign up failed: " + ex.Message); }
        finally
        {
            Cursor = Cursors.Default;
            if (!IsDisposed) _btnSignUp.Enabled = true;
        }
    }
}
