using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FadingUI))]
public class ErrorPanel : MonoBehaviour
{
    #region Vars

    [SerializeField]
    private Text _errorIndicator;

    [SerializeField]
    private FadingUI _fader;

    private float _maxLifeTime = 1f;
    private float _lifeTime;

    private float _currentAlpha;

    #endregion

    #region Methods

    private void Awake()
    {
        _fader = GetComponent<FadingUI>();
    }

    private void Update()
    {
        if (_lifeTime < _maxLifeTime)
        {
            _lifeTime += Time.deltaTime;

            if (_lifeTime >= _maxLifeTime)
            {
                Hide();
            }
        }
    }

    public void Show(float duration = 0.5f)
    {
        _fader.FadeIn(duration);
        _lifeTime = -duration;
    }

    public void Hide(float duration = 0.5f)
    {
        _fader.FadeOut(duration);
    }

    public void ShowError(string text)
    {
        Show();
        _errorIndicator.text = text;
    }

    #endregion
}
