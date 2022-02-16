using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class triggerPlayable : MonoBehaviour
{
    private bool PlayedOnce;
    private PlayableDirector playable;

    [Header("activate by name")]
    public bool ActivateByName;
    public GameObject activateWith;
  //  public bool removeObject = true;
    public void Start()
    {
        playable = GetComponent<PlayableDirector>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!ActivateByName)
        {
            if (other.tag == "Player")
            {
                Debug.Log(other.name);
                playPlayable();
            }
        }
        else if (ActivateByName && !PlayedOnce)
        {
            var name = activateWith.name;

            if (other.name.Contains(name))
            {
                playable.Play();
                PlayedOnce = true;

              /*  if (removeObject)
                {
                    //other.GetComponent<interactable>().StopInteract();

                    other.gameObject.SetActive(false);
                }*/
            }
        }
    }
    public void playPlayable()
    {
        if (PlayedOnce)
            return;
        playable.Play();
        PlayedOnce = true;
    }
}
