using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#nullable enable

// TODO update with appropriate flow later
// This is temporary result view that only opt to restart the game.
public class RestartResultView : MonoBehaviour, IResultView
{
    [SerializeField]
    private GameObject _panel;

    [SerializeField]
    private TextMeshProUGUI _winnerName;

    [SerializeField]
    private Button _restartButton;

    private void Start()
    {
        _restartButton.onClick.AddListener(OnRestartButtonClick);
    }

    private void OnRestartButtonClick()
    {
        SceneManager.LoadScene("Game");
    }

    public void Show(IParticipantInfo participantInfo)
    {
        _winnerName.text = "Winner: " + participantInfo.Name;

        _panel.SetActive(true);

        Common.isON = true;
    }

    private void OnDestroy()
    {
        _restartButton.onClick.RemoveListener(OnRestartButtonClick);
    }
}
