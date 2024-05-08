using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[System.Serializable]
public class RoomInfo
{
    public string roomName;
    public bool isClosed = false;
    public bool isRoomFull = false;
    public bool isVisible = true;
    public List<MirrorPlayer> players = new List<MirrorPlayer>();
    public int maxPlayerCount = 3;

    public NetworkIdentity gameManagerID;
}

public enum GameType
{
    PvCom4
}

[System.Serializable]
public class MirrorPlayer
{
    public string playerID;
    public PlayerManager playerManager;
}