using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPlayer : MonoBehaviour
{
    PlayerControls controls;
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
        controls = new PlayerControls();
        SetInput();
    }
    public void Update()
    {
    }
    public void OnEnable()
    {
        controls.Gameplay.Enable();
    }
    public void OnDisable()
    {
        controls.Gameplay.Disable();
    }
    public void SetInput()
    {
        controls.Gameplay.RightTrigger.performed += ctx => deflector.deflectRelease();
        controls.Gameplay.LeftTrigger.performed += ctx => movement.Dash();

        controls.Gameplay.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move.canceled += ctx => _moveInput = Vector2.zero;

        controls.Gameplay.Aim.performed += ctx => _rotationInput = ctx.ReadValue<Vector2>();
        controls.Gameplay.Aim.canceled += ctx => _rotationInput= Vector2.zero;
    }
    void test()
    {
        Debug.Log("test");
    }
    // create events fore each button
    public void ButtonEast()
    {

    }


    //returns
    public Vector3 MousePosition()
    {
        return controls.Gameplay.MousePosition.ReadValue<Vector2>();
    }
}
