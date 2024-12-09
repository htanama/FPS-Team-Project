// Edited - implemented HP Bar

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public GameObject player;
    public playerController playerScript;


    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin, menuLose;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] GameObject timerGoal;
    public Image playerHPBar;

    public GameObject playerDamageScreen;

    public bool isPaused;

    float timeScaleOrig;    
    int goalCount;
    //int numberFlags;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        //playerScript = player.GetComponent<playerController>();

        //goalCount = playerScript.GetHP();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                StatePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                StateUnPause();
            }
        }
    }

    public void StatePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void StateUnPause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void UpdateGame(int amount)
    {
        goalCount += amount;
        goalCountText.text = goalCount.ToString("F0");

        if (goalCount <= 0)// character will respawn when HP hits zero
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void LoseGame()
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    
    // Times is up-- you Lose
    void TimeUp()
    {
        // Handle what happens when the timer reaches zero
        StatePause();
        menuActive = menuLose; // Show the lose menu when time is up
        menuActive.SetActive(true);
    }

    void HowManyFlagsCapture(int amount)
    {
        //numberFlags += amount;
    }


}
