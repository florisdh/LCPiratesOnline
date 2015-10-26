using UnityEngine;
using UnityEngine.UI;

public class RoomLobbyMenu : Menu
{
	#region Vars

	[SerializeField]
	private Text _header;
	[SerializeField]
	private ListView _playerListView;
	[SerializeField]
	private Dropdown _boatInput;
	[SerializeField]
	private Dropdown _colorInput;
	[SerializeField]
	private Toggle _readyInput;

	private GameSession _session;
	private PlayerSetup _setup;

	private bool _updateList;

	#endregion

	#region Methods

	private void OnEnable()
	{
		_session = GameSession.CURRENT;
		_session.ServerConnection.OtherJoinedRoom += ServerConnection_OtherJoinedRoom;
		_session.ServerConnection.OtherLeftRoom += ServerConnection_OtherLeftRoom;
		_session.ServerConnection.OtherChangedSetup += ServerConnection_OtherChangedSetup;
		_header.text = _session.ServerConnection.ConnectedRoom.Name;

		// Load current setup
		foreach (RoomPlayerInfo info in _session.ServerConnection.ConnectedRoom.Players)
		{
			if (info.PlayerID == _session.ServerConnection.PlayerID)
			{
				_setup = info.Setup;
				_boatInput.value = _setup.BoatID;
				_colorInput.value = _setup.FlagColorID;
				_readyInput.isOn = _setup.Ready;
				break;
			}
		}
		if (_setup == null)
		{
			Debug.Log("Failed to load player setup.");
			gameObject.SetActive(false);
		}

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

	public void OnChangedSetup()
	{
		_setup.BoatID = _boatInput.value;
		_setup.FlagColorID = _colorInput.value;
		_setup.Ready = _readyInput.isOn;
		_session.ServerConnection.ChangeSetup(_setup);
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
	}

	private void ServerConnection_OtherChangedSetup(object sender, RoomPlayerInfo player)
	{
		_updateList = true;
	}
	
	#endregion
}
