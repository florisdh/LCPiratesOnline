using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public enum PackageType
{
    // General Packages
    Error = 0,

    // Client Packages
    RequestSecureConnection = 8,
    LoginAttempt = 9,
    CreateRoom = 10,

    // Server Packages
    SetupSecureConnection = 64,
    LoginSucceed = 65,
    LoginFailed = 66,
    RoomCreated = 67
}

#region BasePackages

public interface PackageData
{
    #region Vars

    #endregion

    #region Methods

    byte[] ToBytes();

    void FromBytes(byte[] data, int offset = 0);

    #endregion
}

public class PackageFactory
{
    public static byte[] Pack(PackageType type, PackageData data)
    {
        List<byte> total = new List<byte>();
        total.Add((byte)type);
        total.AddRange(data.ToBytes());
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

    public void FromBytes(byte[] data, int offset = 0)
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

    public void FromBytes(byte[] data, int offset = 0)
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

    public void FromBytes(byte[] data, int offset = 0)
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

    public SecuritySetupData(byte[] data, int offset = 0)
    {
        FromBytes(data, offset);
    }

    #endregion

    #region Methods

    public byte[] ToBytes()
    {
        return null;
    }

    public void FromBytes(byte[] data, int offset = 0)
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

    #endregion

    #region Construct

    public LoginSucceedData(byte[] data, int offset = 0)
    {
        FromBytes(data, offset);
    }

    #endregion

    #region Methods

    public byte[] ToBytes()
    {
        return null;
    }

    public void FromBytes(byte[] data, int offset = 0)
    {
        PlayerID = BitConverter.ToInt32(data, offset);
    }

    #endregion
}

#endregion