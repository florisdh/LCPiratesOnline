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

	private float _reconnectInterval = 10f;
	private float _reconnectTimer = 10f;
	private int _reconnectCountdown = -1;

    #endregion

    #region Methods

    private void OnEnable()
    {
		UPnPHelper.RESULT += UPnPHelper_RESULT;
		if (GameSession.CURRENT.UPnPLoaded)
		{
			_result = LoginResult.UPnPLoaded;
		}
		else
		{
			_errorOutput.ShowError("Loading UPnP connection..");
		}

        _session = GameSession.CURRENT;
		_session.ServerConnection.Connected += ServerConnection_Connected;
		_session.ServerConnection.ConnectFailed += ServerConnection_ConnectFailed;
		_session.ServerConnection.ConnectionSecured += ServerConnection_ConnectionSecured;
        _session.ServerConnection.LoginFailed += ServerConnection_LoginFailed;
        _session.ServerConnection.LogedIn += ServerConnection_LogedIn;

    }

	private void UPnPHelper_RESULT(UPnPHelperResult res)
	{
		if (res.Type != UPnPHelperResultType.Succeed)
			_result = LoginResult.UPnPFailed;
		else
			_result = LoginResult.UPnPLoaded;
	}

    private void OnDisable()
    {
        if (_session != null)
        {
			_session.ServerConnection.Connected -= ServerConnection_Connected;
			_session.ServerConnection.ConnectFailed -= ServerConnection_ConnectFailed;
			_session.ServerConnection.ConnectionSecured -= ServerConnection_ConnectionSecured;
            _session.ServerConnection.LoginFailed -= ServerConnection_LoginFailed;
            _session.ServerConnection.LogedIn -= ServerConnection_LogedIn;

			UPnPHelper.RESULT -= UPnPHelper_RESULT;
        }
    }

    private void FixedUpdate()
    {
		// Unity cannot handle access from other thread
		// Easey fix by putting output in update
		// SO that it will be called on main unity thread

		if (_result == LoginResult.UPnPFailed)
		{
			_result = LoginResult.None;
			_errorOutput.ShowError("Failed to load UPnP, this must be enabled on the router to play.", 0.5f, 10f);
		}
		else if (_result == LoginResult.UPnPLoaded)
		{
			_result = LoginResult.None;
			BeginConnect();
		}
		else if (_result == LoginResult.ConnectionFailed)
		{
			_result = LoginResult.None;
			_errorOutput.ShowError("Failed to connect.");
			_reconnectTimer = -1f;
		}
		else if (_result == LoginResult.Connected)
		{
			_result = LoginResult.None;
			_errorOutput.ShowError("Securing connection...");
		}
		else if (_result == LoginResult.ConnectionSecured)
		{
			_result = LoginResult.None;
			_errorOutput.ShowError("Connection established.");
		}
		else if (_result == LoginResult.FailedToLogin)
		{
			_result = LoginResult.None;
			_errorOutput.ShowError("Invalid credentials.");
		}
		else if (_result == LoginResult.LogedIn)
		{
			_result = LoginResult.None;
			_errorOutput.ShowError("Welcome " + _session.ServerConnection.UserName);
			_mainMenu.Show(this);
		}

		if (!_session.ServerConnection.IsConnected && _reconnectTimer < _reconnectInterval)
		{
			_reconnectTimer += Time.deltaTime;
			int currentCountDown = (int)Mathf.Floor(_reconnectInterval - _reconnectTimer);

			if (_reconnectTimer >= _reconnectInterval)
			{
				BeginConnect();
			}
			else if (currentCountDown != _reconnectCountdown && currentCountDown < _reconnectInterval)
			{
				_reconnectCountdown = currentCountDown;
				_errorOutput.ShowError("Connecting in " + _reconnectCountdown.ToString(), 0.4f, 0.4f);
			}
		}
    }

	private void BeginConnect()
	{
		_session.ServerConnection.Connect();
		_errorOutput.ShowError("Connecting...");
	}

    public void Login()
    {
		if (!_session || _session.ServerConnection.IsLoggingIn || _session.ServerConnection.IsLogedIn) return;
        
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

	private void ServerConnection_ConnectionSecured(object sender, System.EventArgs e)
	{
		_result = LoginResult.ConnectionSecured;
	}

	private void ServerConnection_ConnectFailed(object sender, System.EventArgs e)
	{
		_result = LoginResult.ConnectionFailed;
	}

	private void ServerConnection_Connected(object sender, System.EventArgs e)
	{
		_result = LoginResult.Connected;
	}

    private void ServerConnection_LogedIn(object sender, System.EventArgs e)
    {
        _result = LoginResult.LogedIn;
    }

    private void ServerConnection_LoginFailed(object sender, System.EventArgs e)
    {
        _result = LoginResult.FailedToLogin;
    }

    #endregion
}

enum LoginResult
{
	UPnPFailed,
	UPnPLoaded,
	Connected,
	ConnectionSecured,
	ConnectionFailed,
    LogedIn,
    FailedToLogin,
    None
}