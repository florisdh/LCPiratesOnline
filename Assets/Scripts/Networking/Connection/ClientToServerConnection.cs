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
    public event EventHandler CreatedRoom;
    public event EventHandler JoinedRoom;
	public event EventHandler ReceivedRooms;

    #endregion

    #region Vars

	public GameRoom ConnectedRoom;
	public GameRoom[] OpenRooms;

    private bool _loggingIn = false;
    private bool _logedIn = false;

    private string _userName;
    private int _playerID;

    #endregion

    #region Construct

    public ClientToServerConnection()
    {
    }

    #endregion

    #region Methods

    public void Login(string usr, string pw)
    {
        if (_logedIn || _loggingIn) return;

        _userName = usr;
        BeginSend(PackageFactory.Pack(PackageType.LoginAttempt, new LoginData(usr, pw)));
    }

    public void CreateRoom(string name, int max, int mapID)
    {
        if (!_logedIn) return;

        BeginSend(PackageFactory.Pack(PackageType.CreateRoom, new CreateGameData(name, max, mapID)));
    }

	public void RequestRooms()
	{
		BeginSend(PackageFactory.Pack(PackageType.RequestRooms, null));
	}

    protected override void HandleMessage(TypedPackage message)
    {
        if (message.Type == PackageType.LoginSucceed)
		{
			int offset = 0;
            LoginSucceedData data = new LoginSucceedData(message.Data, ref offset);
            _playerID = data.PlayerID;
            OnLogedIn();
        }
        else if (message.Type == PackageType.LoginFailed)
        {
            OnLoginFailed();
        }
        else if (message.Type == PackageType.RoomCreated)
        {
            OnCreatedRoom();
        }
        else if (message.Type == PackageType.JoinedRoom)
		{
			JoinedRoomInfoData data;
			int offset = 0;
			try
			{
				data = new JoinedRoomInfoData(message.Data, ref offset);
				ConnectedRoom = data.Room;
				OnJoinedRoom();
			}
			catch (Exception e)
			{
				Debug.Log("Failed to parse data: " + e.Message);
			}
		}
		else if (message.Type == PackageType.RoomList)
		{
			int offset = 0;
			RoomListData data = new RoomListData(message.Data, ref offset);
			OpenRooms = data.Rooms;
			OnReceivedRooms();
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

    private void OnCreatedRoom()
    {
        if (CreatedRoom != null) CreatedRoom(this, null);
    }

    private void OnJoinedRoom()
    {
        if (JoinedRoom != null) JoinedRoom(this, null);
    }

	private void OnReceivedRooms()
	{
		if (ReceivedRooms != null) ReceivedRooms(this, null);
	}
    
    #endregion

    #region Properties

    public bool IsLogedIn
    {
        get { return _logedIn; }
    }

    public bool IsLoggingIn
    {
        get { return _loggingIn; }
    }

    public string UserName
    {
        get { return _userName; }
    }

    public int PlayerID
    {
        get { return _playerID; }
    }

    #endregion
}
