using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class DialogueBehaviour : PlayableBehaviour// old behavior logis is clip by clip basis
{
    [Space(10)]
    [Header("Dialouge Related")]
    public bool onEnterPauseTimeline = true;
   // public bool onExitHideDialogue = true;
    public bool displayDialogue= true;


    private bool clipPlayed = false;
    private bool dialoguePlayed = false;
    private PlayableDirector director;

    public override void OnPlayableCreate(Playable playable)
    {
        director = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public bool canPause()
    {
        if (onEnterPauseTimeline && !clipPlayed)   //pause timeline when enter
        {
            clipPlayed = true;
            return true;
        }
        return false;
    }

    public bool canPlayDialogue()
    {
        if (displayDialogue && !dialoguePlayed)   //pause timeline when enter
        {
            dialoguePlayed = true;
            return true;
        }
        return false;
    }
    #region behaviors
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)//kind of like update, but when track plays
    {
    }

    public override void OnBehaviourPlay(Playable playable, FrameData info) //enter
    {
        //else if (IsStartClip)
        //{
        //    DialogueManager.Instance.CutsceneEndDialogue();
        //    //DialogueManager.Instance.StartCoroutine(DialogueManager.Instance.EndDialogue());
        //}
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)    //when the the track is traversing to a new track / changing track
    {
      /*  if (!Application.isPlaying)
        {
            return;
        }

        var duration = playable.GetDuration();
        var time = playable.GetTime();
        var count = time + info.deltaTime;

        if ((info.effectivePlayState == PlayState.Paused && count > duration) || Mathf.Approximately((float)time, (float)duration))
        {
            // Execute your finishing logic here:
            Debug.Log("Clip done!");
        }*/
    }
    #endregion


}