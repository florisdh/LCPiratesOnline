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
		_clientManager = GetComponent<ClientManager>();
		ClientConnection = new ClientToClient();
		ServerConnection = new ClientToServerConnection(ClientConnection);
    }

    private void OnApplicationQuit()
    {
        ServerConnection.Disconnect();
    }

	private void FixedUpdate()
	{
		if (_loadRequest)
		{
			_loadRequest = false;
			_loadingGame = true;
			Application.LoadLevel(_currentRoom.MapID);
			Debug.Log("Loading");
		}
		else if (_loadingGame)
		{
			_loadingGame = false;

			_clientManager.ConnectedPlayers = _currentRoom.Players.ToArray();
			_clientManager.PlayerID = ServerConnection.PlayerID;
			_clientManager.SpawnPlayers();

			Debug.Log("Loaded");
		}
	}

	public void LoadGame(GameRoom room)
	{
		if (_loadingGame || _loadRequest || _inGame) return;
		_loadRequest = true;
		_loadingGame = false;
		_inGame = false;
		_currentRoom = room;
	}

    #endregion
}