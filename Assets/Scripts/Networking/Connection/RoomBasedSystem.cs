using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class GameRoom
{
	#region Vars

	public int RoomID;
	public string Name;
	public int MaxPlayers;
	public int MapID;
	public int CreatorID;
	public List<RoomPlayerInfo> Players;

	#endregion

	#region Construct

	public GameRoom(int roomID, string name, int maxPlayers, int mapID, int creatorID)
	{
		RoomID = roomID;
		Name = name;
		MaxPlayers = maxPlayers;
		MapID = mapID;
		CreatorID = creatorID;
		Players = new List<RoomPlayerInfo>();
	}

	#endregion
}

public class RoomPlayerInfo
{
	#region Vars

	public int PlayerID;
	public string PlayerName;
	public int SpawnPointID;
	public PlayerSetup Setup;
	public EndPoint UdpEP;

	#endregion

	#region Construct

	public RoomPlayerInfo(int id, string name, int spawnPointID, PlayerSetup setup)
	{
		PlayerID = id;
		PlayerName = name;
		SpawnPointID = spawnPointID;
		Setup = setup;
	}

	#endregion
}

public class PlayerSetup
{
	#region Vars

	public int BoatID;
	public int FlagColorID;
	public bool Ready;

	#endregion

	#region Construct

	public PlayerSetup(int boatID, int flagColorID, bool ready)
	{
		BoatID = boatID;
		FlagColorID = flagColorID;
		Ready = ready;
	}

	#endregion
}