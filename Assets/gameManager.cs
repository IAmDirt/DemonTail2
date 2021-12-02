using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class gameManager : MonoBehaviour
{


    private gameManager()
    {
        // initialize your game manager here. Do not reference to GameObjects here (i.e. GameObject.Find etc.)
        // because the game manager will be created before the objects
    }

    public static gameManager Instance;
 

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
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            restartLVL();
        }
    }

    public void PauseGame()
    {

        StopTime();
    }

    public void unpauseGame()
    {
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
