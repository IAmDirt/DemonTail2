using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class TriggerLoadLvL : MonoBehaviour
{
    public int LvLToLoad;
    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.name.Contains("Player"))
        SceneManager.LoadScene(LvLToLoad);
    }
}