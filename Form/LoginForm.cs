using NitroRivals.Services;

namespace NitroRivals.Forms;

public class LoginForm : Form
{
    private readonly TextBox _txtUser;
    private readonly TextBox _txtPass;
    private readonly Button _btnLogin;
    private readonly AuthService _auth = new();

    public LoginForm()
    {
        FormUi.Setup(this, "Nitro Rivals - Login", 380, 380);

        FormUi.AddLabel(this, "NITRO RIVALS", 50, 25, 22f, true);
        FormUi.AddLabel(this, "Race. Upgrade. Win.", 52, 70, 10f, muted: true);

        FormUi.AddLabel(this, "Username", 50, 115);
        _txtUser = FormUi.AddTextBox(this, 50, 140);

        FormUi.AddLabel(this, "Password", 50, 180);
        _txtPass = FormUi.AddTextBox(this, 50, 205, password: true);

        _btnLogin = FormUi.AddButton(this, "LOGIN", 50, 255, 280);
        var link = FormUi.AddLink(this, "New player? Create an account", 50, 310);

        AcceptButton = _btnLogin;
        _btnLogin.Click += OnLogin;
        link.LinkClicked += (s, e) =>
        {
            using var f = new SignUpForm();
            Hide();
            f.ShowDialog();
            Show();
        };
    }

    private void OnLogin(object? sender, EventArgs e)
    {
        if (_txtUser.Text.Trim() == "" || _txtPass.Text == "")
        {
            FormUi.Error("Please enter your username and password.");
            return;
        }

        try
        {
            LoginResult result = _auth.Login(_txtUser.Text, _txtPass.Text,
                out int playerId, out string email);

            switch (result)
            {
                case LoginResult.InvalidCredentials:
                    FormUi.Error("Incorrect username or password.");
                    break;

                case LoginResult.NotVerified:
                    FormUi.Info("Your account is not verified yet. Please enter the code sent to your email.");
                    using (var otp = new OtpForm(playerId, email))
                    {
                        otp.ShowDialog(this);
                    }
                    break;

                case LoginResult.Success:
                    OpenMainMenu();
                    break;
            }
        }
        catch (Exception ex)
        {
            FormUi.Error("Could not log in: " + ex.Message);
        }
    }

    private void OpenMainMenu()
    {
        var menu = new MainMenuForm();
        menu.FormClosed += (s, e) =>
        {
            Session.Clear();
            _txtPass.Clear();
            Show();
        };
        Hide();
        menu.Show();
    }
}
