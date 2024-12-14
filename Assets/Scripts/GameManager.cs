using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public AudioSource aud;

    [Header("Game Menus")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin, menuLose;

    [Header("Goal Settings")]
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] TMP_Text flagCaptureText;
    [SerializeField] GameObject timerGoal;

    private GameObject player;
    private GameObject flag;
    private playerController playerScript;
    private flagManager flagScript;

    public GameObject Player => player;     //Read-only getter
    public GameObject Flag => flag;
    public playerController PlayerScript => playerScript;
    public flagManager FlagScript => flagScript;

    [SerializeField] private Image playerHPBar;
    [SerializeField] private GameObject playerDamageScreen;

    private bool isPaused;

    //getters and setters
    public Image PlayerHPBar
    {
        get => playerHPBar;
        set => playerHPBar = value;
    }

    public GameObject PlayerDamageScreen
    {
        get => playerDamageScreen;
        set => playerDamageScreen = value;
    }

    public bool IsPaused
    {
        get => isPaused;
        set => isPaused = value;
    }

    float timeScaleOrig;    
    int goalCount, flagCount;
    //int numberFlags;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        flag = GameObject.FindWithTag("Flag");
        flagScript = flag.GetComponent<flagManager>();
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

    public void UpdateFlagCount(int amount)
    {
        flagCount += amount;
        flagCaptureText.text = flagCount.ToString("F0");

        if (flagCount >= 3)
        {
            StatePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
        else if (flagCount <= -3)
        {
            StatePause();
            menuActive = menuLose;
            menuActive.SetActive(true);
        }

    }

    public void Respawn()
    {
        //drop flag

        //move player to spawn point (don't destroy)

        //change life counter
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
