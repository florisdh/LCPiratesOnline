using UnityEngine;
using System.Collections;

public class Cannon : MonoBehaviour
{
    #region Vars

    [SerializeField]
    private GameObject _barrel;
    [SerializeField]
    private float _maxAngle;
    [SerializeField]
    private float _minAngle;
    [SerializeField]
    private GameObject _cannonBall;
    [SerializeField]
    private float _force;
    [SerializeField]
    private float _maxInterval;
    private ParticleSystem Particles;
    private AudioSource _shootSound;

    private float _shootTiming = 1;
    private float _shootTimer;

    private bool _shooting;

    public int row;

    #endregion

    #region Methods

    void Start()
    {
        Particles = GetComponentInChildren<ParticleSystem>();
        _shootSound = GetComponent<AudioSource>();
        _shootTiming = Random.Range(0.1f, 0.4f);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }

        if(_shooting)
        {
            if(_shootTimer > _shootTiming)
            {
                _shootTimer = 0;
                _shooting = false;
                _shootTiming = Random.Range(0.1f, 0.7f);

                GameObject newProjectile = (GameObject)Instantiate(_cannonBall, _barrel.transform.position + _barrel.transform.forward * 0.7f * _barrel.transform.lossyScale.z, _barrel.transform.rotation);
                newProjectile.GetComponent<Rigidbody>().velocity = transform.parent.GetComponent<Rigidbody>().velocity;
                newProjectile.GetComponent<Rigidbody>().AddForce(_barrel.transform.forward * _force);

                Particles.Play();
                _shootSound.Play();
            }
            else
            {
                _shootTimer += Time.deltaTime;
            }
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
        if(_barrel == null)
        {
            Destroy(this);
            return;
        }

        _shooting = true;
        
    }

    #endregion
}