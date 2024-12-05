using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameMang : MonoBehaviour
{ public static GameMang Instance; 

    public enum GameState {MainMenu, Playing, Paused, Win ,Lose };
    public GameState CurrentState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { 
          Destroy(gameObject);
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        switch (newState) {

            case GameState.MainMenu:
                Debug.Log("Game in the Main Menu");
                break;

            case GameState.Playing:
                Time.timeScale = 1f;
                Debug.Log("Game Started");
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                Debug.Log("Game Paused");
                break;

            case GameState.Win:
                Debug.Log("You Won!!");
                break;

            case GameState.Lose:
                Debug.Log("You lost!");
                break;


        }


    }
