using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class Common : MonoBehaviour
{
    public CharacterSelectible charcherSelect;
    public GameObject Passbuttons, betButton;

    public GameObject[] TimerObjs;

    [SerializeField] GameObject bet_Panel,setting_Panel_Pos, setting_Panel_PosBack, setting_Panel, Settings, InternetPanel;

    public static bool isTime = false;

    public static bool isSet = false;

    public static bool isON = false;


    public float TotalMoney;

    public TextMeshProUGUI TotalAmount;

    public static Common instance;


    private void Awake()
    {
        instance = this;

        // { Save the Total Money Value }
        TotalMoney = PlayerPrefs.GetFloat("TotalMoneyValue", 250.00f);
        // { END }
    }

    private void Start()
    {
        betButton.SetActive(false); bet_Panel.SetActive(false);

        Invoke("DelayedTime", 2f);

        StartCoroutine(SetButtonOFF(2f));

        TotalAmount.SetText(TotalMoney.ToString("0.00"));
    }

    private void Update()
    {
        // { Save the Total Money Value }
        PlayerPrefs.SetFloat("TotalMoneyValue", TotalMoney);
        // { END }

        TotalAmount.SetText(TotalMoney.ToString("0.00"));
    }

    void DelayedTime()
    {
        /*charcherSelect.OnClick();*/
        betButton.SetActive(true);
        bet_Panel.SetActive(true);
    }

    public void BetButton()     // Fixed betValue of 10usd and start the Game
    {
        TotalMoney -= 10f;
        charcherSelect.OnClick();
        betButton.SetActive(false);
    }


    IEnumerator SetButtonOFF(float delay)
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            if (isSet)
            {
                Passbuttons.gameObject.SetActive(false);
            }
            else
            {
                Passbuttons.gameObject.SetActive(true);
            }

            if (isON)
            {
                for (int i = 0; i < TimerObjs.Length; i++)
                {
                   TimerObjs[i].SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < TimerObjs.Length; i++)
                {
                    TimerObjs[i].SetActive(true);
                }
            }
            yield return null;
        }
    }


    public void Setting_panel()
    {
        setting_Panel.transform.DOMove(setting_Panel_Pos.transform.position, 0.3f);
    }
    public void Setting_panel_Back()
    {
        setting_Panel.transform.DOMove(setting_Panel_PosBack.transform.position, 0.3f);
    }

    public void Sound_ON()
    {
        AudioListener.volume = 1;
    }
    public void Sound_OFF()
    {
        AudioListener.volume = 0;
    }
}