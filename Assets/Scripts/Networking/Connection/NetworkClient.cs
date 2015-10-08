using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using UnityEngine;

public class NetworkClient
{
    #region Events

    public event EventHandler Connected;
    public event EventHandler ConnectFailed;
    public event EventHandler Disconnected;
    public event EventHandler ConnectionSecured;

    #endregion

    #region Vars

    private Socket _connection;
    private EndPoint _serverEP;

    private bool _connecting = false;
    private bool _connected = false;
    private bool _connectionSecured = false;

    private byte[] _receiveBuffer;
    private int _receiveBufferSize = 1024;

    // Security
    private RSACryptoServiceProvider _rsa;
    private RijndaelManaged _aes;
    private ICryptoTransform _encrypter, _decrypter;

    #endregion

    #region Construct

    public NetworkClient()
    {
        _receiveBuffer = new byte[_receiveBufferSize];
    }

    #endregion

    #region Methods

    public void Connect(string ip, int port)
    {
        if (_connecting || _connected) return;
        _connecting = true;

        _serverEP = new IPEndPoint(IPAddress.Parse(ip), port);
        _connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _connection.BeginConnect(_serverEP, new AsyncCallback(ConnectCallBack), null);
    }

    public void Disconnect()
    {
        if (_connected)
        {
            _connection.Shutdown(SocketShutdown.Both);
            _connection.Disconnect(false);
            OnDisconnect();
        }
    }

    private void RequestSecureConnection()
    {
        if (!_connected) return;

        _rsa = new RSACryptoServiceProvider(1024);
        RSAParameters keyInfo = _rsa.ExportParameters(false);
        byte[] data = PackageFactory.Pack(PackageType.RequestSecureConnection, new SecurityRequestData(keyInfo.Exponent, keyInfo.Modulus));
        BeginSend(data);
    }

    protected virtual void BeginSend(byte[] data)
    {
        if (!_connected) return;

        // Encrypt
        if (_connectionSecured)
        {
            data = _encrypter.TransformFinalBlock(data, 0, data.Length);
        }

        _connection.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallBack), null);
    }

    private void BeginReceive()
    {
        if (!_connected) return;
        _connection.BeginReceive(_receiveBuffer, 0, _receiveBufferSize, SocketFlags.None, new AsyncCallback(ReceiveCallBack), null);
    }

    protected virtual void HandleMessage(TypedPackage message)
    {
        if (message.Type == PackageType.SetupSecureConnection)
        {
            try
            {
                _aes = new SecuritySetupData(message.Data).AES;
                _encrypter = _aes.CreateEncryptor();
                _decrypter = _aes.CreateDecryptor();
            }
            catch (Exception e)
            {
                Debug.Log("Failed to sync aes: " + e.Message);
                return;
            }
            OnConnectionSecured();
        }
        else if (message.Type == PackageType.Error)
        {
            Debug.Log("Network_ERROR: ");
        }
    }

    private void ConnectCallBack(IAsyncResult res)
    {
        try
        {
            _connection.EndConnect(res);
        }
        catch (Exception)
        {
            OnConnectFailed();
            return;
        }

        OnConnect();

        BeginReceive();
        RequestSecureConnection();
    }

    private void ReceiveCallBack(IAsyncResult res)
    {
        int received = 0;
        try
        {
            received = _connection.EndReceive(res);
        }
        catch (Exception)
        {
            OnDisconnect();
            return;
        }

        if (received == 0)
        {
            OnDisconnect();
            return;
        }

        byte[] receivedBytes = new byte[received];
        Array.Copy(_receiveBuffer, receivedBytes, received);

        // Decrypt
        byte[] plain;
        try
        {
            if (_connectionSecured)
            { // Use AES
                plain = _decrypter.TransformFinalBlock(receivedBytes, 0, received);
            }
            else
            { // Use RSA
            
                plain = _rsa.Decrypt(receivedBytes, true);
            }
        }
        catch (Exception)
        {
            Debug.Log("Failed to decrypt.");
            return;
        }

        HandleMessage(PackageFactory.Unpack(plain));
        BeginReceive();
    }

    private void SendCallBack(IAsyncResult res)
    {
        try
        {
            _connection.EndSend(res);
        }
        catch (Exception)
        {
            OnDisconnect();
        }
    }

    private void OnConnect()
    {
        if (_connected) return;
        _connected = true;
        _connecting = false;

        if (Connected != null) Connected(this, null);
    }

    private void OnConnectFailed()
    {
        if (!_connecting) return;
        _connected = _connecting = false;

        if (ConnectFailed != null) ConnectFailed(this, null);
    }

    private void OnDisconnect()
    {
        if (!_connected) return;
        _connected = _connecting = false;

        if (Disconnected != null) Disconnected(this, null);
    }

    private void OnConnectionSecured()
    {
        if (_connectionSecured) return;
        _connectionSecured = true;

        if (ConnectionSecured != null) ConnectionSecured(this, null);
    }

    #endregion
}
