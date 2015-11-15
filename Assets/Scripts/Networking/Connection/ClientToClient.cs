using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientToClient : IDisposable
{
	#region Events

	public event UDPMessageEvent Received;
	public delegate void UDPMessageEvent(EndPoint ep, TypedPackage package);

	#endregion

	#region Vars

	private Socket _connection;
	private int _playerID;
	private EndPoint _serverEP;
	private byte[] _receiveBuffer;
	private int _receiveBufferSize = 128;
	private EndPoint _receiveEP;

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
			_connection.Close();
		}
		catch (Exception) { }
	}

	public void Bind(EndPoint ep)
	{
		_connection.Bind(ep);
	}

	public void RegisterToServer(EndPoint serverEP, int playerID, byte[] sessionKey)
	{
		_serverEP = serverEP;
		_playerID = playerID;
		
		SendMessage(PackageType.RegisterUDP, new UDPRegisterData(playerID, sessionKey), _serverEP);

		BeginReceive();
	}

	public void BeginReceive()
	{
		_receiveEP = _connection.LocalEndPoint;
		_connection.BeginReceiveFrom(_receiveBuffer, 0, _receiveBufferSize, SocketFlags.None, ref _receiveEP, new AsyncCallback(ReceiveCallBack), _receiveEP);
	}

	private void ReceiveCallBack(IAsyncResult res)
	{
		//if (res.IsCompleted) return;

		int received = _connection.EndReceiveFrom(res, ref _receiveEP);
		EndPoint ep = _receiveEP;

		TypedPackage package = PackageFactory.Unpack(_receiveBuffer);
		if (Received != null)
			Received(ep, package);
		
		// Wait for next msg
		BeginReceive();
	}

	public void Send(byte[] data, EndPoint ep)
	{
		_connection.BeginSendTo(data, 0, data.Length, SocketFlags.None, ep, new AsyncCallback(SendCallBack), null);
	}

	public void SendMessage(PackageType type, PackageData data, EndPoint ep)
	{
		Send(PackageFactory.Pack(type, data), ep);
	}

	private void SendCallBack(IAsyncResult res)
	{
		//if (res.IsCompleted) return;

		_connection.EndSendTo(res);
	}

	#endregion
}