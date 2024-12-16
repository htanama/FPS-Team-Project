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
    //[SerializeField] TMP_Text flagCaptureText;
    [SerializeField] GameObject timerGoal;
    [SerializeField] Transform spawnPoint;
    [SerializeField] private int playerLives = 3;

    private GameObject player;
    private GameObject flag;
    private playerController playerScript;
    private flagManager flagScript;

    [Header("      UI      ")]
    [SerializeField] private HealthBars playerHealthBar;
    [SerializeField] private HealthBars enemyHealthBar;
    [SerializeField] private float fillSpeed;
    [SerializeField] private Gradient colorGradient;

    [SerializeField] private GameObject playerDamageScreen;
    [SerializeField] TMP_Text playerLivesText;

    public GameObject Player => player;     //Read-only getter
    public GameObject Flag => flag;
    public playerController PlayerScript => playerScript;
    public flagManager FlagScript => flagScript;
    private bool isPaused;
    
    float timeScaleOrig; //For pausing/resume
                         //int goalCount

    // Properties //
    public HealthBars PlayerHealthBar
    {
        get { return playerHealthBar; }
        set { playerHealthBar = value; }
    }
    public HealthBars EnemyHealthBar
    {
        get { return enemyHealthBar; }
        set { enemyHealthBar = value; }
    }
    public float FillSpeed
    {
        get { return fillSpeed; }
        set { fillSpeed = value; }
    }
    public Gradient ColorGradient
    {
        get { return colorGradient; }
        set { colorGradient = value; }
    }

    public GameObject PlayerDamageScreen
    {
        get { return playerDamageScreen; }
        set { playerDamageScreen = value; }
    }
    public int PlayerLives
    {
        get { return playerLives; }
        set
        {
            playerLives = value;
            UpdateLivesUI();
        }
    }

    public bool IsPaused
    {
        get => isPaused;
        set => isPaused = value;
    }
            
   
    // Unity Methods //
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        playerHealthBar.Initialize(fillSpeed, colorGradient);
        enemyHealthBar.Initialize(fillSpeed, colorGradient);

        flag = GameObject.FindWithTag("Flag");
        flagScript = player.GetComponent<flagManager>();    //Attached flag manager to player
        
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

    // Custom Methods //
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


    public void UpdateCaptures(int amount)      //Game goal
    {
       //show flag captures on UI
        //flagCaptureText.text = amount.ToString("F0");
        
        //win condition
        if(FlagScript.CaptureCount >= 3)
        {
            WinGame();
        }

    }

    public void UpdateLivesUI()
    {
        //code to update lives on the UI
        playerLivesText.text = playerLives.ToString("F0");
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
<<<<<<< Updated upstream
=======
            //playerScript.GetPlayerHealth() = playerScript.OrigHP;  //Refill _HP

            //reset Health
            playerScript.PlayerCurrentHealth = playerScript.PlayerMaxHealth;
>>>>>>> Stashed changes

            //change life counter
            playerLives--;
            UpdateLivesUI();
            playerScript.updatePlayerUI();

            Debug.Log($"Player Respawned. Lives remaining: {playerLives}");

<<<<<<< Updated upstream
            //Update lives shown in the UI
            UpdateLives();
=======
>>>>>>> Stashed changes
        }
        else
        {
            LoseGame();     //Dead: Out of Lives
        }

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
