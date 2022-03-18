using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class onenableLoadMenu : MonoBehaviour
{
    public void OnEnable()
    {
        SceneManager.LoadScene(0);
    }
}
