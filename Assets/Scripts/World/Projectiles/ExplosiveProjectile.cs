using UnityEngine;
using System.Collections;

public class ExplosiveProjectile : MonoBehaviour
{
	#region Vars

	[SerializeField]
	private GameObject _explosionPrefab;

	private float _activationTime = 0.5f;
	private bool _active = false;

	#endregion

	#region Methods

	public void Awake()
	{
		Invoke("Activate", _activationTime);
	}

	private void Activate()
	{
		_active = true;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!_active) return;
		_active = false;

		Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}

	#endregion
}
