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

	public void RegisterToServer(EndPoint serverEP, int playerID, byte[] sessionKey)
	{
		_serverEP = serverEP;
		_playerID = playerID;
		
		SendMessage(PackageType.RegisterUDP, new UDPRegisterData(playerID, sessionKey), _serverEP);

		BeginReceive();
	}

	public void BeginReceive()
	{
		//IPEndPoint ipep = new IPEndPoint(IPAddress.Any, 0);
		//_receiveEP = (EndPoint)ipep;
		_receiveEP = _connection.LocalEndPoint;
		Debug.Log("UDP begin receive at " + _receiveEP.ToString());
		_connection.BeginReceiveFrom(_receiveBuffer, 0, _receiveBufferSize, SocketFlags.None, ref _receiveEP, new AsyncCallback(ReceiveCallBack), _receiveEP);
	}

	private void ReceiveCallBack(IAsyncResult res)
	{
		//if (res.IsCompleted) return;

		int received = _connection.EndReceiveFrom(res, ref _receiveEP);
		EndPoint ep = _receiveEP;

		try
		{
			int offset = 0;
			RigidData msg = new RigidData(_receiveBuffer, ref offset);
			Debug.Log(string.Format("UDP Received from addr {1} pos {2} angle {3} and velo {4}", ep, msg.Positioning.Position.Vector, msg.Positioning.Angle.Vector, msg.Velocity.Vector));
		}
		catch (Exception)
		{
			Debug.Log(string.Format("UDP Received {0}b from {1}", received, ep));
		}

		// Wait for next msg
		BeginReceive();
	}

	public void Send(byte[] data, EndPoint ep)
	{
		_connection.BeginSendTo(data, 0, data.Length, SocketFlags.None, ep, new AsyncCallback(SendCallBack), null);

		Debug.Log("UDP sending to " + ep.ToString());
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