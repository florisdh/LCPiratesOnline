using UnityEngine;
using System.Collections;

public class BoatManager : MonoBehaviour
{
    #region Vars

    public Cannon[] Cannons;
	private BoatMovement _movement;
	private CannonAngler _angler;
	private CannonManager[] _cannonManagers;

	private bool _enabled;
	private bool _desiredEnabled;

    #endregion

    #region Methods

    void Awake()
    {
        Cannons = GetComponentsInChildren<Cannon>();
		_movement = GetComponent<BoatMovement>();
		_angler = GetComponent<CannonAngler>();
		_cannonManagers = GetComponentsInChildren<CannonManager>();
    }

	void FixedUpdate()
	{
		if (_desiredEnabled != _enabled)
		{
			_enabled = _desiredEnabled;
			_movement.enabled = _angler.enabled = _enabled;
			foreach (CannonManager manager in _cannonManagers)
			{
				manager.enabled = _enabled;
			}
		}
	}

	public void EnableMovement()
	{
		_desiredEnabled = true;
	}

    #endregion
}
