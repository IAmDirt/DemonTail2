using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class dialogueContainer : MonoBehaviour
{
    public TextAsset inkFile;

    public bool OnlyTriggerOnce;
    private bool _alreadyTriggered;
    public void triggerDialogue(DialogueManager dialogueManager)
    {
        if(OnlyTriggerOnce && !_alreadyTriggered || !OnlyTriggerOnce)
        {
        dialogueManager.StartDialogue(inkFile);
            _alreadyTriggered = true;
        }
    }

}
