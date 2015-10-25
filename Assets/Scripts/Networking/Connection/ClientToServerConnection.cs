using System;
using UnityEngine;

public class ClientToServerConnection : NetworkClient
{
    #region Defaults

    public const string DEFAULT_IP = "127.0.0.1";
	public const int DEFAULT_PORT = 1337;

    #endregion

    #region Events

    public event EventHandler LogedIn;
    public event EventHandler LoginFailed;
    public event EventHandler CreatedRoom;
    public event EventHandler JoinedRoom;
	public event EventHandler ReceivedRooms;
	public event RoomPlayerEvent OtherJoinedRoom;
	public event RoomPlayerEvent OtherLeftRoom;

	public delegate void RoomPlayerEvent(object sender, RoomPlayerInfo player);

    #endregion

    #region Vars

	public GameRoom ConnectedRoom;
	public GameRoom[] OpenRooms;

    private bool _loggingIn = false;
    private bool _logedIn = false;
	private bool _joiningRoom = false;
	private bool _joinedRoom = false;

    private string _userName;
    private int _playerID = -1;

    #endregion

    #region Construct

    public ClientToServerConnection()
    {
    }

    #endregion

    #region Methods

	public new void Connect(string ip = DEFAULT_IP, int port = DEFAULT_PORT)
	{
		base.Connect(ip, port);
	}

    public void Login(string usr, string pw)
    {
        if (_logedIn || _loggingIn) return;

        _userName = usr;
        BeginSend(PackageFactory.Pack(PackageType.LoginAttempt, new LoginData(usr, pw)));
    }

	public void Logout()
	{
		if (!_logedIn) return;
		_logedIn = false;
		_joinedRoom = _joiningRoom = false;
		_userName = "";
		_playerID = -1;

		BeginSend(PackageFactory.Pack(PackageType.Logout, null));
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

	public void JoinRoom(int roomID)
	{
		if (!_logedIn || _joiningRoom || _joinedRoom) return;
		_joiningRoom = true;

		BeginSend(PackageFactory.Pack(PackageType.RequestJoin, new JoinRoomData(roomID)));
	}

	public void LeaveRoom()
	{
		if (!_joinedRoom) return;
		_joinedRoom = false;
		ConnectedRoom = null;

		BeginSend(PackageFactory.Pack(PackageType.LeaveRoom, null));
	}

    protected override void HandleMessage(TypedPackage message)
    {
		int offset = 0;

		//Debug.Log(string.Format("Msg: {0} of length {1}.", message.Type, message.Data.Length));

        if (message.Type == PackageType.LoginSucceed)
		{
            PlayerIDData data = new PlayerIDData(message.Data, ref offset);
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
			try
			{
				data = new JoinedRoomInfoData(message.Data, ref offset);
				ConnectedRoom = data.Room;
				OnJoinedRoom();
			}
			catch (Exception e)
			{
				Debug.Log("Failed to parse data: " + e.Message + " | " + e.TargetSite + " | " + e.StackTrace);
			}
		}
		else if (message.Type == PackageType.RoomList)
		{
			RoomListData data = new RoomListData(message.Data, ref offset);
			OpenRooms = data.Rooms;
			OnReceivedRooms();
		}
		else if (message.Type == PackageType.OtherJoinedRoom)
		{
			RoomPlayerInfo playerInfo = new RoomPlayerData(message.Data, ref offset).Player;
			ConnectedRoom.Players.Add(playerInfo);
			OnOtherJoinedRoom(playerInfo);
		}
		else if (message.Type == PackageType.OtherLeftRoom)
		{
			PlayerIDData playerInfo = new PlayerIDData(message.Data, ref offset);
			RoomPlayerInfo player = null;
			for (int i = ConnectedRoom.Players.Count - 1; i >= 0; i--)
			{
				if (ConnectedRoom.Players[i].PlayerID == playerInfo.PlayerID)
				{
					player = ConnectedRoom.Players[i];
					ConnectedRoom.Players.RemoveAt(i);
					break;
				}
			}

			if (player != null) OnOtherLeftRoom(player);
			else Debug.Log("Could not find player to remove");
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
		_joiningRoom = false;
		_joinedRoom = true;

        if (JoinedRoom != null) JoinedRoom(this, null);
    }

	private void OnReceivedRooms()
	{
		if (ReceivedRooms != null) ReceivedRooms(this, null);
	}

	private void OnOtherJoinedRoom(RoomPlayerInfo playerInfo)
	{
		if (OtherJoinedRoom != null) OtherJoinedRoom(this, playerInfo);
	}

	private void OnOtherLeftRoom(RoomPlayerInfo playerInfo)
	{
		if (OtherLeftRoom != null) OtherLeftRoom(this, playerInfo);
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

	public bool IsInRoom
	{
		get { return _joinedRoom; }
	}

	public bool JoiningRoom
	{
		get { return _joiningRoom; }
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
