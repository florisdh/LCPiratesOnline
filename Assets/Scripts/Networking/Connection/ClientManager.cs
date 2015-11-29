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
	private SpawnedPlayer _currentPlayer;
	public int PlayerID;

	// Networking
	private float _updateInterval = 50f;
	private Timer _updateTimer;
	private ClientToClient _connection;
	private byte[] _sendBuffer;

	private bool _started = false;

	#endregion

	#region Methods

	private void Start()
	{
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

			// Self
			if (player.PlayerID == PlayerID)
			{
				// Create camera for player
				GameObject cam = (GameObject)Instantiate(CameraPrefab, newShip.transform.position, Quaternion.identity);
				cam.GetComponent<CameraMovement>().Target = newShip.transform;

				_currentPlayer = sPlayer;

				_currentPlayer.Manager.PartChanged += AddPartUpdate;

				foreach (CannonManager group in _currentPlayer.Manager.CannonGroups)
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
				_players.Add(player.UdpEP, sPlayer);
				Destroy(newShip.GetComponent<Rigidbody>());
			}
		}
	}

	private void AddPartUpdate(ObjectHealth part)
	{
		_currentPlayer.HitBuffer.Add(new HealthData(part.ID, part.Health));
	}

	private void Cannon_OnShoot(Vector3 pos, Vector3 angle, Vector3 velo)
	{
		_currentPlayer.ShootBuffer.Shots.Add(new RigidData(pos, angle, velo));
	}

	private void UpdateNetworkPackage()
	{
		_currentPlayer.Positioning.Position.Vector = _currentPlayer.Boat.transform.position;
		_currentPlayer.Positioning.Angle.Vector = _currentPlayer.Boat.transform.eulerAngles;
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

			if (player.HitBuffer.Count > 0)
			{
				lock (player.HitBuffer)
				{
					foreach (HealthData item in player.HitBuffer)
					{
						player.Manager.UpdatePartHealth(item.PartID, item.Health);
					}
					player.HitBuffer.Clear();
				}
			}
		}
	}

	public void StartPlayer()
	{
		if (_started) return;
		_started = true;

		_currentPlayer.Manager.EnableMovement();
		_updateTimer.Start();
	}

	private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		// Create package //
		List<byte> totalPackage = new List<byte>();

		// Add positioning
		totalPackage.AddRange(PackageFactory.Pack(PackageType.PlayerMove, _currentPlayer.Positioning));
		
		// Add Shots
		if (_currentPlayer.ShootBuffer.Shots.Count > 0)
		{
			lock (_currentPlayer.ShootBuffer.Shots)
			{
				totalPackage.AddRange(PackageFactory.Pack(PackageType.PlayerShoot, _currentPlayer.ShootBuffer));
				_currentPlayer.ShootBuffer.Shots.Clear();
			}
		}

		// Add Hits
		if (_currentPlayer.HitBuffer.Count > 0)
		{
			lock (_currentPlayer.HitBuffer)
			{
				foreach (HealthData item in _currentPlayer.HitBuffer)
				{
					totalPackage.AddRange(PackageFactory.Pack(PackageType.BoatPartHit, item));
				}
				_currentPlayer.HitBuffer.Clear();
			}
		}

		_sendBuffer = totalPackage.ToArray();

		// Send to all clients
		foreach (RoomPlayerInfo player in ConnectedPlayers)
		{
			if (player.PlayerID == PlayerID) continue;

			_connection.Send(_sendBuffer, player.UdpEP);
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
		}
		else if (package.Type == PackageType.BoatPartHit)
		{
			HealthData message = new HealthData(package.Data, ref package.Offset);
			lock (p.HitBuffer)
				p.HitBuffer.Add(message);
		}
		else
		{
			Debug.Log("Unhandled UDP msg");
		}
	}

	#endregion

	#region Properties

	#endregion
}

public class SpawnedPlayer
{
	public GameObject Boat;
	public BoatManager Manager;
	public RoomPlayerInfo Info;
	public bool Updated;
	public PositioningData Positioning;
	public ShootingData ShootBuffer;
	public List<HealthData> HitBuffer;

	public SpawnedPlayer(GameObject boat, RoomPlayerInfo info)
	{
		Boat = boat;
		Manager = boat.GetComponent<BoatManager>();
		Positioning = new PositioningData(Boat.transform.position, Boat.transform.eulerAngles);
		ShootBuffer = new ShootingData();
		HitBuffer = new List<HealthData>();
		Info = info;
		Updated = true;
	}
}