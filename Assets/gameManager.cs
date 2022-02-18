using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
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
    #region Time
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
    #endregion

    #region scenes
    public void restartLVL()
    {
        SetNormalTime();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public Animator fadeAnim;
    public void loadScene(int i)
    {
        StartCoroutine(loadSceneDelay(i));
    }
    public IEnumerator loadSceneDelay(int i)
    {
        fadeBlack(true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(i);
    }
    public void fadeBlack(bool bo)
    {
        fadeAnim.SetBool("fadeBlack", bo);
    }

    public GameObject gameOverScreen;
    public void GameOver()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        PauseGame();
    }
    #endregion

    #region Timeline
    //timeline
    //--------------------------------------------------------------

    private PlayableDirector activeDirector;
    public void PauseTimeline(PlayableDirector whichOne)
    {
        gameMode = GameMode.Cutscene; //InputManager will be waiting for a spacebar to resume
        paused = true;
        if (whichOne != null)
        {
            activeDirector = whichOne;
            activeDirector.playableGraph.GetRootPlayable(0).SetSpeed(0);
        }
    }
    //Called by the InputManager
    public void ResumeTimeline()
    {
        if (paused )//&& !DialogueManager.Instance.textIsReading())
        {
            if (activeDirector != null)
            {
                var playable = activeDirector.playableGraph.GetRootPlayable(0);
                playable.SetSpeed(1);
            }
            paused = false;
            //activeDirector.Resume();
        }
        //  else
        // DialogueManager.Instance.SpeedRead();
    }
    public void endTimeLine()
    {
        gameMode = GameMode.Gameplay;
        //DialogueManager.Instance.endDialogue();
        Debug.Log("end");
    }
    #endregion
}
