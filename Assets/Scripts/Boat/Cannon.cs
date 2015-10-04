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

    private float _currentRotation;

    #endregion

    #region Methods

    void Start()
    {
        _currentRotation = _barrel.transform.rotation.eulerAngles.z;
    }

    void Update()
    {
        
    }

    public float ApplyRotation(float angle)
    {
        _currentRotation = Mathf.Clamp(_currentRotation + angle, _minAngle, _maxAngle);
        _barrel.transform.localRotation = Quaternion.Euler(_currentRotation, 0, 0);
        return _barrel.transform.localRotation.x; 
    }

    #endregion
}
