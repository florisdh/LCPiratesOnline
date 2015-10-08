using UnityEngine;
using System.Collections;

public class AngleIndicator : MonoBehaviour
{
    #region Vars

    [SerializeField]
    public CannonAngler AngleInput;
    [SerializeField]
    private GameObject _leftArrow;
    [SerializeField]
    private GameObject _rightArrow;
    [SerializeField]
    private float _maxAngle;
    [SerializeField]
    private float _minAngle;
    private float _lastAngleNormal = -1f;

    #endregion

    #region Methods

    private void Update()
    {
        float newAngle = AngleInput.AngleNormal;
        if (newAngle != _lastAngleNormal)
        {
            IndicateNewAngle(newAngle);
        }
    }

    public void IndicateNewAngle(float angleNormal)
    {
        float desiredAngle = _minAngle + (_maxAngle - _minAngle) * angleNormal;
        _leftArrow.transform.localRotation = Quaternion.Euler(0, 0, desiredAngle);
        _rightArrow.transform.localRotation = Quaternion.Euler(0, 0, -desiredAngle);
        _lastAngleNormal = angleNormal;
    }

    #endregion
}
