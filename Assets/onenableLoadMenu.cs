using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class onenableLoadMenu : MonoBehaviour
{
    [Header("loadMenu")]
    public bool loadMenu;
    public void OnEnable()
    {
        if (loadMenu)
        {
            SceneManager.LoadScene(0);
            return;
        }

        if (loadCustomLevel)
        {

            SceneManager.LoadScene(customLevelInt);
        }
    }
    [Header("other")]
    public bool loadCustomLevel;
    public int customLevelInt = 1;
}
