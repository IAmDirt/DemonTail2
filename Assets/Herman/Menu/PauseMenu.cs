using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Sett den på som menyen er")]
    public int level = 0;
    public void MainMenu()
    {
        SceneManager.LoadScene(level);
    }

    public void Restart()
    {
        gameManager.Instance.SetNormalTime();
        gameManager.Instance.restartLVL();
    }
}
