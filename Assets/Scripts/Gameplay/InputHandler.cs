using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour {
    
    //todo: make a better easier to use movement mechanic

    private EventArchive _eventArchive;

    private InputAction _move;
    private InputAction _sprint;
    private InputAction _jump;

    private bool _isPlayable;

    private void Awake() {

        _eventArchive = GetComponent<EventArchive>();

    }

    // Start is called before the first frame update
    void Start() {

        _eventArchive.OnGameStart += () => _isPlayable = true;
        
        _move = InputSystem.actions.FindAction("Move");
        _sprint = InputSystem.actions.FindAction("Sprint");
        _jump = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update() {
        
        _eventArchive.InvokeOnMoveInput(_move.ReadValue<Vector2>());
        _eventArchive.InvokeOnWheelieInput(_sprint.IsPressed());
        _eventArchive.InvokeOnJumpInput(_jump.triggered);
        
        
        
        
    }
}