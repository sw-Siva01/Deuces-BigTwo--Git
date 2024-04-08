using UnityEngine;
using UnityEngine.UI;

public class playerTimer : MonoBehaviour
{ 
    public float totalTime = 15f;  // Total time for the timer
    private float timeRemaining;    // Time remaining for the timer
    public Image timerBar;          // Reference to the UI Image representing the timer

    public static playerTimer instance;

    public ButtonInteractableCollection buttonInteractableCollection;


    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        timeRemaining = totalTime;
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime; // Decrease time remaining
            UpdateTimerUI(); // Update the UI
        }
        else
        {
            // Timer has reached zero
            Common.isTime = true;
            buttonInteractableCollection.OnTimerComplete?.Invoke();


        }
    }

    void UpdateTimerUI()
    {
        float fillAmount = timeRemaining / totalTime;
        timerBar.fillAmount = fillAmount;
    }

    private void OnDisable()
    {
        timeRemaining = totalTime;
        timerBar.fillAmount = 1;
        Common.isTime = false;
    }
}
