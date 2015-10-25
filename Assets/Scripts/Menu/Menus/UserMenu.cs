using UnityEngine;
using UnityEngine.UI;

public class UserMenu : Menu
{
    #region Vars

    [SerializeField]
    private Text _header;

    private GameSession _session;

    #endregion

    #region Methods

    private void OnEnable()
    {
        _session = GameSession.CURRENT;
        _header.text = _session.ServerConnection.UserName;
    }

	public void Logout()
	{
		_session.ServerConnection.Logout();
		ShowPrevious();
	}

    #endregion
}
