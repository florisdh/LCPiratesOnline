using UnityEngine;

public class BoatManager : MonoBehaviour
{
	#region Events

	public event ObjectHealth.HealthEvent PartChanged;

	#endregion

	#region Vars

	private BoatMovement _movement;
	private CannonAngler _angler;
	private CannonManager[] _cannonManagers;

	[SerializeField]
	private ObjectHealth[] _parts;

	private bool _loaded;
	private bool _enabled;
	private bool _desiredEnabled;
	
    #endregion

    #region Methods

    private void Awake()
    {
		_movement = GetComponent<BoatMovement>();
		_angler = GetComponent<CannonAngler>();
		_cannonManagers = GetComponentsInChildren<CannonManager>(true);
		
		// Index parts for faster access
		_parts = GetComponentsInChildren<ObjectHealth>();
		for (int i = 0; i < _parts.Length; i++)
		{
			_parts[i].ID = i;
			_parts[i].Changed += OnPartChange;
		}

		_loaded = true;
    }

	private void Update()
	{
		if (_loaded && _enabled != _desiredEnabled)
		{
			_enabled = _desiredEnabled;
			try
			{
				UpdateEnabled();
			}
			catch (System.Exception e)
			{
				Debug.Log(e);
			}
		}
	}

	private void OnPartChange(ObjectHealth item)
	{
		if (_enabled && PartChanged != null)
			PartChanged(item);
	}

	public void EnableMovement()
	{
		if (_enabled || _desiredEnabled) return;
		_desiredEnabled = true;
	}

	public void UpdatePartHealth(int id, float newHealth)
	{
		if (id < 0 || id >= _parts.Length || _parts[id] == null) return;
		_parts[id].Health = newHealth;
	}

	private void UpdateEnabled()
	{
		_movement.enabled = _angler.enabled = _enabled;
		foreach (CannonManager manager in _cannonManagers)
		{
			manager.enabled = _enabled;
		}
		foreach (ObjectHealth part in _parts)
		{
			part.enabled = _enabled;
		}
	}

    #endregion

	#region Methods

	public CannonManager[] CannonGroups
	{
		get { return _cannonManagers; }
	}

	#endregion
}
