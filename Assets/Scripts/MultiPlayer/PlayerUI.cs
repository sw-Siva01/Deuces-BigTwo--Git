using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerUI : MonoBehaviour
{
    public Text playerNameTxt, statusTxt;
    public Image timerImg;
    public GameObject profileTransform;

    public string playerID;
    public PlayerState myPlayerState;
    public bool isInit = false;
    bool isMine;
    public void ClearUI()
    {
        playerNameTxt.text = statusTxt.text = string.Empty;
        timerImg.fillAmount = 0;
        profileTransform.SetActive(false);
        isInit = false;
        isMine = false;
    }

    public void InitUI()
    {
        if (isInit)
            return;

        playerID = myPlayerState.playerData.playerID;
        playerNameTxt.text = myPlayerState.playerData.playerName;
        profileTransform.SetActive(true);
        timerImg.fillAmount = 0;

        isInit = true;

        if (playerID == UIController.instance.myPlayerData.playerID)
            isMine = true;
    }

    public void SetState()
    {
        if (myPlayerState.disconnectTime == -1)
            statusTxt.text = "Online";
        else
            statusTxt.text = "Disconnected";

        timerImg.fillAmount = 0;
        if (myPlayerState.isMyTurn)
        {
            StopCoroutine(nameof(StartTimer));
            StartCoroutine(nameof(StartTimer));
            if (isMine && GameManager.localInstance.gameState.currentState == 2)
                GameHUD.instance.ShowHud();
        }
        else
        {
            if (isMine)
                GameHUD.instance.HideHud();
            StopCoroutine(nameof(StartTimer));
        }
    }

    public void UpdateUI(PlayerState ps)
    {
        myPlayerState = ps;
        InitUI();
        SetState();
    }

    IEnumerator StartTimer()
    {
        while (GameManager.localInstance.gameState.currentState == 2 && myPlayerState.myTurnTime + 15 > NetworkTime.time)
        {
            float fillAmount = 1 - (float)((NetworkTime.time - myPlayerState.myTurnTime) / 15f);
            timerImg.fillAmount = fillAmount;
            yield return new WaitForEndOfFrame();
        }

    }
}
