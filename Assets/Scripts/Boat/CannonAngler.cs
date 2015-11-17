using UnityEngine;
using System.Collections;

public class CannonAngler : MonoBehaviour
{
    #region Vars

	[SerializeField]
    private string _inputAxis;
    private BoatManager _manager;

    [SerializeField]
    private float _angleSpeed;
    private float _currentAngleNormal; // Current angle between 0f and 1f

    #endregion

    #region Methods

    private void Start()
    {
        _manager = GetComponent<BoatManager>();
		ApplyCannonsAngle();
    }

    private void Update()
    {
        float input = Input.GetAxis(_inputAxis);
        if (input != 0f)
        {
            _currentAngleNormal = Mathf.Clamp01(_currentAngleNormal + _angleSpeed * Time.deltaTime * input);
            ApplyCannonsAngle();
        }
    }

    private void ApplyCannonsAngle()
    {
        foreach (Cannon cannon in _manager.Cannons)
        {
            cannon.ApplyRotation(_currentAngleNormal);
        }
    }

    #endregion

    #region Properties

    public float AngleNormal
    {
        get { return _currentAngleNormal; }
    }

    #endregion
}
