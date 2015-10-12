using UnityEngine;
using UnityEngine.UI;

public class CreateRoomMenu : Menu
{
    #region Vars

    [SerializeField]
    private InputField _nameInput;
    [SerializeField]
    private Slider _maxInput;
    [SerializeField]
    private Dropdown _mapInput;

    private GameSession _session;

    #endregion

    #region Methods

    private void OnEnable()
    {
        _session = GameSession.CURRENT;
    }

    public void CreateGame()
    {
        if (_nameInput.text == string.Empty)
        {
            // TODO: show error
            return;
        }

        _session.ServerConnection.CreateRoom(_nameInput.text, (int)_maxInput.value, _mapInput.value);
    }

    #endregion
}
