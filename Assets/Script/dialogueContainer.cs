using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class dialogueContainer : MonoBehaviour
{
    public TextAsset inkFile;

    public bool OnlyTriggerOnce;
    private bool _alreadyTriggered;
    public GameObject promt;
    private GameObject buttonPromt;
    public void Start()
    {
        dialoguePromt = Instantiate(promt, transform.position, promt.transform.rotation, transform);
        buttonPromt = dialoguePromt.transform.GetChild(0).gameObject;
        buttonPromt.SetActive(false);
        ToglePromt();
    }

    public void triggerDialogue(DialogueManager dialogueManager)
    {
        if (OnlyTriggerOnce && !_alreadyTriggered || !OnlyTriggerOnce)
        {
            dialogueManager.StartDialogue(inkFile);
            _alreadyTriggered = true;
        }
        ToglePromt();
    }

    private GameObject dialoguePromt;
    public void ToglePromt()
    {
        dialoguePromt.SetActive(!_alreadyTriggered);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            buttonPromt.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {


        if (other.gameObject.layer == 10)
        {
            buttonPromt.SetActive(false);
        }
    }
}
