using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    public ClipBoardBooleans BlackBoard;


    public GameObject boss1_Interactables;
    public GameObject boss2_Interactables;

    public bool Debug_StartWithTicket;
    public void Start()
    {
        BlackBoard.clueCollection2.haveTicket= Debug_StartWithTicket;


        var useFirstInteractables = BlackBoard.clueCollection2.haveTicket;
        boss1_Interactables.SetActive(!useFirstInteractables);
        boss2_Interactables.SetActive(useFirstInteractables);

    }
}
