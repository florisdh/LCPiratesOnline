using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class ConnectionMenu : Menu
{
    #region Vars

    [SerializeField]
    private Menu _loginMenu;

    [SerializeField]
    private Text _statusIndicator;
    private string _newStatus;
    private string _status;

    private bool _succeed = false;

    private GameSession _session;
    
    #endregion

    #region Methods

    private void Start()
    {
        _session = GameSession.CURRENT;
        _session.ServerConnection.Connected += ServerConnection_Connected;
        _session.ServerConnection.ConnectFailed += ServerConnection_ConnectFailed;
        _session.ServerConnection.ConnectionSecured += ServerConnection_ConnectionSecured;
        _session.ServerConnection.Connect(ClientToServerConnection.DEFAULT_IP, ClientToServerConnection.DEFAULT_PORT);
    }

    private void FixedUpdate()
    {
        if (_succeed)
        {
            _loginMenu.Show(this);
            return;
        }
        if (_newStatus != _status)
        {
            _statusIndicator.text = _status = _newStatus;
        }
    }

    private void UpdateStatus(string newStatus)
    {
        _newStatus = newStatus;
    }

    private void ServerConnection_ConnectionSecured(object sender, EventArgs e)
    {
        _succeed = true;
    }

    private void ServerConnection_ConnectFailed(object sender, EventArgs e)
    {
        UpdateStatus("Failed to connect.");
    }

    private void ServerConnection_Connected(object sender, EventArgs e)
    {
        UpdateStatus("Securing connection..");
    }

    #endregion
}
