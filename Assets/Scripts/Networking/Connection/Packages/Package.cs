using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System;

public enum PackageType
{
    // General Packages
    Error = 0,

    // Client Packages
    RequestSecureConnection = 8,
    LoginAttempt = 9,
	Logout = 10,
    CreateRoom = 11,
	RequestRooms = 12,
	RequestJoin = 13,
	LeaveRoom = 14,

    // Server Packages
    SetupSecureConnection = 64,
    LoginSucceed = 65,
    LoginFailed = 66,
    RoomCreated = 67,
    JoinedRoom = 68,
	RoomList = 69,
	OtherJoinedRoom = 70,
	OtherLeftRoom = 71
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
        int restLenght = data.Length - 1;
        byte[] rest = new byte[restLenght];
        Array.Copy(data, 1, rest, 0, restLenght);
        return new TypedPackage((PackageType)(int)data[0], rest);
    }
}

public struct TypedPackage
{
    public PackageType Type;
    public byte[] Data;

    public TypedPackage(PackageType type, byte[] data)
    {
        Type = type;
        Data = data;
    }
}

#endregion

#region ClientPackages

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

#endregion

#region ServerPackages

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

public class PlayerIDData : PackageData
{
    #region Vars

    public int PlayerID;

    #endregion

    #region Construct

    public PlayerIDData(byte[] data, ref int offset)
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
        PlayerID = BitConverter.ToInt32(data, offset);
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

	public PlayerSetupData(byte[] data, ref int offset)
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

#endregion