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
    [SerializeField] TMP_Text orbCaptureText;
    [SerializeField] GameObject timerGoal;
    [SerializeField] Transform spawnPoint;
    [SerializeField] int playerLives = 3;
    public Image playerHpBar; // from lecture

    public Image playerHpBar;
    private GameObject player;
    private playerController playerScript;
    private List<orbManager> orbScripts;

    public GameObject Player => player;     //Read-only getter
    public playerController PlayerScript => playerScript;
    public List<orbManager> OrbScripts => orbScripts;

    [SerializeField] public GameObject playerDamageScreen;      // *****
    [SerializeField] TMP_Text playerLivesText;

    private bool isPaused;

    // Properties //
    //public GameObject PlayerDamageScreen
    //{
    //    get { return playerDamageScreen; }
    //    set { playerDamageScreen = value; }
    //}
    //public int PlayerLives
    //{
    //    get { return playerLives; }
    //    set
    //    {
    //        playerLives = value;
    //        UpdateLivesUI();
    //    }
    //}

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
        //find and set player reference
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        //find all orbs with orbManager scripts in the scene
        orbScripts = new List<orbManager>(FindObjectsOfType<orbManager>());
        if (orbScripts.Count == 0)
        {
            Debug.LogWarning("No orbManager instances found! Ensure orbs are placed in the scene.");
        }
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

    public void UpdateOrbsCollected(int amount)      //Game goal
    {
       //show orb captures on UI
        orbCaptureText.text = amount.ToString("F0");
        
        //win condition
        if(FindObjectsOfType<orbManager>().Length <= amount)        //auto updates based on number of orbs in level
        {
            WinGame();              //Change to reach the end point rather than collect the orbs
        }                           //or increase to final number to win the game

    }
    public void UpdateLivesUI()
    {
        //code to update lives on the UI
        playerLivesText.text = playerLives.ToString("F0");
    }

    public void Respawn()       //called when player health reaches zero
    {
        //respawn while player has lives
        if (playerLives > 0)
        {
            //drop all orbs held
            foreach (orbManager orb in orbScripts)
            {
                if (orb.IsHoldingOrb)
                {
                    orb.DropOrb(player.transform);
                }
            }

            //move player to spawn point (don't destroy)
            CharacterController controller = player.GetComponent<CharacterController>();
            //Moves the player the distance needed to be back at spawn
            controller.Move(spawnPoint.position - player.transform.position);

            //reset Health
            playerScript.PlayerCurrentHealth = playerScript.PlayerMaxHealth;

            //change life counter
            playerLives--;

            //UpdateLivesUI();          *****
            
            //Update lives and health shown in the UI
            playerScript.updatePlayerUI();

            Debug.Log($"Player Respawned. Lives remaining: {playerLives}");            
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
