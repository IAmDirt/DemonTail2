using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateManager : MonoBehaviour
{

    [Header("State Machine")]
    [SerializeField]
    private string currentStateName;
    private IState currentState;


    [Header("bossStuff")]
    public Transform player;

  /*  public Deflect DeflectState = new Deflect();
    public Dashing dashState = new Dashing();
    public Running runState = new Running();
    public Idle idleState = new Idle();*/
    public virtual void Awake()
    {
       // setNewState(idleState);
    }
    public virtual void Update()
    {
        currentState.updateState(this);
    }
    public virtual void FixedUpdate()
    {
        //FSM 
        currentState.FixedUpdateState(this);
    }

    public void setNewState(IState newState)
    {
        /*if (newState == currentState)
        {
            return;
        }*/
        currentState?.exitState(this);

        currentState = newState;
        currentStateName = newState.ToString();

        currentState.enterState(this);
    }

}
