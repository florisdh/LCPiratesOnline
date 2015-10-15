using UnityEngine;
using UnityEngine.UI;

public class RoomLobbyMenu : Menu
{
	#region Vars

	[SerializeField]
	private Text _header;

	[SerializeField]
	private ListView _playerListView;

	private GameSession _session;

	private bool _updateList;

	#endregion

	#region Methods

	private void OnEnable()
	{
		_session = GameSession.CURRENT;
		_session.ServerConnection.OtherJoinedRoom += ServerConnection_OtherJoinedRoom;
		_session.ServerConnection.OtherLeftRoom += ServerConnection_OtherLeftRoom;
		_header.text = _session.ServerConnection.ConnectedRoom.Name;
		_updateList = true;
	}
	
	private void OnDisable()
	{
		if (_session != null)
		{
			_session.ServerConnection.OtherJoinedRoom -= ServerConnection_OtherJoinedRoom;
			_session.ServerConnection.OtherLeftRoom -= ServerConnection_OtherLeftRoom;
			_session = null;
		}
	}

	private void FixedUpdate()
	{
		if (_updateList)
		{
			_updateList = false;
			_playerListView.Clear();
			foreach (RoomPlayerInfo info in _session.ServerConnection.ConnectedRoom.Players)
			{
				AddPlayerRow(info);
			}
		}
	}

	public void Leave(Menu targetMenu)
	{
		_session.ServerConnection.LeaveRoom();
		targetMenu.Show(this);
	}

	private void AddPlayerRow(RoomPlayerInfo playerInfo)
	{
		_playerListView.AddRow(new string[] { playerInfo.PlayerName, playerInfo.Setup.BoatID.ToString(), playerInfo.Setup.FlagColorID.ToString(), playerInfo.Setup.Ready.ToString() });
	}

	private void ServerConnection_OtherJoinedRoom(object sender, RoomPlayerInfo player)
	{
		_updateList = true;
	}

	private void ServerConnection_OtherLeftRoom(object sender, RoomPlayerInfo player)
	{
		_updateList = true;
		Debug.Log("Player left " + player.PlayerName);
	}

	#endregion
}
