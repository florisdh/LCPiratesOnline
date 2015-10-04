using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public enum GameModes
{
    FreeForAll = 0,
    TeamDeathMatch = 1
}

[Serializable]
public enum Maps
{
    Lake = 1,
    Islands = 2
}

public class GameSettings
{
    #region Static

    public static GameSettings CURRENT_SESSION;
    public static int MIN_PLAYERS = 2;
    public static int MAX_PLAYERS = 4;
    public static Color[] AvailableColors = new Color[] { Color.black, Color.white, Color.grey, Color.green, Color.blue, Color.yellow, Color.red };

    #endregion

    #region Vars

    public PlayerInfo[] PlayingPlayers;
    public Maps CurrentMap;
    public GameModes GameMode;

    #endregion

    #region Construct

    public GameSettings()
    {
        Init();
    }

    #endregion

    #region Methods

    public static void CREATE_SESSION()
    {
        CURRENT_SESSION = new GameSettings();
    }

    public void Init()
    {
        PlayingPlayers = new PlayerInfo[4];
        for (int i = 0; i < PlayingPlayers.Length; i++)
        {
            PlayerInfo newPlayer = new PlayerInfo();
            newPlayer.IsPlaying = false;
            newPlayer.ID = i + 1;
            newPlayer.SelectedColor = Color.black;
            newPlayer.ShootAxis1 = string.Format("Player{0}Shoot1", newPlayer.ID.ToString());
            newPlayer.ShootAxis2 = string.Format("Player{0}Shoot2", newPlayer.ID.ToString());
            newPlayer.SelectAxis = string.Format("Player{0}Select", newPlayer.ID.ToString());
            newPlayer.BackAxis = string.Format("Player{0}Back", newPlayer.ID.ToString());
            newPlayer.MoveXAxis = string.Format("Player{0}MoveX", newPlayer.ID.ToString());
            newPlayer.MoveYAxis = string.Format("Player{0}MoveY", newPlayer.ID.ToString());
            newPlayer.RotateXAxis = string.Format("Player{0}RotateX", newPlayer.ID.ToString());
            newPlayer.RotateYAxis = string.Format("Player{0}RotateY", newPlayer.ID.ToString());
            newPlayer.DpadXAxis = string.Format("Player{0}DpadX", newPlayer.ID.ToString());
            newPlayer.DpadYAxis = string.Format("Player{0}DpadY", newPlayer.ID.ToString());
            newPlayer.StartAxis = string.Format("Player{0}Start", newPlayer.ID.ToString());
            PlayingPlayers[i] = newPlayer;
        }
    }

    #endregion

}

public class PlayerInfo
{
    public bool IsPlaying;
    public bool IsReady;
    public int ID;
    public Color SelectedColor;
    public int TeamID;

    public string ShootAxis1;
    public string ShootAxis2;
    public string SelectAxis;
    public string BackAxis;
    public string MoveXAxis;
    public string MoveYAxis;
    public string RotateXAxis;
    public string RotateYAxis;
    public string DpadXAxis;
    public string DpadYAxis;
    public string StartAxis;

    public PlayerInfo()
    {
    }
}

