using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Common : MonoBehaviour
{
    public CharacterSelectible charcherSelect;
    public GameObject Passbuttons;

    public GameObject[] TimerObjs;

    [SerializeField] GameObject setting_Panel_Pos, setting_Panel_PosBack, setting_Panel, Settings, InternetPanel;

    private readonly IPlayerInteractionController _playerInteractionController;

    public static bool isTime = false;

    public static bool isSet = false;

    public static bool isON = false;

    public static Common instance;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Invoke("DelayedTime", 2f);

        StartCoroutine(SetButtonOFF(2f));
    }

    void DelayedTime()
    {
        charcherSelect.OnClick();
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