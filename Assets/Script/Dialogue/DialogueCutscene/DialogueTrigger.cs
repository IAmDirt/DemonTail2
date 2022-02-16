using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public cutSceneDialogue[] myDialouge;

    [HideInInspector]
    public DialogueManagerCutscene dm;
    public virtual void Start()
    {
        var dialougeManager = GameObject.Find("DialogueManager");
        if (dialougeManager != null)
            dm = dialougeManager.GetComponent<DialogueManagerCutscene>();
    }
    public bool talked;
    public virtual void OnTriggerEnter2D(Collider2D player)
    {
        if (player.tag == "Player" && !talked)
        {
            dm.playDialogue(myDialouge);
            talked = true;
        }
        else
            return;
    }
}