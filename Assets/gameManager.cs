using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class gameManager : MonoBehaviour
{


    public static gameManager Instance;
    private gameManager()
    {
        // initialize your game manager here. Do not reference to GameObjects here (i.e. GameObject.Find etc.)
        // because the game manager will be created before the objects
    }
    public GameMode gameMode = GameMode.Gameplay;
    public enum GameMode
    {
        Gameplay,
        Cutscene,
        DialogueMoment, //waiting for input
    }

    public void startDialogue()
    {
        gameMode = GameMode.Cutscene;
    }
    public void returnToGameplay()
    {
        gameMode = GameMode.Gameplay;
    }
    public bool isInGamePlay()
    {
        return (gameMode == GameMode.Gameplay);
    }

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private bool paused;
    public void pause()
    {
        if (paused)
            unpauseGame();
        else
            PauseGame();
    }
    void PauseGame()
    {
        paused = true;
        StopTime();
    }

    void unpauseGame()
    {
        paused = false;
        SetNormalTime();
    }

    public void StopTime()
    {
        Time.timeScale = 0;
    }

    public void SetNormalTime()
    {
        Time.timeScale = 1;
    }

    public void restartLVL()
    {
        SetNormalTime();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    public GameObject gameOverScreen;
    public void GameOver()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        PauseGame();
    }
}
