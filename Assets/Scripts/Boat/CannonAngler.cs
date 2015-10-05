using UnityEngine;
using System.Collections;

public class CannonAngler : MonoBehaviour
{
    #region Vars

    public string InputAxis;

    [SerializeField]
    private float _angleSpeed;
    private BoatManager _manager;
    private float _currentAngleNormal; // Current angle between 0f and 1f

    #endregion

    #region Methods

    private void Start()
    {
        _manager = GetComponent<BoatManager>();
    }

    private void Update()
    {
        float change = Input.GetAxis(InputAxis);
        if (change != 0f)
        {
            _currentAngleNormal = Mathf.Clamp01(_currentAngleNormal + _angleSpeed * Time.deltaTime * change);
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
