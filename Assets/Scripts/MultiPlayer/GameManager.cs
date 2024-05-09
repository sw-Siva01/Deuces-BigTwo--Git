using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class GameManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnGameStateChanged))]
    public string gameStateJson;
    public GameState gameState;
    public NetworkMatch networkMatch;
    public List<PlayerManager> playerManagersList = new List<PlayerManager>();
    public RoomInfo roomInfo;
    public static GameManager localInstance;
    Coroutine playerTurnTimerRoutine;

    [Client]
    private void Awake()
    {
        localInstance = this;
    }

    void OnGameStateChanged(string oldStr, string newStr)
    {
        gameState = JsonUtility.FromJson<GameState>(newStr);
        if (isClient)
        {
            UpdateNetworkGame();
        }

    }

    public void AddPlayerManager(PlayerManager pm)
    {
        if (pm.networkMatch.matchId != networkMatch.matchId)
            return;

        if (!playerManagersList.Contains(pm))
            playerManagersList.Add(pm);
    }

    public void RemovePlayerManager(PlayerManager pm)
    {
        if (pm.networkMatch.matchId != networkMatch.matchId)
            return;

        if (playerManagersList.Contains(pm))
            playerManagersList.Remove(pm);
    }

    public PlayerUI GetUI(string playerID)
    {
        PlayerUI assignedUI;
        List<PlayerState> playerList = new List<PlayerState>();
        foreach (PlayerState ps in gameState.players)
        {
            playerList.Add(ps);
        }
        foreach (PlayerState ps in gameState.waitingPlayers)
        {
            playerList.Add(ps);
        }

        assignedUI = GameHUD.instance.playerUIDetails[playerList.FindIndex(x => x.playerData.playerID == playerID)];
        CheckUIDuplication(assignedUI, playerID);
        return assignedUI;
    }

    void CheckUIDuplication(PlayerUI assignedUI, string playerID)
    {
        foreach (PlayerUI ui in GameHUD.instance.playerUIDetails)
        {
            if (ui != assignedUI && ui.playerID == playerID)
            {
                Debug.LogError("Duplicate UI issue handled");
                ui.ClearUI();
            }
        }
    }

    public PlayerUI GetAvailableUI(string _pid)
    {
        if (GameHUD.instance.playerUIDetails.Exists(x => x.playerID == _pid))
            return GameHUD.instance.playerUIDetails.Find(x => x.playerID == _pid);

        PlayerUI availableUI = GameHUD.instance.playerUIDetails[0];
        foreach (PlayerUI playerUI in GameHUD.instance.playerUIDetails)
        {
            if (!playerUI.isInit)
            {
                availableUI = playerUI;
                continue;
            }
        }
        return availableUI;
    }


    //public void LinkPlayerManagerToUI(bool repositionUI = false)
    //{
    //    if (repositionUI)
    //    {
    //        foreach (PlayerUI ui in GameHUD.instance.playerUIDetails)
    //            ui.ClearUI();
    //    }

    //    foreach (PlayerManager player in playerManagersList)
    //    {
    //        player.InitPlayerManager();
    //    }

    //}

    //public void ClearVal()
    //{
    //    //gameStateJson = "";
    //    gameState.players.Clear();
    //    gameState.waitingPlayers.Clear();
    //    gameState.InitNewRoundState();
    //    networkMatch.matchId = string.Empty.ToGuid();
    //}

    [Server]
    void UpdateGameStateToServer()
    {
        //PlayerManager[] pmList = FindObjectsOfType<PlayerManager>();
        //foreach (PlayerManager pm in pmList)
        //    AddPlayerManager(pm);

        foreach (MirrorPlayer mp in roomInfo.players)
        {
            PlayerManager pm = mp.playerManager;
            if (pm != null)
            {
                if (gameState.players.Exists(x => x.playerData.playerID == pm.playerID))
                {
                    pm.myPlayerStateJson = JsonUtility.ToJson(GetPlayerState(pm.playerID));
                }
                else if (gameState.waitingPlayers.Exists(x => x.playerData.playerID == pm.playerID))
                {
                    pm.myPlayerStateJson = JsonUtility.ToJson(GetPlayerState(pm.playerID));
                }
            }
        }
        string newGameStateJson = JsonUtility.ToJson(gameState);
        if (gameStateJson != newGameStateJson)
        {
            gameStateJson = newGameStateJson;
            RemoveCompletedEvents();
            CheckActionRequired();
            //OnGameStateChanged("", gameStateJson);
        }

    }

    [Server]
    public void CheckActionRequired()
    {
        foreach (SWEvent request in requestList)
        {
            request.eventAction.Invoke();
            gameState.completedRequests.Add(request.GetEventID());
        }

        switch (gameState.currentState)
        {
            case 0: //Not enough Players
                AddPlayersToGame(false);
                break;
            case 1:
                AddPlayersToGame(true);
                StopCoroutine("StartGameTimerServer");
                StartCoroutine(nameof(StartGameTimerServer));
                break;
            case 2:
                CheckActivePlayers();
                break;
            case 4:
                StopCoroutine(playerTurnTimerRoutine);
                CancelInvoke("InitNewRound");
                Invoke(nameof(InitNewRound), 3);
                break;

        }
        UpdateGameStateToServer();
    }

    [SerializeField]
    List<SWEvent> requestList = new List<SWEvent>();
    public void AddServerEvent(SWEvent newEvent)
    {
        requestList.Add(newEvent);
        CheckActionRequired();
    }
    public void RemoveServerEvent(string eventID)
    {
        requestList.RemoveAll(x => x.GetEventID() == eventID);
        //requestList.Remove(requestList.Find(x => x.GetEventID() == eventID));
    }

    void RemoveCompletedEvents()
    {
        foreach (string reqID in gameState.completedRequests)
            RemoveServerEvent(reqID);
        //gameState.completedRequests.Clear();
    }

    public void AddPlayersToWaitingList(PlayerState newPlayerState)
    {
        gameState.waitingPlayers.Add(newPlayerState);
    }

    public void AddPlayersToGame(bool isGameStarted)
    {
        if ((gameState.waitingPlayers.Count + gameState.players.Count > 1))
        {
            foreach (PlayerState ps in gameState.waitingPlayers)
            {
                Debug.Log("Player Added : " + ps.playerData.playerName);
                gameState.players.Add(ps);
            }
            gameState.waitingPlayers.Clear();
        }
        if (gameState.players.Count > 1)
        {
            foreach (PlayerState ps in gameState.players)
            {
                ps.currentState = 1; // Updated to playing state from waiting
                if (ps.cardStrength == -1)
                    ps.cardStrength = UnityEngine.Random.Range(1, 100);
            }
            if (!isGameStarted)
            {
                gameState.currentState = 1;
                gameState.gameStartTime = NetworkTime.time;
            }
        }

    }

    void CheckActivePlayers()
    {
        if (gameState.players.Count < 2)
            ShowCards();
    }

    [Client]
    public void UpdateNetworkGame()
    {
        //PlayerManager.localPlayer.myPlayerState = GetPlayerState(PlayerManager.localPlayer.playerID);
        UpdateUI();
        switch (gameState.currentState)
        {
            case 0:
                GameHUD.instance.cardValueTxt.text = GameHUD.instance.winnerTxt.text = "";
                break;

            case 1:
                GameHUD.instance.cardValueTxt.text = GameHUD.instance.winnerTxt.text = "";
                StopCoroutine("StartGameTimerClient");
                StartCoroutine(nameof(StartGameTimerClient));
                break;

            case 2:
                //UpdatePlayerProperties();
                GameHUD.instance.cardValueTxt.text = "My card value: " + GetPlayerState(PlayerManager.localPlayer.playerID).cardStrength;
                CheckMyTurn();
                break;

            case 4:
                ShowWinner();
                break;
        }
    }

    public void UpdateUI()
    {
        foreach (PlayerUI ui in GameHUD.instance.playerUIDetails)
        {
            if (ui.isInit)
            {
                PlayerState tempState = new PlayerState();
                if (gameState.players.Exists(x => x.playerData.playerID == ui.playerID))
                    tempState = gameState.players.Find(x => x.playerData.playerID == ui.playerID);
                else if (gameState.waitingPlayers.Exists(x => x.playerData.playerID == ui.playerID))
                    tempState = gameState.waitingPlayers.Find(x => x.playerData.playerID == ui.playerID);
                else
                    tempState = null;

                if (tempState != null)
                {
                    ui.UpdateUI(tempState);
                }
                else
                {
                    ui.ClearUI();
                }

            }
        }
    }

    IEnumerator StartGameTimerClient()
    {
        int countDown = 10;
        while (gameState.gameStartTime + 10 > NetworkTime.time && gameState.players.Count > 1)
        {
            countDown = (int)(gameState.gameStartTime + 10 - NetworkTime.time);
            GameHUD.instance.gameStartTimer.text = countDown.ToString();
            yield return new WaitForEndOfFrame();
        }
        GameHUD.instance.gameStartTimer.text = "";
        GameHUD.instance.cardValueTxt.text = "My card value: " + PlayerManager.localPlayer.myPlayerState.cardStrength;
        //if (isServer)
        //{
        //    gameState.currentState = 2;
        //    gameState.players[0].SetMyTurn(true);
        //    foreach (PlayerState ps in gameState.players)
        //    {
        //        ps.currentState = 1;
        //    }
        //    UpdateGameStateToServer();
        //}
    }


    IEnumerator StartGameTimerServer()
    {
        while (gameState.gameStartTime + 10 > NetworkTime.time && gameState.players.Count > 1)
        {
            yield return new WaitForEndOfFrame();
        }

        gameState.currentState = 2;
        gameState.players[0].SetMyTurn(true);
        playerTurnTimerRoutine = StartCoroutine(StartPlayerTimer(gameState.players[0].playerData.playerID));
        foreach (PlayerState ps in gameState.players)
        {
            ps.currentState = 1;
        }
        UpdateGameStateToServer();
    }

    void CheckMyTurn()
    {
        //try
        //{
        //    if (GetPlayerState(PlayerManager.localPlayer.playerID).isMyTurn)
        //    {
        //        GameHUD.instance.myTurnUI.SetActive(true);
        //    }
        //    else
        //    {
        //        GameHUD.instance.myTurnUI.SetActive(false);
        //    }
        //}
        //catch { }


        if (gameState.players.Count == 1)
        {
            ShowWinner();
        }
    }

    public void NextPlayerTurn()
    {
        int currentPlayerIndex = GetCurrentPlayingPlayerIndex();
        int nextPlayerIndex = currentPlayerIndex;
        gameState.players[currentPlayerIndex].SetMyTurn(false);
        StopCoroutine(playerTurnTimerRoutine);
        bool isNextPlayerFound = false;

        while (!isNextPlayerFound)
        {
            if (nextPlayerIndex + 1 == gameState.players.Count)
                nextPlayerIndex = 0;
            else
                nextPlayerIndex += 1;

            if (gameState.players[nextPlayerIndex].currentState == 1)
                isNextPlayerFound = true;
        }

        gameState.players[nextPlayerIndex].SetMyTurn(true);
        playerTurnTimerRoutine = StartCoroutine(StartPlayerTimer(gameState.players[nextPlayerIndex].playerData.playerID));
    }

    IEnumerator StartPlayerTimer(string playerID)
    {
        yield return new WaitForSeconds(15f);
        PlayerState tempState = new PlayerState();
        if (gameState.players.Exists(x => x.playerData.playerID == playerID))
            tempState = gameState.players.Find(x => x.playerData.playerID == playerID);
        else if (gameState.waitingPlayers.Exists(x => x.playerData.playerID == playerID))
            tempState = gameState.waitingPlayers.Find(x => x.playerData.playerID == playerID);
        else
            tempState = null;

        if (tempState != null && tempState.myTurnTime != -1)
        {
            if (roomInfo.players.Exists(x => x.playerID == playerID))
            {
                roomInfo.players.Find(x => x.playerID == playerID).playerManager.ServerDisconnect();
                RemoveFromGame(playerID);
            }
            else
            {
                RemoveFromGame(playerID);
            }
        }

    }

    public void ShowCards()
    {
        gameState.currentState = 4;
    }

    public void ShowWinner()
    {
        GameHUD.instance.winnerTxt.text = "The winner is " + GetWinningPlayer().playerName;
        GameHUD.instance.HideHud();
    }

    public void InitNewRound()
    {
        if (isServer)
        {
            gameState.InitNewRoundState();
            UpdateGameStateToServer();
        }
    }

    public void RejoinGame(string playerID)
    {
        Debug.Log("Server rejoin");
        GetPlayerState(playerID).disconnectTime = -1;
        UpdateGameStateToServer();
    }

    public int GetCurrentPlayingPlayerIndex()
    {
        return gameState.players.FindIndex(x => x.isMyTurn == true);
    }

    PlayerData GetWinningPlayer()
    {
        PlayerData winningPlayer = gameState.players[0].playerData;
        int highScore = 0;
        foreach (PlayerState ps in gameState.players)
        {
            if (ps.cardStrength > highScore)
            {
                highScore = ps.cardStrength;
                winningPlayer = ps.playerData;
            }
        }
        return winningPlayer;
    }

    PlayerState GetPlayerState(string playerID)
    {
        PlayerState playerState = new PlayerState();
        foreach (PlayerState ps in gameState.players)
        {
            if (playerID == ps.playerData.playerID)
            {
                playerState = ps;
                return ps;
            }
        }
        foreach (PlayerState ps in gameState.waitingPlayers)
        {
            if (playerID == ps.playerData.playerID)
            {
                playerState = ps;
                return ps;
            }
        }
        return playerState;
    }

    public void RemoveFromGame(string playerID)
    {
        Debug.Log("Player Removed from game: " + playerID);
        if (gameState.players.Contains(GetPlayerState(playerID)))
        {
            if (GetPlayerState(playerID).isMyTurn)
                NextPlayerTurn();
            gameState.players.Remove(GetPlayerState(playerID));
            UpdateGameStateToServer();
        }
        else if (gameState.waitingPlayers.Contains(GetPlayerState(playerID)))
        {
            gameState.waitingPlayers.Remove(GetPlayerState(playerID));
            UpdateGameStateToServer();
        }
    }

    public void SetDisconnectedPlayer(string playerID)
    {
        Debug.LogError("Player Disconnected from game");
        if (gameState.players.Contains(GetPlayerState(playerID)))
        {
            Debug.Log(GetPlayerState(playerID).playerData.playerName + " has been disconnected");
            GetPlayerState(playerID).disconnectTime = NetworkTime.time;
        }
        else if (gameState.waitingPlayers.Contains(GetPlayerState(playerID)))
        {
            Debug.Log(GetPlayerState(playerID).playerData.playerName + " has been removed from Waiting List");
            gameState.waitingPlayers.Remove(GetPlayerState(playerID));
        }
        UpdateGameStateToServer();
    }
}
