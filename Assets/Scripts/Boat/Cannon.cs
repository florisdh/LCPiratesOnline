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

    #endregion

    #region Methods

    void Start()
    {
    }

    void Update()
    {
        
    }

    public void ApplyRotation(float angleNormal)
    {
        // Bound to 0f and 1f
        angleNormal = Mathf.Clamp01(angleNormal);

        // Apply angle
        float desiredRotation = _minAngle + (_maxAngle - _minAngle) * angleNormal;
        _barrel.transform.localRotation = Quaternion.Euler(desiredRotation, 0, 0);
    }

    #endregion
}
