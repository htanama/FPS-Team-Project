
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    playerController player;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin, menuLose;
    [SerializeField] TMP_Text goalCountText;

    public bool isPaused;

    float timeScaleOrig;
    int goalCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = FindObjectOfType<playerController>();
        goalCount = player.GetHP();
        instance = this;
        timeScaleOrig = Time.timeScale;
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

        
        goalCountText.text = goalCount.ToString("F0");
        goalCount -= amount;

        if (goalCount < 0)
        {
            // character will respawn when HP hits zero
        }
    }
}
