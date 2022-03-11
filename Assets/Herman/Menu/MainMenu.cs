using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class MainMenu : MonoBehaviour {

    public PlayableDirector director;
    public string StartLevelName = "Overworld Demo";
    public void PlayGame ()
    {

        //deAnimate menu
        LeanTween.scale(transform.GetChild(0).gameObject, Vector3.zero, 1).setEaseInOutQuart();
        LeanTween.scale(transform.GetChild(1).gameObject, Vector3.zero, 1).setEaseInOutQuart();
        director.Play();
            Invoke("loadDelay", 3.5f);
       // StartCoroutine(LoadScene());
    }
    
    public void QuitGame ()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    void loadDelay()
    {
        SceneManager.LoadSceneAsync(StartLevelName);
    }
    IEnumerator LoadScene()
    {
        yield return null;

        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(StartLevelName);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        Debug.Log("Pro :" + asyncOperation.progress);
        //When the load is still in progress, output the Text and progress bar

        float waitTime = 3.5f;
    float Currenttime = 0f;
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            //  m_Text.text = "Loading progress: " + (asyncOperation.progress * 100) + "%";
            Currenttime += Time.deltaTime;
            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                //Change the Text to show the Scene is ready
              //  m_Text.text = "Press the space bar to continue";
                //Wait to you press the space key to activate the Scene
              //  if (Input.GetKeyDown(KeyCode.Space))
                    //Activate the Scene
                    if(Currenttime >= waitTime)
                    asyncOperation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
