using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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
}
