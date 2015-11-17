using UnityEngine;
using System.Collections.Generic;
using System.Timers;
using System.Net;

public class ClientManager : MonoBehaviour
{
	#region Vars

	// Prefabs
	public GameObject BoatPrefab;
	public GameObject CameraPrefab;

	// Current Session
	private LevelSettings _currentLevel;
	public RoomPlayerInfo[] ConnectedPlayers;
	private Dictionary<EndPoint, SpawnedPlayer> _players;

	// Current Player
	private BoatManager _playerBoat;
	private Rigidbody _rigid;
	public int PlayerID;

	private float _updateInterval = 50f;
	private Timer _updateTimer;

	// Networking
	private ClientToClient _connection;
	private PositioningData _posPackage;
	private byte[] _sendBuffer;

	private bool _started = false;

	#endregion

	#region Methods

	private void Start()
	{
		_posPackage = new PositioningData(Vector3.zero, Vector3.zero);
		_updateTimer = new Timer(_updateInterval);
		_updateTimer.Elapsed += UpdateTimer_Elapsed;
		_updateTimer.AutoReset = true;
		_connection = GameSession.CURRENT.ClientConnection;
		_connection.Received += Connection_Received;
	}

	private void OnApplicationQuit()
	{
		if (_updateTimer != null && _updateTimer.Enabled)
			_updateTimer.Stop();
	}

	private void FixedUpdate()
	{
		if (_started)
		{
			UpdateNetworkPackage();
			UpdatePlayers();
		}
	}

	public void SpawnPlayers()
	{
		_currentLevel = LevelSettings.Current;
		_players = new Dictionary<EndPoint, SpawnedPlayer>();

		foreach (RoomPlayerInfo player in ConnectedPlayers)
		{
			GameObject newShip = (GameObject)Instantiate(BoatPrefab, _currentLevel.SpawnPoints[player.SpawnPointID].position, Quaternion.identity);

			SpawnedPlayer sPlayer = new SpawnedPlayer(newShip, player);
			_players.Add(player.UdpEP, sPlayer);

			// Self
			if (player.PlayerID == PlayerID)
			{
				// Create camera for player
				GameObject cam = (GameObject)Instantiate(CameraPrefab, newShip.transform.position, Quaternion.identity);
				cam.GetComponent<CameraMovement>().Target = newShip.transform;

				_playerBoat = newShip.GetComponent<BoatManager>();
				_rigid = newShip.GetComponent<Rigidbody>();

				UpdateNetworkPackage();
			}
			// Other player
			else
			{
				Destroy(newShip.GetComponent<Rigidbody>());
			}
		}
	}

	private void UpdateNetworkPackage()
	{
		_posPackage.Position.Vector = _playerBoat.transform.position;
		_posPackage.Angle.Vector = _playerBoat.transform.eulerAngles;
	}

	private void UpdatePlayers()
	{
		foreach (SpawnedPlayer player in _players.Values)
		{
			if (player.Info.PlayerID == PlayerID) continue;

			if (player.PosUpdated)
			{
				player.PosUpdated = false;
				player.Boat.transform.position = player.PosData.Position.Vector;
				player.Boat.transform.eulerAngles = player.PosData.Angle.Vector;
			}
		}
	}

	public void StartPlayer()
	{
		if (_started) return;
		_started = true;

		_playerBoat.EnableMovement();
		_updateTimer.Start();
	}

	private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		// Send to all clients
		_sendBuffer = PackageFactory.Pack(PackageType.PositionUpdate, _posPackage);
		foreach (RoomPlayerInfo player in ConnectedPlayers)
		{
			if (player.PlayerID != PlayerID)
			{
				_connection.Send(_sendBuffer, player.UdpEP);
			}
		}
	}

	private void Connection_Received(EndPoint ep, TypedPackage package)
	{
		// Select player
		if (!_players.ContainsKey(ep))
		{
			Debug.Log("Received UDP from unconnected client! By " + ep.ToString());
			return;
		}
		
		SpawnedPlayer p = _players[ep];

		// Handle msg
		if (package.Type == PackageType.PositionUpdate)
		{
			p.PosData.FromBytes(package.Data, ref package.Offset);
			p.PosUpdated = true;
		}
		else
		{
			Debug.Log("Unhandled UDP msg");
		}
	}

	#endregion
}

public class SpawnedPlayer
{
	public GameObject Boat;
	public PositioningData PosData;
	public RoomPlayerInfo Info;
	public bool PosUpdated;

	public SpawnedPlayer(GameObject boat, RoomPlayerInfo info)
	{
		Boat = boat;
		PosData = new PositioningData(Boat.transform.position, Boat.transform.eulerAngles);
		Info = info;
		PosUpdated = true;
	}
}