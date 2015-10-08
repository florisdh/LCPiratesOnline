using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginMenu : Menu
{
    #region Vars

    [SerializeField]
    private InputField _usrnInput;
    [SerializeField]
    private InputField _pwInput;
    [SerializeField]
    private ErrorPanel _errorOutput;

    #endregion

    #region Methods

    void Start()
    {

    }

    void Update()
    {

    }

    public void Login()
    {
        string usr = _usrnInput.text;
        string pw = _pwInput.text;

        //
        if (usr == string.Empty || pw == string.Empty)
        {
            _errorOutput.ShowError("Invalid usr/pw.");
            return;
        }

        //
        GameSession.CURRENT.ServerConnection.Login(usr, pw);
    }

    #endregion
}
