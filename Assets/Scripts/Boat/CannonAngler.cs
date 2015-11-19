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
	
	// Current angle between 0f and 1f
    private float _currentAngleNormal;

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
		foreach (CannonManager group in _manager.CannonGroups)
		{
			foreach (Cannon cannon in group.cannons)
			{
				cannon.ApplyRotation(_currentAngleNormal);
			}
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
