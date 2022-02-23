using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [Header("Hvilken level den starter på")]
    public int level = 0;
    public void PlayGame ()
    {
       SceneManager.LoadScene(level);
    }
    
    public void QuitGame ()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}
