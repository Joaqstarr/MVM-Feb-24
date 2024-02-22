using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonPlayerControls : PlayerControls
{

    private Vector2 _moveInput = Vector2.zero;
    private Vector2 _lookInput = Vector2.zero;
    private PlayerInput _input;

    private void Start()
    {
        _input = GetComponent<PlayerInput>();
    }

    public override bool OnFPMove(InputValue Value)
    {
        if(!base.OnFPMove(Value))return false;

        //input code here
        _moveInput = Value.Get<Vector2>();

        return true;
    }

    public override bool OnLook(InputValue Value)
    {
        if (!base.OnFPMove(Value)) return false;

        //input code here
        _lookInput = Value.Get<Vector2>();

        if(_input.currentControlScheme == "Controller")
        {
            _lookInput *= Time.deltaTime;
        }

        return true;
    }

    public Vector2 MoveInput
    {
        get { return _moveInput; }
    }
    public Vector2 LookInput
    {
        get { return _lookInput; }
        
    }
}
