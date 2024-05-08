using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class PlayerManager : NetworkBehaviour
{
    public static PlayerManager localPlayer;
    public NetworkMatch networkMatch;
    [SyncVar] public PlayerData myPlayerData;
    [SyncVar] public string playerID;
    [SyncVar] public string roomName;
    [SyncVar] public int playerIndex;

    private void Awake()
    {
        networkMatch = GetComponent<NetworkMatch>();
        if (isLocalPlayer)
            localPlayer = this;
    }

    [Command]
    public void UpdatePlayerData(string RawData)
    {
        PlayerData pd = JsonUtility.FromJson<PlayerData>(RawData);
        myPlayerData = pd;
        playerID = pd.playerID;
        PlayerState ps = new PlayerState();
        ps.playerData = pd;
    }

    public override void OnStartClient()
    {
        if (isLocalPlayer)
            localPlayer = this;
        else
            StartCoroutine(WaitAndCheckJoinStatus(false));
    }

    public override void OnStartLocalPlayer()
    {
        localPlayer = this;
    }

    public void SearchGame()
    {
        cmdSearchGame();
    }

    [Command]
    void cmdSearchGame()
    {
        if (MirrorManager.instance.SearchGame(gameObject, playerID, out roomName))
        {
            Debug.Log($"<color=green>Game Found Successfully</color>");
        }
        else
        {
            TargetSearchGame(false, roomName);
        }
    }

    [TargetRpc]
    void TargetSearchGame(bool success, string _matchID)
    {
        roomName = _matchID;

        if (!success)
        {
            HostGame();
            Debug.Log("GameSearch-Success");
        }
        else
        {
            Debug.Log("GameSearch-Not-Success");
        }
    }

    public void HostGame()
    {
        string _matchID = "Room_" + DateTime.UtcNow;
        cmdHostGame(_matchID);
    }

    [Command]
    void cmdHostGame(string _matchID)
    {
        roomName = _matchID;
        if (MirrorManager.instance.HostGame(gameObject, playerID, _matchID))
        {
            Debug.Log("Game-Hosted");
        }
        else
        {
            TargetHostGame(false, _matchID);
        }
    }

    [TargetRpc]
    void TargetHostGame(bool success, string _matchID)
    {
        roomName = _matchID;
        if (!success)
        {
            roomName = _matchID;
            Debug.Log("Game-Host");
        }
        else
        {
            HostGame();
            Debug.Log("Game-Not-Host");
        }
    }

    public void JoinGame(string _matchID)
    {
        cmdJoinGame(_matchID);
    }

    [Command]
    void cmdJoinGame(string _matchID)
    {
        roomName = _matchID;
        if (MirrorManager.instance.JoinGame(gameObject, playerID, _matchID))
        {

        }
        else
        {
            TargetJoinGame(false, _matchID);
        }
    }

    [TargetRpc]
    void TargetJoinGame(bool success, string _matchID)
    {
        roomName = _matchID;
        if (!success)
        {
            roomName = _matchID;
            Debug.Log("Game-Joined");
        }
        else
        {
            Debug.Log("Game-Not-Joined");
        }
    }

    IEnumerator WaitAndCheckJoinStatus(bool isMine)
    {
        /*while (!gameManager)
        {
            if (GameManager.localInstance)
            {
                gameManager = GameManager.localInstance;
            }
            yield return null;
            //Debug.LogError("GameManager initializing..");
        }
        gameManager.AddPlayerManager(this);

        if (isMine)
            CheckJoinStatus();

        StartCoroutine(WaitForPlayerInGameState());*/

        yield return null;  // Backspace this line while uncommenting the above lines
    }
}
