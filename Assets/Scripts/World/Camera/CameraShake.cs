using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    #region Vars

    [SerializeField]
    private GameObject _camera;

    private float _shakeTiming = 0.5f;
    private float _shakeTime = 0;
    private float _shakeAmt = 0.25f;

    private bool _shaking;

    private Vector2 _shakePos;

    #endregion

    #region Methods

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            _shaking = true;
        }

        if(_shaking)
        {
            if(_shakeTime > _shakeTiming)
            {
                _shakeTime = 0;
                _shaking = false;
            }
            else
            {
                _shakePos = Random.insideUnitCircle * _shakeAmt;
                this.transform.position = new Vector3(transform.position.x + _shakePos.x, transform.position.y + _shakePos.y, this.transform.position.z);
                _shakeTime += Time.deltaTime;
            }
        }

    }

    public void Shake()
    {
        _shaking = true;
    }

    #endregion
}
