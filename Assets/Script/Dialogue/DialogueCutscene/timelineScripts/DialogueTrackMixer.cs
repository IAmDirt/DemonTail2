using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Ink.Runtime;
public class DialogueTrackMixer : PlayableBehaviour //putting logic over all clips on track here
{
    public TextAsset inkFile;

    private PlayableDirector director;
    private DialogueBehaviour lastClip;

    public override void OnBehaviourPlay(Playable playable, FrameData info) //enterTrack
    {
    }
    public override void OnPlayableCreate(Playable playable)    //TrackStart
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var newClip = getCurrentBehavior(playable);

        if (newClip == null)
        {
            return;
        }

        /*if (newClip.onExitHideDialogue) 
        {
            DialogueManager.Instance.hideDialogue();
            return;
        }*/
        if (newClip.canPause())
        {
            //timeline is paused
            Debug.Log("paused track Mixer");
            gameManager.Instance.PauseTimeline(director);
        }

        if (newClip.canPlayDialogue())
            StartDialogue();
    }
    private void StartDialogue()
    {
        DialogueManager.Instance.StartDialogue(inkFile);
        gameManager.Instance.resetSkip();
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)    //when the the trackis stopped
    {
    }
    public DialogueBehaviour getCurrentBehavior(Playable playable)
    {
        DialogueBehaviour currentClip = null;

        int inputCount = playable.GetInputCount();
        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            if (inputWeight > 0f) // check for current clip
            {
                var clip = (ScriptPlayable<DialogueBehaviour>)playable.GetInput(i);
                DialogueBehaviour input = clip.GetBehaviour();
                currentClip = input;
                lastClip = currentClip;

                return currentClip;
            }
        }

        /* if (lastClip != null)
             if (lastClip.onExitHideDialogue)
             {
                 DialogueManagerCutscene.Instance.StartCoroutine(DialogueManagerCutscene.Instance.hideDialogueDisplay());
                // GameManager.Instance.returnToGameplay();
                 lastClip = null;
                 Debug.Log("hide");
             }*/
        return currentClip;
    }
}