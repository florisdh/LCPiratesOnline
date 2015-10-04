using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
    #region Vars

    [SerializeField]
    public Transform Target;
    [SerializeField]
    public string HorizontalInput;
    [SerializeField]
    public string VerticalInput;
    [SerializeField]
    private float RotationSpeed = 60; // Deg per second max
    [SerializeField]
    private bool AutoRetracktToTarget = true;
    [SerializeField]
    private float RetracktToTargetAfter = 3f;
    [SerializeField]
    private float RetracktSpeed = 40;
    private float CurrentRotation;
    private float TargetRotation;
    private float LastInputTimer = 0f;

    #endregion

    #region Methods

    void Start()
    {
        TargetRotation = CurrentRotation = transform.eulerAngles.y;
        LastInputTimer = RetracktSpeed;
    }

    void OnEnable()
    {
        Cursor.visible = false;
    }

    void OnDisable()
    {
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        float horizontal = Input.GetAxis(HorizontalInput);
        
        // Calculate Target Rotation
        if (Mathf.Abs(horizontal) > 0f)
        {
            TargetRotation = (CurrentRotation + horizontal * RotationSpeed * Time.deltaTime) % 360;
            if (AutoRetracktToTarget) LastInputTimer = 0f;
        }
        else if (AutoRetracktToTarget && Target != null)
        {
            // Go back to angle of ship if idle too long
            if (LastInputTimer >= RetracktToTargetAfter)
            {
                TargetRotation = Mathf.MoveTowardsAngle(CurrentRotation, Target.eulerAngles.y, RetracktSpeed) % 360;
            }
            // Track last input time
            else
            {
                LastInputTimer += Time.deltaTime;
            }
        }
        
        // Apply Rotation
        if (Mathf.Abs(Mathf.DeltaAngle(TargetRotation, CurrentRotation)) >= 1f)
        {
            CurrentRotation = TargetRotation;
            transform.rotation = Quaternion.Euler(0, CurrentRotation, 0);
        }

        if (Target != null)
        {
            transform.position = Target.position;
        }
    }

    #endregion
}
