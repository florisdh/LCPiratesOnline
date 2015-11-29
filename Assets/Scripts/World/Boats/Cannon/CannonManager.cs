using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CannonManager : MonoBehaviour
{
	#region Vars

	public List<Cannon> cannons;

	[SerializeField]
	private string _inputAxis;
	
	[SerializeField]
	private float _shootInterval = 1f;
	private float _shootTimer = 0f;

	[SerializeField]
	private float _delay = 0.1f;

	private bool _shooting = false;

	#endregion

	#region Construct

	public CannonManager()
		: base()
	{
		cannons = new List<Cannon>();
	}

	#endregion

	#region Methods

	private void Update()
    {
		if (_shooting)
		{
			if (_shootTimer < _shootInterval)
			{
				_shootTimer += Time.deltaTime;
			}
			else
			{
				_shooting = false;
				_shootTimer = 0f;
			}
		}
		else if (Input.GetAxis(_inputAxis) > 0.5f)
        {
			_shooting = true;
			Invoke("FireCannons", _delay);
        }
    }

	private void FireCannons()
    {
		foreach (Cannon cannon in cannons)
		{
			cannon.Shoot();
		}
	}

	#endregion

	#region Properties

	#endregion
}
