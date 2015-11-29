using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
	#region Vars

	private float _radius = 2f;
	private float _damage = 150f;

	#endregion

	#region Methods

	private void Start()
	{
		Explode();
	}

	private void Explode()
	{
		Collider[] items = Physics.OverlapSphere(transform.position, _radius);

		ObjectHealth itemHealth;
		foreach (Collider item in items)
		{
			itemHealth = item.GetComponent<ObjectHealth>();
			if (itemHealth == null || !item.enabled) continue;

			float range = (item.ClosestPointOnBounds(transform.position) - transform.position).magnitude;
			float damageScalar = 1f - (range / _radius);

			itemHealth.Damage(_damage * damageScalar);

			//Debug.Log(string.Format("Collided with {0} of type {1} at range {2} scale {3}", new object[] { item.gameObject.name, item.gameObject.tag, range, damageScalar }));
		}
	}

	#endregion
}
