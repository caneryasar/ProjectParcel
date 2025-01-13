using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Transform model;
    public Transform collider;
    
    public float maxSpeed;
    public float acceleration;
    public float deceleration;
    public float turnSpeed;
    public float jumpForce;

    private float _currentSpeed;
    private float _defaultMaxSpeed;
    private float _defaultTurnSpeed;
    private float _defaultacceleration;
    private float _defaultdeceleration;

    private float _verticalVelocity;

    private EventArchive _eventArchive;

    private Vector2 _inputMove;
    private bool _inputJump;
    private bool _inputWheelie;

    private bool _isGrounded;
    private bool _isGrinding;

    private bool _isPlayable;

    private readonly float Gravity = -9.81f;

    private void Awake() {

        _eventArchive = FindObjectOfType<EventArchive>();

        _eventArchive.OnMoveInput += v2 => _inputMove = v2;
        _eventArchive.OnJumpInput += j => _inputJump = j;
        _eventArchive.OnWheelieInput += w => _inputWheelie = w;
        _eventArchive.OnGameStart += () => _isPlayable = true;
    }

    // Start is called before the first frame update
    void Start() {
        
        
    }

    // Update is called once per frame
    void Update() {

        if(!_isPlayable) { return; }
        
        // Debug.DrawRay(transform.position, Vector3.down * .1f, Color.red);
        
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, .1f, LayerMask.GetMask("Ground"));
        
        Debug.Log($"jump: {_inputJump} / grounded: {_isGrounded}");

        if(_inputMove.y > 0) {

            _currentSpeed += acceleration * Time.deltaTime;
        }
        else if(_inputMove.y < 0) {

            _currentSpeed -= deceleration * Time.deltaTime;
        }
        else {

            if(_currentSpeed > 0) {

                _currentSpeed -= deceleration * .5f * Time.deltaTime;

                if(_currentSpeed < 0) { _currentSpeed = 0; }
            }
            else if(_currentSpeed < 0) {

                _currentSpeed += deceleration * 2f * Time.deltaTime;

                if(_currentSpeed > 0) { _currentSpeed = 0f; }
            }
        }

        if(_inputWheelie) {
            
            var wheelieAngle = Mathf.Clamp(_inputMove.x * 15f, 0f, -25f);

            var modelEuler = model.localRotation.eulerAngles;
            var modelEulerTarget = new Vector3(wheelieAngle, modelEuler.y, modelEuler.z);
            var modelRotation = Quaternion.Euler(modelEulerTarget);
        
            model.localRotation = Quaternion.Slerp(model.localRotation, modelRotation, wheelieAngle * Time.deltaTime);
        
        
            var colliderEuler = model.localRotation.eulerAngles;
            var colliderEulerTarget = new Vector3(wheelieAngle, colliderEuler.y, colliderEuler.z);
            var colliderRotation = Quaternion.Euler(colliderEulerTarget);
        
            collider.localRotation = Quaternion.Slerp(collider.localRotation, colliderRotation,  wheelieAngle * Time.deltaTime);
            
        }

        /*
        if(_isGrounded) {

            if(_inputJump) {

                _verticalVelocity += Mathf.Sqrt(jumpForce * -2f * Gravity);
            }
        }
        else {

            Debug.Log("lower please?");
            if(_verticalVelocity < 0) { _verticalVelocity = 0f; }
            
            _verticalVelocity += Gravity * Time.deltaTime;
            
        }
        */
        
        _currentSpeed = Mathf.Clamp(_currentSpeed, -maxSpeed * .05f, maxSpeed);
        
        transform.Translate(Vector3.forward * (_currentSpeed * Time.deltaTime) + Vector3.up * (_verticalVelocity * Time.deltaTime));

        if(_currentSpeed != 0) {
            
            transform.Rotate(0f, _inputMove.x * turnSpeed * Time.deltaTime, 0f);
        
            var reverseFactor = _currentSpeed > 0 ? -1f : 1f;

            var leanTime = 10f;
        
            var leanAngle = Mathf.Clamp(_inputMove.x * 15f, -15f, 15f) * reverseFactor;

            var modelEuler = model.localRotation.eulerAngles;
            var modelEulerTarget = new Vector3(modelEuler.x, modelEuler.y, leanAngle);
            var modelRotation = Quaternion.Euler(modelEulerTarget);
        
            model.localRotation = Quaternion.Slerp(model.localRotation, modelRotation, leanTime * Time.deltaTime);
        
        
            var colliderEuler = model.localRotation.eulerAngles;
            var colliderEulerTarget = new Vector3(colliderEuler.x, colliderEuler.y, leanAngle);
            var colliderRotation = Quaternion.Euler(colliderEulerTarget);
        
            collider.localRotation = Quaternion.Slerp(collider.localRotation, colliderRotation,  leanTime * Time.deltaTime);
        }
        
    }
}