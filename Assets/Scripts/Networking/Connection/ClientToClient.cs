using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientToClient : IDisposable
{
	#region Events

	#endregion

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
		_connection.ExclusiveAddressUse = false;
		_connection.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

		_receiveBuffer = new byte[_receiveBufferSize];
	}

	#endregion

	#region Methods

	public void Dispose()
	{
		try
		{
			_connection.Shutdown(SocketShutdown.Both);
		}
		catch (Exception) { }
	}

	public void RegisterToServer(EndPoint serverEP, int playerID, byte[] sessionKey)
	{
		_serverEP = serverEP;
		_playerID = playerID;

		BeginReceive();

		byte[] package = PackageFactory.Pack(PackageType.RegisterUDP, new UDPRegisterData(playerID, sessionKey));
		Send(package, _serverEP);
	}

	public void BeginReceive(P2PConnection clientState = null)
	{
		if (clientState == null)
			clientState = new P2PConnection();

		_connection.BeginReceiveFrom(_receiveBuffer, 0, _receiveBufferSize, SocketFlags.None, ref clientState.EP, new AsyncCallback(ReceiveCallBack), clientState);
	}

	private void ReceiveCallBack(IAsyncResult res)
	{
		//if (res.IsCompleted) return;

		int received = _connection.EndReceive(res);
		P2PConnection clientState = (P2PConnection)res.AsyncState;

		try
		{
			int offset = 0;
			RigidData msg = new RigidData(_receiveBuffer, ref offset);
			Debug.Log(string.Format("Received from id {0} with addr {1} pos {2} angle {3} and velo {4}", clientState.ID, clientState.EP, msg.Positioning.Position.Vector, msg.Positioning.Angle.Vector, msg.Velocity.Vector));
		}
		catch (Exception)
		{
			Debug.Log(string.Format("UDP Received {0}b from {1}", received, clientState.EP));
		}

		// Wait for more data from client
		BeginReceive(clientState);

		// Wait for new client
		BeginReceive();
	}

	public void Send(byte[] data, EndPoint ep)
	{
		_connection.BeginSendTo(data, 0, data.Length, SocketFlags.None, ep, new AsyncCallback(SendCallBack), null);

		Debug.Log("UDP sending to " + ep.ToString());
	}

	private void SendCallBack(IAsyncResult res)
	{
		//if (res.IsCompleted) return;

		_connection.EndSendTo(res);
	}

	#endregion
}

public class P2PConnection
{
	public static int ID_COUNT = 0;

	public EndPoint EP;
	public int ID;

	public P2PConnection()
	{
		EP = new IPEndPoint(IPAddress.Any, 0);
		ID = ID_COUNT++;
	}
}