using UnityEngine;
using System.Collections;

public class ObjectHealth : MonoBehaviour
{
	#region Events

	public event HealthEvent Changed;
	public delegate void HealthEvent(ObjectHealth sender);

	#endregion

	#region Vars

	[SerializeField]
	protected float _health;
	[SerializeField]
	protected float _maxHealth = 100f;
	[SerializeField]
	protected bool _alive;
	[SerializeField]
	protected int _objectID;
	
	#endregion

	#region Methods

	public void Start()
	{
		_alive = true;
		_health = _maxHealth;
	}

	public void Regen()
	{
		_alive = true;
		Health = _maxHealth;
	}

	public void Die()
	{
		if (!_alive) return;
		_alive = false;
		Health = 0f;
		OnDeath();
	}

	public void Damage(float damage)
	{
		if (!_alive || !enabled) return;
		Health = _health - damage;
	}

	protected virtual float BoundHealth(float newHealth)
	{
		return Mathf.Min(_maxHealth, Mathf.Max(0f, newHealth));
	}

	protected virtual void CheckDeath()
	{
		if (_health <= 0f && _alive)
			Die();
	}

	protected virtual void OnDeath()
	{
		Destroy(gameObject);
	}

	#endregion

	#region Properties

	public float Health
	{
		get
		{
			return _health;
		}
		set
		{
			_health = BoundHealth(value);
			
			CheckDeath();

			if (Changed != null)
				Changed(this);
		}
	}

	public bool IsAlive
	{
		get
		{
			return _alive;
		}
	}

	public int ID
	{
		get
		{
			return _objectID;
		}
		set
		{
			_objectID = value;
		}
	}

	#endregion
}
