using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Networking;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public GameObject mainMenuPanel;

    public PlayerData myPlayerData = new PlayerData();

    public string randomPlayerName, randomPlayerID;

    public bool isInGame, isAttempToRecon;

    public System.DateTime appClosedTime;

    private void Awake()
    {
        randomPlayerID = Random.Range(10000, 99999).ToString();

        randomPlayerName = "player_ " + Random.Range(10000, 99999).ToString();

        InitializePlayerData();


        if (PlayerPrefs.HasKey("AppCloseTime"))
            appClosedTime = System.DateTime.Parse(PlayerPrefs.GetString("AppCloseTime"));
        else
            ClearPreviousRoom();

        if (PlayerPrefs.HasKey("LastEnteredRoom") && (System.DateTime.UtcNow - appClosedTime).TotalSeconds < 60)
            Debug.Log("ShowPreviousGame");
        else
            ClearPreviousRoom();
    }

    void InitializePlayerData()
    {
        myPlayerData.playerID = randomPlayerID;
        myPlayerData.playerName = randomPlayerName;
    }

    public void StartGame()
    {
        InitializePlayerData();
        PlayerPrefs.SetString("PlayerName", myPlayerData.playerName);
        PlayerPrefs.SetString("PlayerID", myPlayerData.playerID);
        PlayerPrefs.Save();
        StartCoroutine(CheckAndConnectMirrorNetwork());
    }
    public void ShowGameHUD()
    {
        PlayerPrefs.Save();
        isInGame = true;
        mainMenuPanel.SetActive(false);
    }

    IEnumerator InitializeGame()
    {
        if (!NetworkClient.isConnected)
        {
            Debug.Log("Starting Client");
            NetworkManager.singleton.StartClient();
        }
        while (!NetworkClient.isConnected)
        yield return null;

        Debug.Log("Connected to Server");
        if (!PlayerManager.localPlayer)
            NetworkClient.AddPlayer();
        while (!PlayerManager.localPlayer)
            yield return null;

        PlayerManager.localPlayer.UpdatePlayerData(JsonUtility.ToJson(myPlayerData));
        PlayerManager.localPlayer.SearchGame();
    }

    public IEnumerator DisconnectClient()
    {
        isInGame = false;
        //gamehudPanel.SetActive(false);
        ClearPreviousRoom();
        yield return new WaitForSeconds(0.5f);
        NetworkClient.Shutdown();
        mainMenuPanel.SetActive(true);
        isInGame = false;
    }

    public void Reconnect()
    {
        StartCoroutine(CheckAndConnectMirrorNetwork());
    }

    IEnumerator CheckAndConnectMirrorNetwork()
    {
        if (isAttempToRecon)
            yield break;

        isAttempToRecon = true;
        while (Application.internetReachability == NetworkReachability.NotReachable) 
            yield return new WaitForSeconds(0.5f);

        bool isInternet = false;
        while (!isInternet)
        {
            UnityWebRequest www = UnityWebRequest.Get("https://clients3.google.com/generate_204");
            www.timeout = 3;
            yield return www.SendWebRequest();

            if (www.responseCode.ToString() == "204")
                isInternet = true;
        }


        yield return StartCoroutine(InitializeGame());

        isAttempToRecon = false;
    }

    public void ShowRoomClosed()
    {
        ClearPreviousRoom();
    }

    public void ClearPreviousRoom()
    {
        if (PlayerPrefs.HasKey("LastEnteredRoom"))
        {
            PlayerPrefs.DeleteKey("LastEnteredRoom");

        }
    }
}
