using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour {

    public GameObject frogSit;
    public GameObject frogPose;
    
    
    public Transform model;
    [FormerlySerializedAs("collider")] public Transform modelCollider;
    public Transform handlebar;
    public Transform frontGuard;
    public Transform frontWheel;
    public Transform rearWheel;

    public ParticleSystem smoke;
    public MeshRenderer mesh;
    
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

    private Quaternion _defaultHandleRotation;
    private Quaternion _defaultGuardRotation;

    private float _wheelRotationFactor = 100f;

    private float _verticalVelocity;

    private float _handleBarRotataion = 15f;
    private float _wheelRotation;

    private EventArchive _eventArchive;

    private Vector2 _inputMove;
    private bool _inputJump;
    private bool _inputWheelie;

    private bool _isGrounded;
    private bool _isGrinding;

    private bool _isPlayable;

    private readonly float Gravity = -9.81f;

    private ShowTarget _direction;

    private void Awake() {
        
        frogPose.SetActive(false);

        _eventArchive = FindObjectOfType<EventArchive>();

        _eventArchive.OnMoveInput += v2 => _inputMove = v2;
        _eventArchive.OnJumpInput += j => _inputJump = j;
        _eventArchive.OnWheelieInput += w => _inputWheelie = w;
        _eventArchive.OnGameStart += () => {
            
            _direction.gameObject.SetActive(true);
            _isPlayable = true;
        };
        _eventArchive.OnGameOver += () => _isPlayable = false;
        _eventArchive.OnDeliveryPickup += () => {
            
            frogSit.SetActive(false);
            frogPose.SetActive(true);
        };
        _eventArchive.OnDeliveryDropoff += () => {
            
            frogSit.SetActive(true);
            frogPose.SetActive(false);
        };

        _direction = GetComponentInChildren<ShowTarget>();
    }

    // Start is called before the first frame update
    void Start() {
        
        _direction.gameObject.SetActive(false);

        _defaultHandleRotation = handlebar.localRotation;
        _defaultGuardRotation = frontGuard.localRotation;
        
        smoke.Play();
    }

    // Update is called once per frame
    void Update() {

        if(!_isPlayable) { return; }
        
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, .1f, LayerMask.GetMask("Ground"));
        
        
        var emissionModule  = smoke.emission;
        mesh.materials[4].DisableKeyword("_EMISSION");
        
        
        if(_inputMove.y > 0) {

            _currentSpeed += acceleration * Time.deltaTime;

            emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(16f);
        }
        else if(_inputMove.y < 0) {
            
            mesh.materials[4].EnableKeyword("_EMISSION");

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
            
            emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(4f);
        }
        


        if(_inputWheelie) {

            var currentAngle = model.localEulerAngles.x;
            
            var wheelieAngle = Mathf.Clamp(currentAngle, 0f, -25f);

            wheelieAngle = Mathf.LerpAngle(currentAngle, wheelieAngle, 100f * Time.deltaTime);

            var modelEuler = model.localRotation.eulerAngles;
            var modelEulerTarget = new Vector3(wheelieAngle, modelEuler.y, modelEuler.z);
            var modelRotation = Quaternion.Euler(modelEulerTarget);
        
            model.localRotation = Quaternion.Slerp(model.localRotation, modelRotation, wheelieAngle * Time.deltaTime);
        
        
            var colliderEuler = model.localRotation.eulerAngles;
            var colliderEulerTarget = new Vector3(wheelieAngle, colliderEuler.y, colliderEuler.z);
            var colliderRotation = Quaternion.Euler(colliderEulerTarget);
        
            modelCollider.localRotation = Quaternion.Slerp(modelCollider.localRotation, colliderRotation,  wheelieAngle * Time.deltaTime);
            
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

        if(_inputMove.x != 0) {

            var targetAngle = Mathf.Lerp(-_handleBarRotataion, _handleBarRotataion, (_inputMove.x + 1f) * 0.5f);
            
            
            var handleEuler = handlebar.localEulerAngles;
            handleEuler.y = Mathf.LerpAngle(handleEuler.y, targetAngle, Time.deltaTime * turnSpeed);
            handlebar.localRotation = Quaternion.Euler(handleEuler);
            
            var guardEuler = frontGuard.localEulerAngles;
            guardEuler.y = Mathf.LerpAngle(guardEuler.y, targetAngle, Time.deltaTime * turnSpeed);
            frontGuard.localRotation = Quaternion.Euler(guardEuler);
        }
        else {

            handlebar.localRotation = _defaultHandleRotation;
            frontGuard.localRotation = _defaultGuardRotation;
        }
        
        _currentSpeed = Mathf.Clamp(_currentSpeed, -maxSpeed * .05f, maxSpeed);
        
        transform.Translate(Vector3.forward * (_currentSpeed * Time.deltaTime) + Vector3.up * (_verticalVelocity * Time.deltaTime));
        
        _wheelRotation *= _currentSpeed * Time.deltaTime;

        var direction = Mathf.Sign(_currentSpeed);
        var rotationSpeed = Mathf.Abs(_currentSpeed) * _wheelRotationFactor;
        var rotationThisFrame = rotationSpeed * Time.deltaTime;
        
        frontWheel.Rotate(Vector3.up, rotationThisFrame * direction * -1f);
        rearWheel.Rotate(Vector3.up, rotationThisFrame * direction * -1f);

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
        
            modelCollider.localRotation = Quaternion.Slerp(modelCollider.localRotation, colliderRotation,  leanTime * Time.deltaTime);
        }
        
    }
}