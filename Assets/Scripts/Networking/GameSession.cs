using UnityEngine;
using System.Collections;

public class GameSession : MonoBehaviour
{
    // SingleTon
    public static GameSession CURRENT;
    
    #region Vars

    public ClientToServerConnection ServerConnection;

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

    private void Start()
    {
        //_serverConnection.Connect(System.Net.IPAddress.Parse("127.0.0.1"), 1337);
    }

    #endregion
}
