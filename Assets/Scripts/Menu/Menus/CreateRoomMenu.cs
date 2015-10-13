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

    [SerializeField]
    private Menu _waitRoomMenu;

    private GameSession _session;

	private bool _result = false;

    #endregion

    #region Methods

    private void OnEnable()
    {
        _session = GameSession.CURRENT;
        _session.ServerConnection.CreatedRoom += ServerConnection_CreatedRoom;
		_session.ServerConnection.JoinedRoom += ServerConnection_JoinedRoom;
    }
    private void OnDisable()
    {
        if (_session != null)
        {
            _session.ServerConnection.CreatedRoom -= ServerConnection_CreatedRoom;
			_session.ServerConnection.JoinedRoom -= ServerConnection_JoinedRoom;
            _session = null;
        }
    }

	private void FixedUpdate()
	{
		if (_result)
		{
			_result = false;
			_waitRoomMenu.Show(this);
			return;
		}
	}

    private void ServerConnection_CreatedRoom(object sender, System.EventArgs e)
    {
		
    }

	private void ServerConnection_JoinedRoom(object sender, System.EventArgs e)
	{
		_result = true;
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
