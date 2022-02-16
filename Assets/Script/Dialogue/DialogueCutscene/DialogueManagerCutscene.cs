using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DialogueManagerCutscene : Singleton<DialogueManagerCutscene>
{
    public SuperTextMesh nameTextMesh;
    public SuperTextMesh dialogueTextMesh;


    private Queue<string> sentences = new Queue<string>();

    public bool talking = false;

    public Animator animator;

    public void Awake()
    {
        talking = false;
    }
  

    public bool textIsReading()
    {
        if (dialogueTextMesh.reading)
            return true;
        else
            return false;
    }


    private Queue<cutSceneDialogue> _Dialogue = new Queue<cutSceneDialogue>();

    public void playDialogue(cutSceneDialogue[] _dialogue)    //used in cutscene
    {
        if (talking)    //already talking
        {
            DisplayNextSentence();
          //  animator.SetBool("IsOpen", true);
            return;
        }
        else
        {
            //make sure sentences are empty
            talking = true;
            sentences.Clear();

            //queue up new dialogue and display first sentence
            foreach (cutSceneDialogue dialouge in _dialogue)
            {
                _Dialogue.Enqueue(dialouge);
            }
            Debug.Log(_dialogue.Length);
            dequeDialouge();
         //   animator.SetBool("IsOpen", true);
        }
    }

    public void dequeDialouge() //enqueue next dialogue
    {
        Debug.Log(_Dialogue.Count);
        if (_Dialogue.Count == 0 && !dialogueTextMesh.reading)  //if no more dialogue to Dequeue
        {
          //  GameManager.Instance.endTimeLine(); //return to gameplay
            return;
        }

        var currentdialouge = _Dialogue.Dequeue();
        nameTextMesh.text = currentdialouge.name;
        enqueSentence(currentdialouge.sentence);

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (dialogueTextMesh.reading)   //text not fully typed out
        {
            dialogueTextMesh.SpeedRead();  //speeding up letter type speed
        }
        else
        {
            if (sentences.Count == 0 && !dialogueTextMesh.reading)  //if no more sentences to display
            {
                dequeDialouge();
              //  if (animator.GetBool("IsOpen"))//small animation when changing name
                 //   animator.SetTrigger("ChangeName");
                return;
            }
            else
            {
                //sets display to new sentence
                string sentence = sentences.Dequeue();
                dialogueTextMesh.text = sentence;

                dialogueTextMesh.RegularRead(); //type out sentence over time
            }
        }
    }

    private void enqueSentence(string[] text)   //enqueue all sentences in currentDialogue
    {
        sentences.Clear();

        foreach (string sentence in text)
        {
            sentences.Enqueue(sentence);
        }
    }
    public void endDialogue()
    {
        _Dialogue.Clear();
        sentences.Clear();
       // animator.SetBool("IsOpen", false);
        talking = false;

    }
    public IEnumerator hideDialogueDisplay(float delay = 0.0f)
    {
        yield return new WaitForSeconds(delay);
        //animator.SetBool("IsOpen", false);
    }
}
