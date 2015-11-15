using UnityEngine;
using System.Collections.Generic;
using System.Timers;
using System.Net;

public class ClientManager : MonoBehaviour
{
	#region Vars

	public GameObject BoatPrefab;
	public GameObject CameraPrefab;

	public RoomPlayerInfo[] ConnectedPlayers;
	public int PlayerID;

	private Dictionary<EndPoint, SpawnedPlayer> _players;

	private LevelSettings _currentLevel;
	private BoatManager _playerBoat;
	private Rigidbody _rigid;

	private float _updateInterval = 100f;
	private Timer _updateTimer;

	private ClientToClient _connection;
	private RigidData _rigidPackage;
	private byte[] _sendBuffer;

	private bool _started = false;

	private SpawnedPlayer _other;

	#endregion

	#region Methods

	private void Start()
	{
		_rigidPackage = new RigidData(Vector3.zero, Vector3.zero, Vector3.zero);
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

	private void UpdateNetworkPackage()
	{
		_rigidPackage.Positioning.Position.Vector = _playerBoat.transform.position;
		_rigidPackage.Positioning.Angle.Vector = _playerBoat.transform.eulerAngles;
		_rigidPackage.Velocity.Vector = _rigid.velocity;
	}

	private void UpdatePlayers()
	{
		foreach (SpawnedPlayer player in _players.Values)
		{
			if (player.Info.PlayerID == PlayerID) continue;

			if (player.RigidUpdated)
			{
				player.RigidUpdated = false;
				player.Rigid.transform.position = player.RigidData.Positioning.Position.Vector;
				player.Rigid.transform.eulerAngles = player.RigidData.Positioning.Angle.Vector;
				//player.Rigid.velocity = player.RigidData.Velocity.Vector;
			}
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

			if (player.PlayerID == PlayerID)
			{
				// Create camera for player
				GameObject cam = (GameObject)Instantiate(CameraPrefab, newShip.transform.position, Quaternion.identity);
				cam.GetComponent<CameraMovement>().Target = newShip.transform;

				_playerBoat = newShip.GetComponent<BoatManager>();
				_rigid = newShip.GetComponent<Rigidbody>();

				UpdateNetworkPackage();
			}
			else _other = sPlayer;
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
		_sendBuffer = PackageFactory.Pack(PackageType.PositionUpdate, _rigidPackage);
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
		
		//SpawnedPlayer p = _players[ep];
		SpawnedPlayer p = _other;

		// Handle msg
		if (package.Type == PackageType.PositionUpdate)
		{
			p.RigidData.FromBytes(package.Data, ref package.Offset);
			p.RigidUpdated = true;
		}
		else
		{
			Debug.Log("Unhandles UDP msg");
		}
	}

	#endregion
}

public class SpawnedPlayer
{
	public Rigidbody Rigid;
	public RigidData RigidData;
	public bool RigidUpdated;
	public RoomPlayerInfo Info;

	public SpawnedPlayer(GameObject boat, RoomPlayerInfo info)
	{
		Rigid = boat.GetComponent<Rigidbody>();
		RigidData = new RigidData(Rigid.position, Rigid.transform.eulerAngles, Rigid.velocity);
		Info = info;
		RigidUpdated = true;
	}
}