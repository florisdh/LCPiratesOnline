using UnityEngine;
using System.Collections.Generic;
using System.Timers;
using System.Net;
using System;

public class ClientManager : MonoBehaviour
{
	#region Vars

	// Prefabs
	public GameObject BoatPrefab;
	public GameObject CameraPrefab;
	public GameObject ProjectilePrefab;
	public GameObject ShootEffectPrefab;

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
	private PositioningData _posData;
	private ShootingData _shotsData;
	private byte[] _sendBuffer;
	private List<ShootingData> _shootStack;

	private bool _started = false;

	#endregion

	#region Methods

	private void Start()
	{
		_posData = new PositioningData();
		_shotsData = new ShootingData();
		_shootStack = new List<ShootingData>();
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

				foreach (CannonManager group in _playerBoat.CannonGroups)
				{
					foreach (Cannon cannon in group.cannons)
					{
						cannon.OnShoot += Cannon_OnShoot;
					}
				}

				UpdateNetworkPackage();
			}
			// Other player
			else
			{
				Destroy(newShip.GetComponent<Rigidbody>());
			}
		}
	}

	private void Cannon_OnShoot(Vector3 pos, Vector3 angle, Vector3 velo)
	{
		_shotsData.Shots.Add(new RigidData(pos, angle, velo));
	}

	private void UpdateNetworkPackage()
	{
		_posData.Position.Vector = _playerBoat.transform.position;
		_posData.Angle.Vector = _playerBoat.transform.eulerAngles;
	}

	private void UpdatePlayers()
	{
		foreach (SpawnedPlayer player in _players.Values)
		{
			if (player.Info.PlayerID == PlayerID) continue;

			if (player.Updated)
			{
				player.Updated = false;

				player.Boat.transform.position = player.Positioning.Position.Vector;
				player.Boat.transform.eulerAngles = player.Positioning.Angle.Vector;
			}

			if (player.ShootBuffer.Shots.Count > 0)
			{
				lock (player.ShootBuffer.Shots)
				{
					foreach (RigidData shot in player.ShootBuffer.Shots)
					{
						GameObject projectile = (GameObject)Instantiate(ProjectilePrefab, shot.Positioning.Position.Vector, Quaternion.Euler(shot.Positioning.Angle.Vector));
						projectile.GetComponent<Rigidbody>().velocity = shot.Velocity.Vector;

						GameObject effect = (GameObject)Instantiate(ShootEffectPrefab, shot.Positioning.Position.Vector, Quaternion.Euler(shot.Positioning.Angle.Vector));
						Destroy(effect, 3f);
					}
					player.ShootBuffer.Shots.Clear();
				}
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
		// Create package //
		List<byte> totalPackage = new List<byte>();

		// Add positioning
		totalPackage.AddRange(PackageFactory.Pack(PackageType.PlayerMove, _posData));
		
		// Add Shots
		if (_shotsData.Shots.Count > 0)
		{
			totalPackage.AddRange(PackageFactory.Pack(PackageType.PlayerShoot, _shotsData));
			_shotsData.Shots.Clear();
		}
		
		_sendBuffer = totalPackage.ToArray();

		// Send to all clients
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
		if (package.Type == PackageType.PlayerMove)
		{
			p.Positioning.FromBytes(package.Data, ref package.Offset);
			p.Updated = true;
		}
		else if (package.Type == PackageType.PlayerShoot)
		{
			ShootingData message = new ShootingData(package.Data, ref package.Offset);
			p.ShootBuffer.Shots.AddRange(message.Shots);

			Debug.Log("Other shots received! size: " + message.Shots.Count);
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
	public RoomPlayerInfo Info;
	public bool Updated;
	public PositioningData Positioning;
	public ShootingData ShootBuffer;

	public SpawnedPlayer(GameObject boat, RoomPlayerInfo info)
	{
		Boat = boat;
		Positioning = new PositioningData(Boat.transform.position, Boat.transform.eulerAngles);
		ShootBuffer = new ShootingData();
		Info = info;
		Updated = true;
	}
}