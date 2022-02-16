using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogueCutscene : MonoBehaviour {

    public bool onlyPlayOnce = false;

    public cutSceneDialogue[] dialouge;

    private bool _alreadyPlayed;
    public bool alreadyPlayed()
    {

        if (!onlyPlayOnce)
            return false;

        if (!_alreadyPlayed)
        {
            _alreadyPlayed = true;
            return false;
        }
        else
            return true;
    }
}

