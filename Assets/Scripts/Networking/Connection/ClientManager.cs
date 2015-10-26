using UnityEngine;
using System.Collections;

public class ClientManager : MonoBehaviour
{
	#region Vars

	public GameObject BoatPrefab;
	public GameObject CameraPrefab;

	public RoomPlayerInfo[] ConnectedPlayers;
	public int PlayerID;

	private LevelSettings _currentLevel;

	#endregion

	#region Methods

	public void SpawnPlayers()
	{
		_currentLevel = LevelSettings.Current;
		foreach (RoomPlayerInfo player in ConnectedPlayers)
		{
			Debug.Log(string.Format("Trying to spawn {0} at spawn {1}", player.PlayerName, player.SpawnPointID));
			GameObject newShip = (GameObject)Instantiate(BoatPrefab, _currentLevel.SpawnPoints[player.SpawnPointID].position, Quaternion.identity);

			if (player.PlayerID == PlayerID)
			{
				GameObject cam = (GameObject)Instantiate(CameraPrefab, newShip.transform.position, Quaternion.identity);
				cam.GetComponent<CameraMovement>().Target = newShip.transform;
			}

		}
	}

	#endregion
}