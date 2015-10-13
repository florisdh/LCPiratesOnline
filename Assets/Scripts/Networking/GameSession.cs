using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameSession : MonoBehaviour
{
    // SingleTon
    public static GameSession CURRENT;
    
    #region Vars

    public ClientToServerConnection ServerConnection;
	public GameRoom CurrentRoom;

    #endregion

    #region Methods

    private void Awake()
    {
        CURRENT = this;
        ServerConnection = new ClientToServerConnection();
    }

    private void OnApplicationQuit()
    {
        ServerConnection.Disconnect();
    }

    #endregion
}