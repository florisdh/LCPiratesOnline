using UnityEngine;
using System.Collections;
using System.Timers;

public class ClientManager : MonoBehaviour
{
	#region Vars

	public GameObject BoatPrefab;
	public GameObject CameraPrefab;

	public RoomPlayerInfo[] ConnectedPlayers;
	public int PlayerID;

	private LevelSettings _currentLevel;
	private BoatManager _playerBoat;
	private Rigidbody _rigid;

	private float _updateInterval = 1000f;
	private Timer _updateTimer;

	private ClientToClient _connection;
	private RigidData _rigidPackage;
	private byte[] _sendBuffer;

	private bool _started = false;

	#endregion

	#region Methods

	private void Start()
	{
		_rigidPackage = new RigidData(Vector3.zero, Vector3.zero, Vector3.zero);
		_updateTimer = new Timer(_updateInterval);
		_updateTimer.Elapsed += UpdateTimer_Elapsed;
		_updateTimer.AutoReset = true;
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
		}
	}

	public void SpawnPlayers()
	{
		_currentLevel = LevelSettings.Current;
		foreach (RoomPlayerInfo player in ConnectedPlayers)
		{
			GameObject newShip = (GameObject)Instantiate(BoatPrefab, _currentLevel.SpawnPoints[player.SpawnPointID].position, Quaternion.identity);

			if (player.PlayerID == PlayerID)
			{
				// Create camera for player
				GameObject cam = (GameObject)Instantiate(CameraPrefab, newShip.transform.position, Quaternion.identity);
				cam.GetComponent<CameraMovement>().Target = newShip.transform;

				_playerBoat = newShip.GetComponent<BoatManager>();
				_rigid = newShip.GetComponent<Rigidbody>();

				UpdateNetworkPackage();
			}

		}
	}

	private void UpdateNetworkPackage()
	{
		_rigidPackage.Positioning.Position.Vector = _playerBoat.transform.position;
		_rigidPackage.Positioning.Angle.Vector = _playerBoat.transform.eulerAngles;
		_rigidPackage.Velocity.Vector = _rigid.velocity;
	}

	public void StartPlayer()
	{
		if (_started) return;
		_started = true;

		_playerBoat.EnableMovement();
		_connection = GameSession.CURRENT.ClientConnection;

		_updateTimer.Start();
	}

	private void UpdateTimer_Elapsed(object sender, ElapsedEventArgs e)
	{
		// Send to all clients
		_sendBuffer = _rigidPackage.ToBytes();
		foreach (RoomPlayerInfo player in ConnectedPlayers)
		{
			if (player.PlayerID != PlayerID)
			{
				_connection.Send(_sendBuffer, player.UdpEP);
			}
		}
	}

	#endregion
}