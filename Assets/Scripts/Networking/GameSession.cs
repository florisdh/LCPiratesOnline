using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSession : MonoBehaviour
{
    // SingleTon
    public static GameSession CURRENT;
    
    #region Vars

    public ClientToServerConnection ServerConnection;
	public ClientToClient ClientConnection;

	private GameRoom _currentRoom;
	private bool _loadingGame = false;
	private bool _loadRequest = false;
	private bool _inGame = false;

	private ClientManager _clientManager;

    #endregion

    #region Methods

    private void Awake()
    {
        CURRENT = this;
		DontDestroyOnLoad(this);

		BindUPnP();

		ClientConnection = new ClientToClient();
		_clientManager = GetComponent<ClientManager>();
		ServerConnection = new ClientToServerConnection(ClientConnection);
		ServerConnection.GameLoad += LoadGame;
		ServerConnection.GameStart += StartGame;
    }

    private void OnApplicationQuit()
    {
        ServerConnection.Disconnect();
		ClientConnection.Dispose();
    }

	private void FixedUpdate()
	{
		if (_loadRequest)
		{
			_loadRequest = false;
			_loadingGame = true;
			Application.LoadLevel(_currentRoom.MapID);
		}
		else if (_loadingGame && LevelSettings.Current != null)
		{
			_loadingGame = false;

			// Spawn players
			_clientManager.ConnectedPlayers = _currentRoom.Players.ToArray();
			_clientManager.PlayerID = ServerConnection.PlayerID;
			_clientManager.SpawnPlayers();

			// Send loaded to server
			ServerConnection.LoadedGame();
		}
	}

	private void BindUPnP()
	{
		Debug.Log("res: " + UPnPHelper.BIND("192.168.0.10", 1337, 22000));
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

		_clientManager.StartPlayer();
	}

    #endregion
}