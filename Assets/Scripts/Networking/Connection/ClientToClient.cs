using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientToClient
{
	#region Vars

	private Socket _connection;
	private int _playerID;
	private EndPoint _serverEP;
	private byte[] _receiveBuffer;
	private int _receiveBufferSize = 128;

	#endregion

	#region Construct

	public ClientToClient()
	{
		_connection = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		_receiveBuffer = new byte[_receiveBufferSize];
		BeginReceive();
	}

	#endregion

	#region Methods

	public void RegisterToServer(EndPoint serverEP, int playerID, byte[] sessionKey)
	{
		_serverEP = serverEP;
		_playerID = playerID;

		Debug.Log("Reigster to server: " + serverEP.ToString() + " " + _playerID.ToString() + " " + sessionKey.Length.ToString());

		byte[] package = PackageFactory.Pack(PackageType.RegisterUDP, new UDPRegisterData(playerID, sessionKey));

		_connection.SendTo(package, serverEP);
	}

	private void BeginReceive()
	{
		EndPoint receivedEP = null;
		_connection.BeginReceiveFrom(_receiveBuffer, 0, _receiveBufferSize, SocketFlags.None, ref receivedEP, new AsyncCallback(ReceiveCallBack), receivedEP);
	}

	private void ReceiveCallBack(IAsyncResult res)
	{
		int received = _connection.EndReceive(res);
		EndPoint receivedEP = (EndPoint)res.AsyncState;

		Console.WriteLine(string.Format("Received {0}b from {1}", received.ToString(), receivedEP.ToString()));
	}

	#endregion
}