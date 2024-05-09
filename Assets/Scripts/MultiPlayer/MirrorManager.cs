using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;

public class MirrorManager : NetworkBehaviour
{
    public static MirrorManager instance;
    public SyncList<string> roomID = new SyncList<string>();
    public SyncList<RoomInfo> roomList = new SyncList<RoomInfo>();
    public List<string> emptyRoomList = new List<string>();
    NetworkMatch networkMatch;

    [SerializeField] GameObject gameManagerPrefab;

    private void Awake()
    {
        instance = this;
        networkMatch = GetComponent<NetworkMatch>();
    }

    public bool SearchGame(GameObject _playerObj, string _playerID, out string _matchID)
    {
        _matchID = "";

        return false;
    }
    public bool HostGame(GameObject _playerObj, string _playerID, string _matchID)
    {
        if (!roomID.Contains(_matchID))
        {
            GameObject go = Instantiate(gameManagerPrefab);
            NetworkServer.Spawn(go);

            RoomInfo roomInfo = new RoomInfo();
            roomInfo.roomName = _matchID;

            MirrorPlayer newPlayer = new MirrorPlayer();
            newPlayer.playerID = _matchID;
            newPlayer.playerManager = _playerObj.GetComponent<PlayerManager>();
            roomInfo.players.Add(newPlayer);


            roomID.Add(_matchID);
            roomList.Add(roomInfo);

            Debug.Log($"Match generated");
            return true;
        }
        else
        {
            Debug.Log($"Match ID already exists");
            return false;
        }
    }

    public bool JoinGame(GameObject _playerObj, string _playerID, string _matchID)
    {
        if (!roomID.Contains(_matchID))
        {
            MirrorPlayer newPlayer = new MirrorPlayer();
            newPlayer.playerID = _matchID;
            newPlayer.playerManager = _playerObj.GetComponent<PlayerManager>();

            Debug.Log($"Match joined");
            return true;
        }
        else
        {
            Debug.Log($"Match ID does not exist");
            return false;
        }
    }

    public void PlayerDisconnected(PlayerManager player, string _roomName)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].roomName == _roomName)
            {
                roomList[i].players.RemoveAll(x => x.playerManager == player);
                roomList[i].isRoomFull = false;
                if (!isServer) return;

                Debug.Log($"Player disconnected from match {_roomName} | {roomList[i].players.Count} players remaining");

                if (roomList[i].players.Count == 0)
                {
                    Debug.Log($"No more players in Match. Attempting Terminating {_roomName}");
                    StartCoroutine(WaitAndClearMatch(roomList[i]));
                    //matches.RemoveAt (i);
                    //matchIDs.Remove (_matchID);
                }
                break;
            }
        }
    }

    IEnumerator WaitAndClearMatch(RoomInfo room)
    {
        if (!emptyRoomList.Contains(room.roomName))
            emptyRoomList.Add(room.roomName);
        else
            yield break;

        Debug.LogError("Empty room: " + room.roomName);
        yield return new WaitForSeconds(10);
        if (room.players.Count == 0)
        {
            Debug.LogError("Empty room cleared: " + room.roomName);
            roomList.Remove(room);
            roomID.Remove(room.roomName);
            Destroy(room.gameManagerID.gameObject);
        }
        else
            Debug.LogError("Room clear cancelled, Player count: " + room.players.Count);
        emptyRoomList.Remove(room.roomName);
    }
}

public static class MatchExtensions
{
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}
