using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : MonoBehaviour
{
    public ClipBoardBooleans BlackBoard;


    public GameObject[] boss1_Interactables;
    public GameObject[] boss2_Interactables;


    public bool Debug_StartWithTicket;
    public void Start()
    {
        if (Debug_StartWithTicket)
            BlackBoard.clueCollection2.haveTicket = Debug_StartWithTicket;


        var useFirstInteractables = BlackBoard.clueCollection2.haveTicket;

        if (useFirstInteractables) setPlayerCasino();
        goSetActive(boss1_Interactables, !useFirstInteractables);
        goSetActive(boss2_Interactables, useFirstInteractables);
    }

    public void goSetActive(GameObject[] goList, bool bo)
    {
        foreach (var item in goList)
        {
            item.SetActive(bo);
        }
    }
    public GameObject player;
    public Transform casinoEntrance;
    public void setPlayerCasino()
    {
        player.transform.position = casinoEntrance.position;
    }
}