using UnityEngine;
using UnityEngine.UI;

public class JoinRoomMenu : Menu
{
	#region Vars

	[SerializeField]
	private ListView _roomListView;

	[SerializeField]
	private Menu _lobbyMenu;

	private GameSession _session;

	private bool _reload = false;
	private bool _joined = false;

	#endregion

	#region Methods

	private void OnEnable()
	{
		_session = GameSession.CURRENT;
		_joined = false;
		_session.ServerConnection.ReceivedRooms += ServerConnection_ReceivedRooms;
		_session.ServerConnection.JoinedRoom += ServerConnection_JoinedRoom;
		_session.ServerConnection.RequestRooms();
	}

	private void OnDisable()
	{
		if (_session != null)
		{
			_session.ServerConnection.ReceivedRooms -= ServerConnection_ReceivedRooms;
			_session.ServerConnection.JoinedRoom -= ServerConnection_JoinedRoom;
			_session = null;
		}
		_roomListView.Clear();
	}

	private void FixedUpdate()
	{
		if (_joined)
		{
			_lobbyMenu.Show(this);
			return;
		}
		else if (_reload)
		{
			_reload = false;
			foreach (GameRoom info in _session.ServerConnection.OpenRooms)
			{
				ListItem row = _roomListView.AddRow(new string[] { info.Name, info.MapID.ToString(), string.Format("{0}/{1}", info.Players.Count, info.MaxPlayers) });
				row.State = info;
				row.gameObject.GetComponent<Button>().onClick.AddListener(delegate() { OnRoomClick(row); });
			}
		}
	}

	private void OnRoomClick(ListItem row)
	{
		_session.ServerConnection.JoinRoom(((GameRoom)row.State).RoomID);
	}

	private void ServerConnection_ReceivedRooms(object sender, System.EventArgs e)
	{
		_reload = true;
	}

	void ServerConnection_JoinedRoom(object sender, System.EventArgs e)
	{
		_joined = true;
	}

	#endregion
}
