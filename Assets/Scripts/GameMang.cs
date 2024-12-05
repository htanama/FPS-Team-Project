using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMang : MonoBehaviour
{
    public static GameMang instance;

    public enum GameState { MainMenu, Playing, Paused, Win, Lose }
    public GameState CurrentState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                Debug.Log("Game in Main Menu");
                break;
            case GameState.Playing:
                Debug.Log("Game Started");
                break;
            case GameState.Paused:
                Debug.Log("Game Paused");
                break;
            case GameState.Win:
                Debug.Log("You Won!!");
                break;
            case GameState.Lose:
                Debug.Log("You Lost!");
                break;
        }
    }

    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            ChangeState(GameState.Paused);
        }
    }

    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            ChangeState(GameState.Playing);
        }
    }
}
