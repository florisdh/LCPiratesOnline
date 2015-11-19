using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour
{
	#region Events

	public event RigidEvent OnShoot;
	public delegate void RigidEvent(Vector3 pos, Vector3 angle, Vector3 velo);

	#endregion

	#region Vars

	[SerializeField]
    private ParticleSystem Particles;
    [SerializeField]
    private GameObject _barrel;
    [SerializeField]
    private GameObject _projectilePrefab;
    private AudioSource _shootSound;

    [SerializeField]
    private float _maxAngle;
    [SerializeField]
    private float _minAngle;
    [SerializeField]
    private float _force;

	private float _minDelay = 0.1f;
	private float _maxDelay = 0.3f;

    private float _shootDelay;
    private bool _shooting;

    #endregion

    #region Methods

    void Start()
    {
        _shootSound = GetComponent<AudioSource>();
    }

    void Update()
    {
		if (_barrel == null)
		{
			Destroy(this);
			return;
		}
    }

    public void ApplyRotation(float angleNormal)
    {
        // Bound to 0f and 1f
        angleNormal = Mathf.Clamp01(angleNormal);

        // Apply angle
        float desiredRotation = _minAngle + (_maxAngle - _minAngle) * angleNormal;
        _barrel.transform.localRotation = Quaternion.Euler(desiredRotation, 0, 0);
    }

    public void Shoot()
    {
		if (_shooting) return;
        _shooting = true;

		_shootDelay = Random.Range(_minDelay, _maxDelay);
		Invoke("Fire", _shootDelay);
    }

	private void Fire()
	{
		if (!_shooting) return;
		_shooting = false;

		// Spawn
		Vector3 targetPos = _barrel.transform.position + _barrel.transform.forward * 0.7f * _barrel.transform.lossyScale.z;
		GameObject projectile = (GameObject)Instantiate(_projectilePrefab, targetPos, _barrel.transform.rotation);
		Rigidbody projectileRigid = projectile.GetComponent<Rigidbody>();

		// Apply force
		projectileRigid.velocity = transform.parent.GetComponent<Rigidbody>().velocity;
		projectileRigid.AddForce(_barrel.transform.forward * _force);

		// User feedback
		Particles.Play();
		_shootSound.Play();

		if (OnShoot != null)
			OnShoot(projectile.transform.position, projectile.transform.eulerAngles, projectileRigid.velocity);
	}

    #endregion
}