using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    public float TimeLeft; // Time in seconds
    public TMP_Text TimerTxt; // Text to display the timer
    public bool TimerOn = false; // Control whether the timer is running
    
    // Start is called before the first frame update
    void Start()
    {
        TimerOn = true; // Starts the timer
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerOn)
        {
            if (TimeLeft > 0)
            {
                TimeLeft -= Time.deltaTime; // Decrease the remaining time
                UpdateTimer(TimeLeft); // Update the timer display
            }
            else
            {
                TimeLeft = 0; // Ensure time doesn't go below 0
                TimerOn = false; // Stop the timer
                TimerTxt.text = "Time's Up!"; // Display end message
                GameManager.instance.LoseGame();
            }
        }
    }

    void UpdateTimer(float currentTime)
    {
        // Ensure no negative time
        currentTime = Mathf.Max(0, currentTime);

        // Calculate minutes and seconds
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        // Update the timer text
        TimerTxt.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Optional: Restart the timer with a new duration
    public void RestartTimer(float newTime)
    {
        TimeLeft = newTime;
        TimerOn = true;
    }
}
