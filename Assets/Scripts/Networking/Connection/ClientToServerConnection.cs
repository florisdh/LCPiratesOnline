using System;
using UnityEngine;

public class ClientToServerConnection : NetworkClient
{
    #region Defaults

    public static string DEFAULT_IP = "127.0.0.1";
    public static int DEFAULT_PORT = 1337;

    #endregion

    #region Events

    public event EventHandler LogedIn;
    public event EventHandler LoginFailed;

    #endregion

    #region Vars

    private bool _loggingIn = false;
    private bool _logedIn = false;

    private string _userName;

    #endregion

    #region Construct

    public ClientToServerConnection()
    {
    }

    #endregion

    #region Methods

    public void Login(string usr, string pw)
    {
        _userName = usr;
        BeginSend(PackageFactory.Pack(PackageType.LoginAttempt, new LoginData(usr, pw)));
    }

    protected override void HandleMessage(TypedPackage message)
    {
        if (message.Type == PackageType.LoginSucceed)
        {
            OnLogedIn();
        }
        else if (message.Type == PackageType.LoginFailed)
        {
            OnLoginFailed();
        }
        else
        {
            base.HandleMessage(message);
        }
    }

    private void OnLogedIn()
    {
        _logedIn = true;
        _loggingIn = false;

        if (LogedIn != null) LogedIn(this, null);
    }

    private void OnLoginFailed()
    {
        _logedIn = _loggingIn = false;

        if (LoginFailed != null) LoginFailed(this, null);
    }

    #endregion
}
