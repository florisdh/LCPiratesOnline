using UnityEngine;
using UnityEngine.UI;

public class SliderValueUI : MonoBehaviour
{
    #region Vars

    [SerializeField]
    private Text _indicator;
    private Slider _slider;

    #endregion

    #region Methods

    public void Start()
    {
        _slider = GetComponent<Slider>();
    }

    public void OnSliderValueChanged()
    {
        _indicator.text = _slider.value.ToString();
    }

    #endregion
}
