using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour {

    private EventArchive _eventArchive;

    private InputAction _move;
    private InputAction _sprint;
    private InputAction _jump;

    private bool _isPlayable;

    private void Awake() {

        _eventArchive = GetComponent<EventArchive>();
    }

    void Start() {

        _eventArchive.OnGameStart += () => _isPlayable = true;
        
        _move = InputSystem.actions.FindAction("Move");
        _sprint = InputSystem.actions.FindAction("Sprint");
        _jump = InputSystem.actions.FindAction("Jump");
    }

    void Update() {

        if(!_isPlayable) { return; }
        
        _eventArchive.InvokeOnMoveInput(_move.ReadValue<Vector2>());
        _eventArchive.InvokeOnWheelieInput(_sprint.IsPressed());
        _eventArchive.InvokeOnJumpInput(_jump.triggered);
    }
}