using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginMenu : Menu
{
    #region Vars

    [SerializeField]
    private Menu _mainMenu;

    [SerializeField]
    private InputField _usrnInput;
    [SerializeField]
    private InputField _pwInput;
    [SerializeField]
    private ErrorPanel _errorOutput;

    private GameSession _session;

    private LoginResult _result = LoginResult.None;

    #endregion

    #region Methods

    private void OnEnable()
    {
        _session = GameSession.CURRENT;

        // Instant proceed if already logedIn
        if (_session.ServerConnection.IsLogedIn)
        {
            Debug.Log("Already logedin");
            _session = null;
            _mainMenu.Show(this);
            return;
        }

        _session.ServerConnection.LoginFailed += ServerConnection_LoginFailed;
        _session.ServerConnection.LogedIn += ServerConnection_LogedIn;
    }

    private void OnDisable()
    {
        if (_session != null)
        {
            _session.ServerConnection.LoginFailed -= ServerConnection_LoginFailed;
            _session.ServerConnection.LogedIn -= ServerConnection_LogedIn;
        }
    }

    private void FixedUpdate()
    {
        if (_result == LoginResult.Succeed)
        {
            _mainMenu.Show(this);
            _result = LoginResult.None;
        }
        else if (_result == LoginResult.InvalidCredentials)
        {
            _errorOutput.ShowError("Invalid credentials.");
            _result = LoginResult.None;
        }
    }

    public void Login()
    {
        if (_session.ServerConnection.IsLoggingIn || _session.ServerConnection.IsLogedIn) return;
        
        string usr = _usrnInput.text;
        string pw = _pwInput.text;

        //
        if (usr == string.Empty || pw == string.Empty)
        {
            return;
        }

        //
        _session.ServerConnection.Login(usr, pw);
    }

    private void ServerConnection_LogedIn(object sender, System.EventArgs e)
    {
        _result = LoginResult.Succeed;
    }

    private void ServerConnection_LoginFailed(object sender, System.EventArgs e)
    {
        _result = LoginResult.InvalidCredentials;
    }

    #endregion
}

enum LoginResult
{
    Succeed,
    InvalidCredentials,
    None
}