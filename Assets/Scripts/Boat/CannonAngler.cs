using UnityEngine;
using System.Collections;

public class CannonAngler : MonoBehaviour
{
    #region Vars

    [SerializeField]
    private int _angleSpeed;
    [SerializeField]
    private GameObject _leftUIArrow;
    [SerializeField]
    private GameObject _rightUIArrow;
    [SerializeField]
    private GameObject _angleIndicator;

    private BoatManager _manager;
    private float _currentAngle;
    public string InputAxis;
    

    #endregion

    #region Methods

    void Start()
    {
        _manager = GetComponent<BoatManager>();
        
    }

    void Update()
    {
        float change = Input.GetAxis(InputAxis);
        if(Mathf.Abs(change) > 0.1f)
        {
            ChangeCannonAngle(_angleSpeed * Time.deltaTime * change);
        }
        
    }

    void ChangeCannonAngle(float angle)
    {
        foreach (Cannon cannon in _manager.Cannons)
        {
            _currentAngle = cannon.ApplyRotation(angle);
        }
        _angleIndicator.GetComponent<AngleIndicator>().IndicateNewAngle(_currentAngle);

    }

    #endregion
}
