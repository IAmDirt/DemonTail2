using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class onenableMix : MonoBehaviour
{
    public bool GiveTicket;
    public ClipBoardBooleans blackBoard;


    public bool resetAll;
    public void OnEnable()
    {
        if(resetAll)
        {
            blackBoard.clueCollection2.haveTicket = false;
            return;
        }

        if (GiveTicket)
        {
            blackBoard.clueCollection2.haveTicket = true;
        }
    }

    public void Start()
    {
        if(GiveTicket)
        LeanTween.moveY(gameObject, 3, 0.7f).setLoopPingPong().setEaseOutQuad();
    }
}