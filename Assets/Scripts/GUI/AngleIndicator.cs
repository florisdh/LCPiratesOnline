using UnityEngine;
using System.Collections;

public class AngleIndicator : MonoBehaviour
{

    #region Vars

    [SerializeField]
    private GameObject _leftArrow;
    [SerializeField]
    private GameObject _rightArrow;
    [SerializeField]
    private float _maxAngle;
    [SerializeField]
    private float _minAngle;
    #endregion

    #region Methods

    public void IndicateNewAngle(float angle)
    {
        _leftArrow.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    #endregion
}
