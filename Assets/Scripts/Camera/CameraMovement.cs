using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    #region Vars

    [SerializeField]
    public Transform Target;
    [SerializeField]
    public string HorizontalInput;
    [SerializeField]
    public string VerticalInput;
    [SerializeField]
    private float _horizontalRotationSpeed = 200f;
    [SerializeField]
    private float _verticalRotationSpeed = 150f;
    [SerializeField]
    private float _minVerticalRotation;
    [SerializeField]
    private float _maxVerticalRotation;
    [SerializeField]
    private Vector3 _targetOffset;

    private Vector3 _currentRotation;
    private Vector3 _targetRotation;
    
    #endregion

    #region Methods

    void Start()
    {
        _targetRotation = _currentRotation = transform.eulerAngles;
    }

    void OnEnable()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable()
    {
        //Cursor.visible = true;
    }

    void LateUpdate()
    {
        float horizontal = Input.GetAxis(HorizontalInput);
        float vertical = Input.GetAxis(VerticalInput);
        
        // Calculate Target Rotation
        if (Mathf.Abs(horizontal) > 0f)
        {
            _targetRotation.y = (_currentRotation.y + horizontal * _horizontalRotationSpeed * Time.deltaTime) % 360;
        }
        if (Mathf.Abs(vertical) > 0f)
        {
            _targetRotation.x = Mathf.Clamp((_currentRotation.x + vertical * _verticalRotationSpeed * Time.deltaTime) % 360, _minVerticalRotation, _maxVerticalRotation);
        }
        
        // Apply Rotation
        if (Mathf.Abs(Vector3.Distance(_targetRotation, _currentRotation)) >= 0.1f)
        {
            _currentRotation = _targetRotation;
            transform.rotation = Quaternion.Euler(_currentRotation);
        }

        // Auto move to target
        if (Target != null)
        {
            transform.position = Target.position + _targetOffset;
        }
    }

    #endregion
}
