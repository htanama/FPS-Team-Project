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
    [SerializeField] TMP_Text playerLivesText;
    [SerializeField] TMP_Text flagCaptureText;
    [SerializeField] GameObject timerGoal;
    [SerializeField] Transform spawnPoint;
    [SerializeField] int playerLives = 3;

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

    float timeScaleOrig;    //For pausing/resume
    //int goalCount

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        flag = GameObject.FindWithTag("Flag");
        flagScript = player.GetComponent<flagManager>();    //Attached flag manager to player
        //find flag goal locations
        flagScript.FlagStartBase = GameObject.FindWithTag("FlagBase").transform;
        flagScript.FlagGoalBase = GameObject.FindWithTag("FlagGoal").transform;
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

    public void UpdateCaptures(int amount)      //Game goal
    {
       //show flag captures on UI
        flagCaptureText.text = amount.ToString("F0");
        
        //win condition
        if(FlagScript.CaptureCount >= 3)
        {
            WinGame();
        }

    }

    public void Respawn()
    {
        //respawn while player has lives
        if (playerLives > 0)
        {
            //drop flag
            if (FlagScript.IsHoldingFlag)
            {
                FlagScript.DropFlag(player.transform);
            }

            //move player to spawn point (don't destroy)
            CharacterController controller = player.GetComponent<CharacterController>();
            //Moves the player the distance needed to be back at spawn
            controller.Move(spawnPoint.position - player.transform.position);
            playerScript.HP = playerScript.OrigHP;  //Refill _HP

            //change life counter
            playerLives--;

            Debug.Log($"Player Respawned. Lives remaining: {playerLives}");

            //Update lives and health shown in the UI
            playerScript.updatePlayerUI();
        }
        else
        {
            LoseGame();     //Dead: Out of Lives
        }

    }

    public void UpdateLives()
    {
        //code to update lives on the UI
        playerLivesText.text = playerLives.ToString("F0");
    }

    public void WinGame()       //Win menu
    {
        StatePause();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }
    public void LoseGame()      //Lose menu
    {
        StatePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    
    // Times is up-- you Lose
    //void TimeUp()
    //{
    //    // Handle what happens when the timer reaches zero
    //    StatePause();
    //    menuActive = menuLose; // Show the lose menu when time is up
    //    menuActive.SetActive(true);
    //}
}
