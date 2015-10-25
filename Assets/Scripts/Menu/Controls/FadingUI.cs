using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class FadingUI : MonoBehaviour
{
    #region Vars

    private float _currentAlpha;
    private float _fadeVelo;
    private bool _fading = false;

    private CanvasGroup _panel;

    #endregion

    #region Methods

    private void Awake()
    {
        _panel = GetComponent<CanvasGroup>();
        _currentAlpha = _panel.alpha;
    }

    private void Update()
    {
        if (_fading)
        {
            _currentAlpha += _fadeVelo * Time.deltaTime;

            if ((_fadeVelo < 0f && _currentAlpha <= 0f) || (_fadeVelo > 0f && _currentAlpha >= 1f))
            {
                _fading = false;
                _currentAlpha = Mathf.Clamp01(_currentAlpha);
            }

            _panel.alpha = _currentAlpha;
        }
    }

    public void FadeIn(float duration)
    {
        _fading = true;
        _fadeVelo = 1f / duration;
    }

    public void FadeOut(float duration)
    {
        _fading = true;
        _fadeVelo = -1f / duration;
    }

    #endregion
}
