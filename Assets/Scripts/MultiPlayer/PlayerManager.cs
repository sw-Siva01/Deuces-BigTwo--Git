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
    [SyncVar(hook = nameof(OnPlayerStateChanged))] public string myPlayerStateJson;
    public PlayerState myPlayerState = new PlayerState();
    [SyncVar] public string playerID;
    [SyncVar] public string roomName;
    [SyncVar] public int playerIndex;

    public PlayerUI myUI;

    //[SerializeField] GameObject gameManagerPrefab;
    public NetworkIdentity gameManagerNetID;
    public GameManager gameManager;

    private void Awake()
    {
        networkMatch = GetComponent<NetworkMatch>();
        if (isLocalPlayer)
            localPlayer = this;
    }

    private void OnDestroy()
    {
        Debug.Log("PlayerManager destroyed");
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

    void OnPlayerStateChanged(string oldStr, string newStr)
    {
        myPlayerState = JsonUtility.FromJson<PlayerState>(newStr);
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
        UpdatePlayerData(JsonUtility.ToJson(UIController.instance.myPlayerData));
    }

    public override void OnStopClient()
    {
        Debug.Log("Client Stopped: " + playerID);
        if (isLocalPlayer && UIController.instance.isInGame)
            UIController.instance.Reconnect();
        ClientDisconnect();
    }

    public override void OnStopServer()
    {
        if (gameManager && !isDuplicate)
            gameManager.SetDisconnectedPlayer(playerID);
        Debug.Log($"Client Stopped on Server");
        ServerDisconnect();
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

    public void LeaveGameCommand()
    {
        CmdDisconnectGame();
    }

    void CmdDisconnectGame()
    {
        ServerDisconnect();
        gameManager.RemovePlayerManager(this);
        gameManager.RemoveFromGame(playerID);
    }

    public void ServerDisconnect()
    {
        StopAllCoroutines();
        MirrorManager.instance.PlayerDisconnected(this, roomName);
        RpcDisconnectGame();
        networkMatch.matchId = string.Empty.ToGuid();
        roomName = "";
    }

    bool isDuplicate = false;
    public void ServerKick()
    {
        StopAllCoroutines();
        MirrorManager.instance.PlayerDisconnected(this, roomName);
        networkMatch.matchId = string.Empty.ToGuid();
        roomName = "";
        isDuplicate = true;
        RpcKickedOutGame();
    }

    [ClientRpc]
    void RpcKickedOutGame()
    {
        if (isLocalPlayer)
        {
            Debug.LogError("Kicked out. Same ID player joined");
            GameHUD.instance.ClearAllUI();
            StartCoroutine(UIController.instance.DisconnectClient());
        }
        else
            ClientDisconnect();
    }

    [ClientRpc]
    void RpcDisconnectGame()
    {
        if (isLocalPlayer)
        {
            GameHUD.instance.ClearAllUI();
            StartCoroutine(UIController.instance.DisconnectClient());
            //UIController.instance.ShowMainMenu();
        }
        ClientDisconnect();
    }

    void ClientDisconnect()
    {
        if (gameManager)
            gameManager.RemovePlayerManager(this);
    }

    IEnumerator WaitAndCheckJoinStatus(bool isMine)
    {
        while (!gameManager)
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

        StartCoroutine(WaitForPlayerInGameState());
    }

    public IEnumerator WaitForPlayerInGameState()
    {
        bool isAddedToGameState = false;
        while (!isAddedToGameState)
        {

            if (gameManager.gameState.players.Exists(x => x.playerData.playerID == playerID))
                isAddedToGameState = true;
            else if (gameManager.gameState.waitingPlayers.Exists(x => x.playerData.playerID == playerID))
                isAddedToGameState = true;
            else
                yield return new WaitForSeconds(0.2f);
        }

        InitPlayerManager();
    }

    public void CheckJoinStatus()
    {
        if (!isLocalPlayer) return;

        foreach (PlayerState playerState in gameManager.gameState.players)
        {
            if (playerState.playerData.playerID == myPlayerState.playerData.playerID)
            {
                myPlayerState = playerState;
                Debug.Log("Rejoining");
                CmdRejoinGame(playerID);
                return;
            }
        }

        foreach (PlayerState playerState in gameManager.gameState.waitingPlayers)
        {
            if (playerState.playerData.playerID == myPlayerState.playerData.playerID)
            {
                myPlayerState = playerState;
                CmdRejoinGame(playerID);
                return;
            }
        }

        myPlayerState.disconnectTime = -1;
        CmdAddPlayersToWaitingList(myPlayerData.playerID, NetworkTime.time, JsonUtility.ToJson(myPlayerState));
    }

    [Command]
    void CmdRejoinGame(string playerID)
    {
        gameManager.RejoinGame(playerID);
    }


    public void InitPlayerManager()
    {
        myUI = gameManager.GetAvailableUI(playerID);
        myUI.UpdateUI(myPlayerState);
    }

    public void NextPlayerTurnCommand()
    {
        CmdNextPlayerTurn(playerID, NetworkTime.time);
    }

    public void ShowCardsCommand()
    {
        CmdShowCards(playerID, NetworkTime.time);
    }


    [Command]
    void CmdAddPlayersToWaitingList(string userID, double serverTime, string newPlayerDetails)
    {
        PlayerState newPlayerState = JsonUtility.FromJson<PlayerState>(newPlayerDetails);
        SWEvent request = new SWEvent();
        request.playerID = userID;
        request.reqTime = serverTime;
        request.eventAction = new Action(() => gameManager.AddPlayersToWaitingList(newPlayerState));
        gameManager.AddServerEvent(request);
    }

    [Command]
    void CmdNextPlayerTurn(string userID, double serverTime)
    {
        SWEvent request = new SWEvent();
        request.playerID = userID;
        request.reqTime = serverTime;
        request.eventAction = new Action(() => gameManager.NextPlayerTurn());
        gameManager.AddServerEvent(request);
    }

    [Command]
    void CmdShowCards(string userID, double serverTime)
    {
        SWEvent request = new SWEvent();
        request.playerID = userID;
        request.reqTime = serverTime;
        request.eventAction = new Action(() => gameManager.ShowCards());
        gameManager.AddServerEvent(request);
    }
}
