using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
[CreateAssetMenu(menuName = "PluggableAI/Misc/gameManager")]
public class GameManager : ScriptableObject
{
    [SerializeField]
    public Transform player;
    public GameObject playerPrefab;

   // public CameraFolloLogic cameraFollowTarget;
  //  public CinemachineFreeLook freeLook;


    public bool paused;
    public void pauseGame()
    {
        paused = true;
        Time.timeScale = 0;
    }
    public void unPauseGame()
    {
        paused = false;
        Time.timeScale = 1;
    }

    public void loadNextLevel()
    {
       /* if (currentLevel ==2)
            loadLevel(2);
        else
            loadLevel(0);
        currentLevel++;*/
    }
    public void testLoad(int currentLevel)
    {
        if (currentLevel == 3)
            loadLevel(3);
        else
            loadLevel(1);

     //   this.currentLevel = currentLevel;
    }
    public void loadLevel(int i)
    {

        unPauseGame();
      //  currentRoom = null;
        SceneManager.LoadScene(i);
    }
}
