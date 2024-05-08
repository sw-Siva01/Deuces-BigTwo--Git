using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameState
{
    public List<PlayerState> players = new List<PlayerState>();
    public List<PlayerState> waitingPlayers = new List<PlayerState>();
    public List<string> completedRequests = new List<string>();
    public int currentState = 0;   // 0 - Not Enough Players, 1 - Countdown, 2 - Game InProgress, 3 - Result
    public double gameStartTime = 0;
    public int lastUpdatedPlayerIndex = -1;  // Index of player

    public void InitNewRoundState()
    {
        currentState = 0;
        gameStartTime = 0;
        lastUpdatedPlayerIndex = -1;
        players.RemoveAll(x => x.disconnectTime > 0);
        foreach (PlayerState ps in players)
        {
            ps.ResetState();
        }
        completedRequests.Clear();
    }
}

[System.Serializable]
public class PlayerState
{
    public PlayerData playerData = new PlayerData();
    public int lives = 3;
    public bool isMyTurn = false;
    public int currentState = 0;    // 0 - InWaiting List, 1 - Playing, 2 - Packed, 3 - Side Show, 4 - Show, 5 - Waiting to Join, 6 - Spectate
    public int cardStrength = -1;
    public double myTurnTime = -1;
    public double disconnectTime = -1;

    public void ResetState()
    {
        lives = 3;
        isMyTurn = false;
        currentState = 0;
        cardStrength = -1;
        myTurnTime = -1;
        disconnectTime = -1;
    }

    public void SetMyTurn(bool val)
    {
        isMyTurn = val;
        if (val)
            myTurnTime = NetworkTime.time;
        else
            myTurnTime = -1;
    }
}