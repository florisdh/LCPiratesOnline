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

	#endregion

	#region Methods

	private void OnEnable()
	{
		_session = GameSession.CURRENT;
		_header.text = _session.ServerConnection.ConnectedRoom.Name;

		foreach (RoomPlayerInfo info in _session.ServerConnection.ConnectedRoom.Players)
		{
			Debug.Log("Added player: " + info.PlayerName);
			_playerListView.AddRow(new string[] { info.PlayerName, info.Setup.BoatID.ToString(), info.Setup.FlagColorID.ToString(), info.Setup.Ready.ToString() });
		}
	}

	private void OnDisable()
	{
		if (_session != null)
		{
			_session = null;
		}
	}

	#endregion
}
