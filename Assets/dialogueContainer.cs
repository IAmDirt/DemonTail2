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
    public void Start()
    {
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

    public GameObject dialoguePromt;
    public void ToglePromt()
    {
        dialoguePromt.SetActive(!_alreadyTriggered);
    }
}
