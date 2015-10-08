using UnityEngine;
using System.Collections;

public class CameraZoom : MonoBehaviour
{
    #region Vars

    public string ZoomAxis;
    [SerializeField]
    private float _zoomSpeed = 100f;
    [SerializeField]
    private float _minRange = 20f;
    [SerializeField]
    private float _maxRange = 100f;
    private float _currentRange;
    private float _targetRange;

    #endregion

    #region Methods

    void Start()
    {
        _currentRange = _targetRange = -transform.localPosition.z;
    }

    void Update()
    {
        float zoomAxis = -Input.GetAxis(ZoomAxis);

        if (Mathf.Abs(zoomAxis) >= 0.1f)
        {
            _targetRange = Mathf.Clamp(_currentRange + _zoomSpeed * zoomAxis, _minRange, _maxRange);
        }

        if (Mathf.Abs(_targetRange - _currentRange) >= 0.1f)
        {
            _currentRange = Mathf.Lerp(_currentRange, _targetRange, Time.deltaTime);
            Vector3 newPos = transform.localPosition;
            newPos.z = -_currentRange;
            transform.localPosition = newPos;
        }
    }

    #endregion
}
