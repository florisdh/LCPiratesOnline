using UnityEngine;

public class JoinRoomMenu : Menu
{
	#region Vars

	[SerializeField]
	private ListView _roomListView;

	private GameSession _session;

	private bool _reload = false;

	#endregion

	#region Methods

	private void OnEnable()
	{
		_session = GameSession.CURRENT;
		_session.ServerConnection.ReceivedRooms += ServerConnection_ReceivedRooms;
		_session.ServerConnection.RequestRooms();
	}

	private void OnDisable()
	{
		if (_session != null)
		{
			_session = null;
		}
	}

	private void FixedUpdate()
	{
		if (_reload)
		{
			_reload = false;
			foreach (GameRoom info in _session.ServerConnection.OpenRooms)
			{
				Debug.Log("Added room: " + info.Name);
				_roomListView.AddRow(new string[] { info.Name, info.MapID.ToString(), string.Format("{0}/{1}", info.Players.Count, info.MaxPlayers) });
			}
		}
	}

	private void ServerConnection_ReceivedRooms(object sender, System.EventArgs e)
	{
		_reload = true;
	}

	#endregion
}
