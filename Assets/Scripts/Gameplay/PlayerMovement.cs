using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

//todo: make movement with wheel collider at some point.
//todo: initialize tweens then call tweens in a seperate functions. add a boolean to check if it has run before

public class PlayerMovement : MonoBehaviour {

    public Transform playerCollider;
    public Transform playerModel;

    [SerializeField] private float _forwardSpeed = 20f;
    // [SerializeField] private float _forwardSpeedFalloff = 5f;
    [SerializeField] private float _multipliedForwardSpeed = 35f;
    [SerializeField] private float _backwardSpeed = 5f;
    // [SerializeField] private float _brakeStrength = 10f;
    [SerializeField] private float _jumpForce = 10f;
    
    [SerializeField] private float _tiltAngle = 15f;
    [SerializeField] private float _tiltTime = .25f;
    [SerializeField] private float _wheelieAngle = -55f;
    [SerializeField] private float _wheelieTime = .25f;

    private float _currentSpeed;
    private float _verticalVelocity;
    
    [SerializeField] private bool _isAvailable;

    private bool _isGrounded;
    
    private bool _isSpeeding;
    private bool _isBacking;

    //todo: use isOnRails to prevent constant checking / use isSnapped to make the player position only snap once to rail / use isGrinding to make it snap out of the grind
    
    private bool _isOnRails;
    private bool _isSnapped;
    private bool _isGrinding;

    private bool _isTiltStarted;
    private bool _isOnTilt;
    
    private bool _isWheelieStarted;
    private bool _isOnWheelie;

    private float _originalFwdSpeed;
    private Vector3 _originalScale;
    
    private float _turnSpeed = 0;

    private Transform _colliderTransform;
    private Transform _modelTransform;

    private Tweener _tiltModel;
    private Tweener _tiltCollider;

    private Tweener _wheelieModel;
    private Tweener _wheelieCollider;

    private Tweener _jumpTween;

    private EventArchive _eventArchive;

    private SplineAnimate _splineAnimate;
    
    private void Awake() {

        _eventArchive = FindObjectOfType<EventArchive>();

        _eventArchive.OnGameStart += StartMovement;

        
        _colliderTransform = playerCollider.transform;
        _modelTransform = playerModel.transform;

        _originalScale = _modelTransform.localScale;
        _originalFwdSpeed = _forwardSpeed;

        _splineAnimate = GetComponent<SplineAnimate>();
    }
    
    private void StartMovement() {

        _isAvailable = true;

        _backwardSpeed *= -1;
        
    }

    private void Start() {
        
        SetupTweens();
    }

    #region Tweens
    
    private void SetupTweens() {

        _wheelieModel = _modelTransform.DOLocalRotate(Vector3.one, _wheelieTime).SetAutoKill(false).OnStart(() => _isWheelieStarted = true ).OnComplete(() => _isWheelieStarted = false);
        _wheelieModel.Pause();
        _wheelieCollider = _colliderTransform.DOLocalRotate(Vector3.one, _wheelieTime).SetAutoKill(false).OnStart(() => _isWheelieStarted = true ).OnComplete(() => _isWheelieStarted = false);
        _wheelieCollider.Pause();

        _tiltModel = _modelTransform.DOLocalRotate(Vector3.one, _tiltTime).SetAutoKill(false).OnStart(() => _isTiltStarted = true ).OnComplete(() => _isTiltStarted = false);
        _tiltModel.Pause();
        _tiltCollider = _colliderTransform.DOLocalRotate(Vector3.one, _tiltTime).SetAutoKill(false).OnStart(() => _isTiltStarted = true ).OnComplete(() => _isTiltStarted = false);
        _tiltCollider.Pause();
    }

    private void TiltTween(Vector3 modelRot, Vector3 coliderRot, bool isTilting) {

        _isOnTilt = isTilting;
        
        _tiltModel.ChangeEndValue(modelRot, true);
        _tiltModel.Restart();
        _tiltCollider.ChangeEndValue(coliderRot, true);
        _tiltCollider.Restart();
    }
    
    private void TiltTween(float endValue, Quaternion modelRotation, Quaternion colliderRotation, bool isTilting) {

        _isOnTilt = isTilting;

        var m_TiltRot = new Vector3(modelRotation.x, modelRotation.y, endValue);
        var c_TiltRot = new Vector3(colliderRotation.x, colliderRotation.y, endValue);
        
        _tiltModel.ChangeEndValue(m_TiltRot, true);
        _tiltModel.Restart();
        _tiltCollider.ChangeEndValue(c_TiltRot, true);
        _tiltCollider.Restart();
    }
    private void WheelieTween(float endValue, Quaternion modelRotation, Quaternion colliderRotation, bool isWheelieing) {
        
        _isOnWheelie = isWheelieing;
        
        var m_WheelieRot = new Vector3(endValue, modelRotation.y, modelRotation.z);
        var c_WheelieRot = new Vector3(endValue, colliderRotation.y, colliderRotation.z);
        
        _wheelieModel.ChangeEndValue(m_WheelieRot, true);
        _wheelieModel.Restart();
        _wheelieCollider.ChangeEndValue(c_WheelieRot, true);
        _wheelieCollider.Restart();
        
        Debug.Log($"is wheelieing? {_wheelieModel.IsPlaying()} <> {_isOnWheelie}");
    }

    private void JumpTween() {
        
        _jumpTween = _modelTransform.DOScale(new Vector3(_originalScale.x * 1.25f, _originalScale.y * .75f, _originalScale.z * 1f), .15f).OnComplete(() => {

            _modelTransform.DOScale(new Vector3(_originalScale.x * .75f, _originalScale.y * 1.25f, _originalScale.z * 1f), .25f);
            transform.DOMoveY(4f, .25f).OnComplete(() => {

                transform.DOMoveY(0, .25f);
                _modelTransform.DOScale(_originalScale, .25f)
                    .OnComplete(() => _modelTransform.DOScale(new Vector3(_originalScale.x * 1.25f, _originalScale.y * .75f, _originalScale.z * 1f), .15f)
                        .OnComplete(() => _modelTransform.DOScale(_originalScale, .15f)));

            });
        });
        
    }
    

    #endregion

    private void FixedUpdate() {
        
        Debug.DrawRay(transform.position, -transform.up * .75f, Color.red);
        
        if(Physics.Raycast(transform.position, -transform.up, .75f)) {

            _isGrounded = true;
            
            return;
        }
        
        

        if(Physics.SphereCast(transform.position, .5f, -transform.up, out var hit, .5f) && !_isGrinding) {
            
            if(hit.collider.CompareTag("Rail")) {

                _isGrinding = true;

                var railSpline = hit.transform.GetComponentInParent<SplineContainer>().Spline;

                var playerPosition = new float3(transform.position.x, transform.position.y, transform.position.z);

                var pointOnSpline = SplineUtility.GetNearestPoint(railSpline, playerPosition, out var nearestWorld, out var narestSpline);

                transform.position = nearestWorld;
                
                
                return;

                // railSpline.Spline.Knots
            }
        }
        
        _isGrinding = false;
        
        _verticalVelocity = -9.81f * Time.deltaTime * 7.5f;

        _isGrounded = false;
    }

    private void OnDrawGizmos() {

        if(_isGrinding) {
            
            Debug.DrawRay(transform.position, -transform.up, Color.green);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(-transform.up, .5f);
        }
    }

    private void Update() {
        
        if(!_isAvailable) { return; }

        // if(!_isGrounded) { return; }

        var verticalInput = Input.GetAxis("Vertical");
        var steer = Input.GetAxis("Horizontal");

        switch(_currentSpeed) {
            
            case > 0: {
                
                _isSpeeding = true;
                break;
            }
            
            case < 0: {
                
                _isBacking = true;
                break;
            }
            
            default: {
                
                _isSpeeding = false;
                _isBacking = false;
                break;
            }
        }

        switch(verticalInput) {
            
            case > 0: {
                
                _currentSpeed = _forwardSpeed;
                transform.position += transform.forward * (verticalInput * _currentSpeed * Time.deltaTime);
                break;
            }
            case < 0: {
                
                /*
                if(_currentSpeed > 0) {
                    
                    _currentSpeed -= _brakeStrength * Time.deltaTime;
                    transform.position += transform.forward * (verticalInput * _currentSpeed * Time.deltaTime);
                    break;
                }
                */

                _currentSpeed = _backwardSpeed;
                transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
                break;
            }

            default: {
                
                /*
                if(_currentSpeed > 0) {
                    
                    // _currentSpeed -= _forwardSpeedFalloff * Time.deltaTime;
                    _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, _forwardSpeedFalloff * Time.deltaTime);
                    Debug.Log($"slowing? {_currentSpeed}");
                    transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
                    break;
                }
                */

                _currentSpeed = 0;
                break;
            }
        }

        var angles = transform.rotation.eulerAngles;
        var rotation = transform.rotation;

        var colliderRotation = _colliderTransform.localRotation;
        var modelRotation = _modelTransform.localRotation;

        if(_isSpeeding) { _turnSpeed = 200; }
        else if(_isBacking) { _turnSpeed = 50; }
        else { _turnSpeed = 0; }


        if(Input.GetKey(KeyCode.LeftShift) && _isSpeeding) {
            
            if(!_isWheelieStarted) { WheelieTween(_wheelieAngle, modelRotation, colliderRotation, true); }

            _forwardSpeed = _multipliedForwardSpeed;
            _turnSpeed = 300f;
        }
        else if(Input.GetKeyUp(KeyCode.LeftShift)) {
            
            if(!_isWheelieStarted) { WheelieTween(0, modelRotation, colliderRotation, false); }

            _forwardSpeed = _originalFwdSpeed;
            _turnSpeed = 200f;
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            
            if(!_isGrounded) { return; }

            // JumpTween();

            _verticalVelocity = _jumpForce;
        }

        if(Input.GetKeyUp(KeyCode.Space)) {
            
        }

        var designedPosition = new Vector3(0, _verticalVelocity, 0);

        transform.Translate(designedPosition * Time.deltaTime);

        if(steer > 0 && (_isSpeeding || _isBacking)) {
            
            if(_isSpeeding && !_isTiltStarted) {
                
                TiltTween(-_tiltAngle, modelRotation, colliderRotation, true);
            }
            else if(_isBacking && !_isTiltStarted) {
                
                TiltTween(_tiltAngle, modelRotation, colliderRotation, true);
            }

            angles.y += steer * _turnSpeed * Time.deltaTime;
            rotation.eulerAngles = angles;
            transform.rotation = rotation;
        }
        else if(steer < 0 && (_isSpeeding || _isBacking)) {
            
            if(_isSpeeding && !_isTiltStarted) {
                
                TiltTween(_tiltAngle, modelRotation, colliderRotation, true);
            }
            else if(_isBacking && !_isTiltStarted) {
                
                TiltTween(-_tiltAngle, modelRotation, colliderRotation, true);
            }
            
            angles.y += steer * _turnSpeed * Time.deltaTime;
            rotation.eulerAngles = angles;
            transform.rotation = rotation;
        }
        else {
            
            rotation.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
            transform.rotation = rotation;

            var tweenInput = Vector3.zero;
            TiltTween(tweenInput, tweenInput, false);
        }
    }
    
}