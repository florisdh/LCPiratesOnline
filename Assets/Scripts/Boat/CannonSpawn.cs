using UnityEngine;
using System.Collections;

public class CannonSpawn : MonoBehaviour
{
	#region Vars

	[SerializeField]
	private GameObject _prefab;

	[SerializeField]
	private CannonManager _targetManager;

	#endregion

	#region Methods

	private void Awake()
	{
		GameObject obj = (GameObject)Instantiate(_prefab, transform.position, transform.rotation);
		obj.transform.parent = transform.parent;
		_targetManager.cannons.Add(obj.GetComponent<Cannon>());
		Destroy(gameObject);
	}

	#endregion
}
