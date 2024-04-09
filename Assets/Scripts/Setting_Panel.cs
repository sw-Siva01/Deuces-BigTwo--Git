using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting_Panel : MonoBehaviour
{
    public void OnFullScreenBtnClick()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        /*APIController.FullScreen();*/
#endif
        // HideSettings();
    }
    public void ExitWebGL()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        /*APIController.CloseWindow();*/
#elif UNITY_EDITOR
        // EditorApplication.isPlaying = false;
#endif
    }
}
