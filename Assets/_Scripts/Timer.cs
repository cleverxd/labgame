using TMPro;
using UnityEngine;
using UnityEngine.UI; // Make sure to include this if you're using UI elements

public class Timer : MonoBehaviour
{
    public TMP_Text timerText; // Reference to a UI Text component to display the timer
    private float timer;    // Time in seconds
    private bool isRunning; // Indicates whether the timer is running

    private void Start()
    {
        timer = 0f;         // Initialize the timer
        isRunning = true;   // Start the timer
    }

    private void Update()
    {
        if (isRunning)
        {
            timer += Time.deltaTime; // Increment the timer by the time since last frame
            UpdateTimerText();        // Update the displayed timer text
        }
    }

    private void UpdateTimerText()
    {
        // Calculate hours, minutes, and seconds
        int hours = Mathf.FloorToInt(timer / 3600);
        int minutes = Mathf.FloorToInt((timer % 3600) / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        // Format the time as HH:MM:SS and update the UI text
        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }

    // Call this method to stop the timer when entering the trigger
    public void StopTimer()
    {
        isRunning = false; // Stop the timer
    }
}
