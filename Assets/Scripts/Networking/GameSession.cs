using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Net;

public class GameSession : MonoBehaviour
{
    // SingleTon
    public static GameSession CURRENT;
    
    #region Vars

    public ClientToServerConnection ServerConnection;
	public ClientToClient ClientConnection;

	private GameRoom _currentRoom;
	private bool _UPnPLoaded = false;
	private bool _UPnPLoading = false;
	private bool _loadingGame = false;
	private bool _loadRequest = false;
	private bool _inGame = false;

	private string _localIP;
	private int _localPort;
	private int _remotePort = 20010;
	private int _upnpCounter = 0;

	private ClientManager _clientManager;

    #endregion

    #region Methods

    private void Awake()
    {
		if (Application.isEditor)
			_remotePort = 20001;

        CURRENT = this;
		DontDestroyOnLoad(this);

		_localIP = GetIPAddress();

		UPnPHelper.RESULT += BindUPnPResult;

		ClientConnection = new ClientToClient();
		_clientManager = GetComponent<ClientManager>();
		ServerConnection = new ClientToServerConnection(ClientConnection);
		ServerConnection.GameLoad += LoadGame;
		ServerConnection.GameStart += StartGame;

		BindUPnP();
    }

    private void OnApplicationQuit()
    {
        ServerConnection.Disconnect();
		ClientConnection.Dispose();

		UPnPHelper.RESULT -= BindUPnPResult;
		if (_UPnPLoaded)
			UPnPHelper.UNBIND(_remotePort);
    }

	private void FixedUpdate()
	{
		// Load map
		if (_loadRequest)
		{
			_loadRequest = false;
			_loadingGame = true;
			Application.LoadLevel(_currentRoom.MapID);
		}
		// Load Players
		else if (_loadingGame && LevelSettings.Current != null)
		{
			_loadingGame = false;
			
			// Spawn players
			_clientManager.ConnectedPlayers = _currentRoom.Players.ToArray();
			_clientManager.PlayerID = ServerConnection.PlayerID;
			_clientManager.SpawnPlayers();

			Debug.Log("Send loaded to server");
			ServerConnection.LoadedGame();
		}
	}

	public void BindUPnP()
	{
		Debug.Log("Loading upnp..");
		UPnPHelper.BIND(_localIP, _remotePort, _remotePort + 20);
	}

	private void BindUPnPResult(UPnPHelperResult res)
	{
		if (res.Type == UPnPHelperResultType.Succeed)
		{
			_UPnPLoaded = true;
			_remotePort = _localPort = res.ExternPort;
			ClientConnection.Bind(new IPEndPoint(IPAddress.Any, _localPort));
			Debug.Log("UPnP loaded on " + _remotePort.ToString());
		}
		else if (res.Type == UPnPHelperResultType.VersionMismatch)
		{
			Debug.Log("UPnPHelper Invalid version!");
		}
		else
		{
			_upnpCounter++;
			Debug.Log("UPnP failed to load. Try " + _upnpCounter.ToString());
			if (_upnpCounter >= 3)
			{
				Debug.Log("Failed to load UPnP after 3 tries.");
			}
			else
				BindUPnP();
		}
	}

	private void LoadGame(object sender, GameRoom room)
	{
		if (_loadingGame || _loadRequest || _inGame) return;
		_loadRequest = true;
		_loadingGame = false;
		_inGame = false;
		_currentRoom = room;
	}

	private void StartGame(object sender, GameRoom room)
	{
		_loadingGame = _loadRequest = false;
		_inGame = true;

		ThreadHelper.MAIN.InvokeOnThread(startGame);
	}

	private void startGame()
	{
		_clientManager.StartPlayer();
	}

	private string GetIPAddress()
	{
		IPHostEntry host;
		string localIP = string.Empty;
		host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (IPAddress ip in host.AddressList)
		{
			if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
			{
				localIP = ip.ToString();
			}
		}
		return localIP;
	}

    #endregion

	#region Properties

	public bool UPnPLoaded
	{
		get { return _UPnPLoaded; }
	}

	#endregion
}