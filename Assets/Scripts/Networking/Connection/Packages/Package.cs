using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Net;
using System;

public enum PackageType
{
    // General Packages
    Error = 0,

    // Client Packages
    RequestSecureConnection = 8,
    LoginAttempt = 9,
	Logout = 10,
	RegisterUDP = 11,
    CreateRoom = 12,
	RequestRooms = 13,
	RequestJoin = 14,
	LeaveRoom = 15,
	SetupChange = 16,
	GameLoaded = 17,
	PlayerUpdate = 18,

    // Server Packages
    SetupSecureConnection = 64,
    LoginSucceed = 65,
    LoginFailed = 66,
	UDPRegistered = 67,
    RoomCreated = 68,
    JoinedRoom = 69,
	RoomList = 70,
	OtherJoinedRoom = 71,
	OtherLeftRoom = 72,
	OtherChangedSetup = 73,
	RoomLoad = 74,
	RoomStart = 75
}

#region BasePackages

public interface PackageData
{
    #region Vars

    #endregion

    #region Methods

    byte[] ToBytes();

    void FromBytes(byte[] data, ref int offset);

    #endregion
}

public class PackageFactory
{
    public static byte[] Pack(PackageType type, PackageData data)
    {
        List<byte> total = new List<byte>();
        total.Add((byte)type);
        if (data != null) total.AddRange(data.ToBytes());
        return total.ToArray();
    }

    public static TypedPackage Unpack(byte[] data)
    {
        return new TypedPackage((PackageType)(int)data[0], data, 1);
    }
}

public struct TypedPackage
{
    public PackageType Type;
	public int Offset;
    public byte[] Data;

    public TypedPackage(PackageType type, byte[] data, int offset = 0)
    {
        Type = type;
        Data = data;
		Offset = offset;
    }
}

#endregion

#region ClientToClient

public class PlayerUpdateData : PackageData
{
	#region Vars

	public PositioningData Positioning;
	public ShootingData Shots;

	#endregion

	#region Construct

	public PlayerUpdateData()
	{
		Positioning = new PositioningData();
		Shots = new ShootingData();
	}

	public PlayerUpdateData(PositioningData positioning)
	{
		Positioning = positioning;
		Shots = new ShootingData();
	}

	public PlayerUpdateData(PositioningData positioning, ShootingData shots)
	{
		Positioning = positioning;
		Shots = shots;
	}

	public PlayerUpdateData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		List<byte> total = new List<byte>();
		total.AddRange(Positioning.ToBytes());
		total.AddRange(Shots.ToBytes());
		return total.ToArray();
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		Positioning = new PositioningData(data, ref offset);
		Shots = new ShootingData(data, ref offset);
	}

	#endregion
}

public class ShootingData : PackageData
{
	#region Vars

	public List<RigidData> Shots;
	
	#endregion

	#region Construct

	public ShootingData()
	{
		Shots = new List<RigidData>();
	}

	public ShootingData(byte[] data, ref int offset)
	{
		Shots = new List<RigidData>();
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		List<byte> total = new List<byte>();
		
		// Add amount
		total.Add((byte)Shots.Count);

		// Add items
		foreach (RigidData shot in Shots)
		{
			total.AddRange(shot.ToBytes());
		}

		return total.ToArray();
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		int total = (int)data[offset++];

		Shots.Clear();
		for (int i = 0; i < total; i++)
		{
			Shots.Add(new RigidData(data, ref offset));
		}
	}

	#endregion
}

public class RigidData : PackageData
{
	#region Vars

	public PositioningData Positioning;
	public Vector3Data Velocity;

	#endregion

	#region Construct

	public RigidData()
	{
		Positioning = new PositioningData();
		Velocity = new Vector3Data();
	}

	public RigidData(Vector3 position, Vector3 angle, Vector3 velo)
	{
		Positioning = new PositioningData(position, angle);
		Velocity = new Vector3Data(velo);
	}

	public RigidData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		List<byte> total = new List<byte>();
		total.AddRange(Positioning.ToBytes());
		total.AddRange(Velocity.ToBytes());
		return total.ToArray();
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		Positioning = new PositioningData(data, ref offset);
		Velocity = new Vector3Data(data, ref offset);
	}

	#endregion
}

public class PositioningData : PackageData
{
	#region Vars

	public Vector3Data Position;
	public Vector3Data Angle;

    #endregion

    #region Construct

	public PositioningData()
	{
		Position = new Vector3Data();
		Angle = new Vector3Data();
	}

	public PositioningData(Vector3 position, Vector3 angle)
	{
		Position = new Vector3Data(position);
		Angle = new Vector3Data(angle);
	}

	public PositioningData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

    #endregion

    #region Methods

    public byte[] ToBytes()
    {
		List<byte> total = new List<byte>();
		total.AddRange(Position.ToBytes());
		total.AddRange(Angle.ToBytes());
		return total.ToArray();
    }

    public void FromBytes(byte[] data, ref int offset)
    {
		Position = new Vector3Data(data, ref offset);
		Angle = new Vector3Data(data, ref offset);
    }

    #endregion
}

public class Vector3Data : PackageData
{
	#region Vars

	public Vector3 Vector;

	#endregion

	#region Construct

	public Vector3Data()
	{
		Vector = Vector3.zero;
	}

	public Vector3Data(Vector3 vector)
	{
		Vector = vector;
	}

	public Vector3Data(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		List<byte> total = new List<byte>();
		total.AddRange(BitConverter.GetBytes(Vector.x));
		total.AddRange(BitConverter.GetBytes(Vector.y));
		total.AddRange(BitConverter.GetBytes(Vector.z));
		return total.ToArray();
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		Vector = Vector3.zero;

		Vector.x = BitConverter.ToSingle(data, offset);
		offset += 4;
		
		Vector.y = BitConverter.ToSingle(data, offset);
		offset += 4;

		Vector.z = BitConverter.ToSingle(data, offset);
		offset += 4;
	}

	#endregion
}

#endregion

#region ClientToServer

public class SecurityRequestData : PackageData
{
    #region Vars

    public byte[] Exponent;
    public byte[] Modulus;

    #endregion

    #region Construct

    public SecurityRequestData(byte[] exponent, byte[] modulus)
    {
        Exponent = exponent;
        Modulus = modulus;
    }

    #endregion

    #region Methods

    public byte[] ToBytes()
    {
        List<byte> total = new List<byte>();
        total.AddRange(Exponent);
        total.AddRange(Modulus);
        return total.ToArray();
    }

    public void FromBytes(byte[] data, ref int offset)
    {
    }

    #endregion
}

public class LoginData : PackageData
{
    #region Vars

    public string UserName;
    public string PassWord;
    
    #endregion

    #region Construct

    public LoginData(string userName, string passWord)
    {
        UserName = userName;
        PassWord = passWord;
    }

    #endregion

    #region Methods

    public byte[] ToBytes()
    {
        List<byte> total = new List<byte>();
        total.Add((byte)UserName.Length);
        total.AddRange(Encoding.UTF8.GetBytes(UserName));
        total.Add((byte)PassWord.Length);
        total.AddRange(Encoding.UTF8.GetBytes(PassWord));
        return total.ToArray();
    }

    public void FromBytes(byte[] data, ref int offset)
    {
    }

    #endregion
}

public class CreateGameData : PackageData
{
    #region Vars

    public string Name;
    public int MaxPlayers;
    public int MapID;

    #endregion

    #region Construct

    public CreateGameData(string name, int maxPlayers, int mapID)
    {
        Name = name;
        MaxPlayers = maxPlayers;
        MapID = mapID;
    }

    #endregion

    #region Methods

    public byte[] ToBytes()
    {
        List<byte> total = new List<byte>();
        total.Add((byte)Name.Length);
        total.AddRange(Encoding.UTF8.GetBytes(Name));
        total.Add((byte)MaxPlayers);
        total.Add((byte)MapID);
        return total.ToArray();
    }

    public void FromBytes(byte[] data, ref int offset)
    {
    }

    #endregion
}

public class JoinRoomData : PackageData
{
	#region Vars

	public int RoomID;

	#endregion

	#region Construct

	public JoinRoomData(int roomID)
	{
		RoomID = roomID;
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		return BitConverter.GetBytes((int)RoomID);
	}

	public void FromBytes(byte[] data, ref int offset)
	{
	}

	#endregion
}

public class UDPRegisterData : PackageData
{
	#region Vars

	public int ClientID;
	public byte[] SessionKey;

	#endregion

	#region Construct

	public UDPRegisterData(int clientID, byte[] sessionKey)
	{
		ClientID = clientID;
		SessionKey = sessionKey;
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		List<byte> total = new List<byte>();
		total.AddRange(new PlayerIDData(ClientID).ToBytes());
		total.AddRange(SessionKey);
		return total.ToArray();
	}

	public void FromBytes(byte[] data, ref int offset)
	{
	}

	#endregion
}

#endregion

#region ServerToClient

public class SecuritySetupData : PackageData
{
    #region Vars

    public RijndaelManaged AES;

    #endregion

    #region Construct

    public SecuritySetupData(byte[] data, ref int offset)
    {
        FromBytes(data, ref offset);
    }

    #endregion

    #region Methods

    public byte[] ToBytes()
    {
        return null;
    }

    public void FromBytes(byte[] data, ref int offset)
    {
        int keyLength = data.Length - offset;
        byte[] key = new byte[keyLength];
        Array.Copy(data, offset, key, 0, keyLength);

        AES = new RijndaelManaged();
        AES.KeySize = keyLength * 8;
        AES.Mode = CipherMode.ECB;
        AES.Padding = PaddingMode.PKCS7;
        AES.Key = key;
    }

    #endregion
}

public class LoginSucceedData : PackageData
{
    #region Vars

    public int PlayerID;
	public byte[] UdpConnectKey;

    #endregion

    #region Construct

    public LoginSucceedData(byte[] data, ref int offset)
    {
        FromBytes(data, ref offset);
    }

    #endregion

    #region Methods

    public byte[] ToBytes()
    {
		return null;
    }

    public void FromBytes(byte[] data, ref int offset)
    {
        PlayerID = new PlayerIDData(data, ref offset).PlayerID;
		
		UdpConnectKey = new byte[8];
		Array.Copy(data, offset, UdpConnectKey, 0, 8);
		offset += 8;
    }

    #endregion
}

public class PlayerIDData : PackageData
{
    #region Vars

    public int PlayerID;

    #endregion

    #region Construct

	public PlayerIDData(int playerID)
	{
		PlayerID = playerID;
	}

    public PlayerIDData(byte[] data, ref int offset)
    {
        FromBytes(data, ref offset);
    }

    #endregion

    #region Methods

    public byte[] ToBytes()
    {
		return BitConverter.GetBytes(PlayerID);
    }

    public void FromBytes(byte[] data, ref int offset)
    {
        PlayerID = BitConverter.ToInt32(data, offset);
		offset += 4;
    }

    #endregion
}

public class JoinedRoomInfoData : PackageData
{
	#region Vars

	public GameRoom Room;

	#endregion

	#region Construct

	public JoinedRoomInfoData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		return null;
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		int roomID = BitConverter.ToInt32(data, offset);
		offset += 4;

		int nameLength = (int)data[offset++];
		string name = Encoding.UTF8.GetString(data, offset, nameLength);
		offset += nameLength;

		int maxPlayers = (int)data[offset++];
		int mapID = (int)data[offset++];
		int creatorID = (int)data[offset++];

		int playerAmount = (int)data[offset++];
		RoomPlayerInfo[] players = new RoomPlayerInfo[playerAmount];
		for (int i = 0; i < playerAmount; i++)
		{
			players[i] = new RoomPlayerData(data, ref offset).Player;
		}

		Room = new GameRoom(roomID, name, maxPlayers, mapID, creatorID);
		Room.Players.AddRange(players);
	}

	#endregion
}

public class RoomInfoData : PackageData
{
	#region Vars

	public GameRoom Room;

	#endregion

	#region Construct

	public RoomInfoData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		return null;
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		int roomID = BitConverter.ToInt32(data, offset);
		offset += 4;

		int nameLength = (int)data[offset++];
		string name = Encoding.UTF8.GetString(data, offset, nameLength);
		offset += nameLength;

		int maxPlayers = (int)data[offset++];
		int mapID = (int)data[offset++];
		int creatorID = (int)data[offset++];
		
		Room = new GameRoom(roomID, name, maxPlayers, mapID, creatorID);
	}

	#endregion
}

public class RoomPlayerData : PackageData
{
	#region Vars

	public RoomPlayerInfo Player;

	#endregion

	#region Construct

	public RoomPlayerData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		return null;
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		int id = (int)data[offset++];

		int nameLength = (int)data[offset++];
		string name = Encoding.UTF8.GetString(data, offset, nameLength);
		offset += nameLength;

		int spawnPointID = (int)data[offset++];

		PlayerSetupData setup = new PlayerSetupData(data, ref offset);
		Player = new RoomPlayerInfo(id, name, spawnPointID, setup.Setup);
	}

	#endregion
}

public class PlayerSetupData : PackageData
{
	#region Vars

	public PlayerSetup Setup;

	#endregion

	#region Construct

	public PlayerSetupData(PlayerSetup setup)
	{
		Setup = setup;
	}

	public PlayerSetupData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		List<byte> data = new List<byte>();
		data.Add((byte)Setup.BoatID);
		data.Add((byte)Setup.FlagColorID);
		data.Add((byte)(Setup.Ready ? 1 : 0));
		return data.ToArray();
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		int boatID = (int)data[offset++];
		int flagID = (int)data[offset++];
		bool ready = data[offset++] == (byte)1;
		Setup = new PlayerSetup(boatID, flagID, ready);
	}

	#endregion
}

public class RoomListData : PackageData
{
	#region Vars

	public GameRoom[] Rooms;

	#endregion

	#region Construct

	public RoomListData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		return null;
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		int roomAmount = (int)data[offset++];

		Rooms = new GameRoom[roomAmount];
		for (int i = 0; i < roomAmount; i++)
		{
			 Rooms[i] = new RoomInfoData(data, ref offset).Room;
		}
	}

	#endregion
}

public class OtherPlayerSetupData : PackageData
{
	#region Vars

	public int PlayerID = -1;
	public PlayerSetup Setup;

	#endregion

	#region Construct

	public OtherPlayerSetupData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		return null;
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		PlayerID = new PlayerIDData(data, ref offset).PlayerID;
		Setup = new PlayerSetupData(data, ref offset).Setup;
	}

	#endregion
}

public class RoomUdpSetupData : PackageData
{
	#region Vars

	public PlayerUdpSetupData[] UdpPlayerList;

	#endregion

	#region Construct

	public RoomUdpSetupData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		return null;
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		int amount = (int)data[offset++];
		UdpPlayerList = new PlayerUdpSetupData[amount];
		for (int i = 0; i < amount; i++)
		{
			UdpPlayerList[i] = new PlayerUdpSetupData(data, ref offset);
		}
	}

	#endregion
}

public class PlayerUdpSetupData : PackageData
{
	#region Vars

	public int PlayerID = -1;
	public IPEndPoint EP;

	#endregion

	#region Construct

	public PlayerUdpSetupData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		return null;
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		PlayerID = new PlayerIDData(data, ref offset).PlayerID;
		EP = new IPEPData(data, ref offset).EP;
	}

	#endregion
}

public class IPEPData : PackageData
{
	#region Vars

	public IPEndPoint EP;

	#endregion

	#region Construct

	public IPEPData(byte[] data, ref int offset)
	{
		FromBytes(data, ref offset);
	}

	#endregion

	#region Methods

	public byte[] ToBytes()
	{
		return null;
	}

	public void FromBytes(byte[] data, ref int offset)
	{
		int ipLength = (int)data[offset++];
		
		string IP = Encoding.UTF8.GetString(data, offset, ipLength);
		offset += ipLength;

		ushort Port = BitConverter.ToUInt16(data, offset);
		offset += 2;

		EP = new IPEndPoint(IPAddress.Parse(IP), (int)Port);
	}

	#endregion
}


#endregion