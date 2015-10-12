using UnityEngine;
using System.Collections;

public class BoatMovement : MonoBehaviour
{
    #region Vars

    [SerializeField]
    public string HorizontalInput;
    [SerializeField]
    public string VerticalInput;
    [SerializeField]
    private float Speed;
    [SerializeField]
    private float TurnSpeed;
    [SerializeField]
    private float _turnSlower;

    private Rigidbody _rigid;

    #endregion

    #region Methods

    void Start()
    {
        _rigid = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {
        float inputForward = Input.GetAxis(VerticalInput);
        float inputRight = Input.GetAxis(HorizontalInput);    

        if (inputForward != 0f)
        {
            if(inputRight != 0)
            {
                _rigid.AddForce(transform.forward * inputForward * Speed / (Mathf.Abs(inputRight) * _turnSlower));
            }
            else
            {
                _rigid.AddForce(transform.forward * inputForward * Speed);
            }
            
        }

        if (inputRight != 0f)
        {
            _rigid.AddTorque(0, inputRight * TurnSpeed, 0f);
        }
        
    }

    #endregion
}
