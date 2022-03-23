using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPlayer : MonoBehaviour
{
   [HideInInspector] public PlayerInputs input;
    SlugMovement movement;
    deflector deflector;

    Vector2 _moveInput;
    Vector2 _rotationInput;

    public Vector3 MoveInput { get { return new Vector3( _moveInput.x, 0, _moveInput.y); } } 
    public Vector2 RotationInput { get { return _rotationInput; } } 

    void Awake()
    {
        movement = GetComponent<SlugMovement>();
        deflector= GetComponent<deflector>();
        input = new PlayerInputs();
        SetInput();

        if (!movement.inOverworld)
        {
            ClipBoardManager.gameObject.SetActive(false);

        }
        else
            ClipBoardManager.gameObject.SetActive(true);
    }
    public void Update()
    {
        if(gameManager.Instance.isInGamePlay())
        {
            input.Gameplay.Enable();
            input.Dialogue.Disable();
        }
        else
        {
            input.Gameplay.Disable();
            input.Dialogue.Enable();

        }
    }
    public void OnEnable()
    {
        input.Gameplay.Enable();
    }
    public void OnDisable()
    {
        input.Gameplay.Disable();
    }
    public void SetInput()
    {
        //Dialogue
        if (dialogueManager)
            input.Dialogue.Select.performed += ctx => nextDialogue();
        //input.Dialogue.RightShoulder.performed += ctx =>nextDialogue();
        //input.Dialogue.LeftShoulder.performed += ctx => nextDialogue();
        //input.Dialogue.RightTrigger.performed += ctx => nextDialogue();
        //input.Dialogue.LeftTrigger.performed += ctx => nextDialogue();


        //Gameplay
        input.Gameplay.RightTrigger.performed += ctx => deflector.deflectRelease();
        input.Gameplay.RightShoulder.performed += ctx => deflector.deflectRelease();

        input.Gameplay.LeftTrigger.performed += ctx => movement.Dash();
        input.Gameplay.LeftShoulder.performed += ctx => movement.Dash();


        input.Gameplay.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        input.Gameplay.Move.canceled += ctx => _moveInput = Vector2.zero;

        input.Gameplay.Aim.performed += ctx => _rotationInput = ctx.ReadValue<Vector2>();
        input.Gameplay.Aim.canceled += ctx => _rotationInput= Vector2.zero;
    
        input.Gameplay.Escape.performed += ctx => pauseGame();
        input.Gameplay.Options.performed += ctx => pauseGame();
        //input.Gameplay.Restart.performed += ctx => restartLevel();


        input.Gameplay.ButtonWest.performed += ctx => deflector.OverwordlInteract();
        input.Gameplay.ButtonSouth.performed += ctx => deflector.OverwordlInteract();
        if(ClipBoardManager)
        input.Gameplay.ButtonNorth.performed += ctx => ClipBoardManager.inputClipBoard();
    }


    public DialogueManager dialogueManager;
    public clipboardUIManager ClipBoardManager;

    public void nextDialogue()
    {
        dialogueManager.inputAdvanceDialogue();
    }
    // create events fore each button
    public void ButtonEast()
    {

    }

    public void pauseGame()
    {
        gameManager.Instance.pause();
    }

    public void restartLevel()
    {
        gameManager.Instance.SetNormalTime();
        gameManager.Instance.restartLVL();
    }
    //returns
    public Vector3 MousePosition()
    {
        return input.Gameplay.MousePosition.ReadValue<Vector2>();
    }
}
