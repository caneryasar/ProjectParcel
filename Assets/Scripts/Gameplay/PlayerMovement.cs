using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

//todo: make movement with wheel collider at some point.
//todo: initialize tweens then call tweens in a seperate functions. add a boolean to check if it has run before

public class PlayerMovement : MonoBehaviour {

    public Transform playerCollider;
    public Transform playerModel;

    [SerializeField] private float _forwardSpeed = 20f;
    [SerializeField] private float _forwardSpeedFalloff = 5f;
    [SerializeField] private float _multipliedForwardSpeed = 35f;
    [SerializeField] private float _backwardSpeed = 5f;
    [SerializeField] private float _brakeStrength = 10f;

    [SerializeField] private float _tiltAngle = 15f;
    [SerializeField] private float _tiltTime = .25f;
    [SerializeField] private float _wheelieAngle = -55f;
    [SerializeField] private float _wheelieTime = .25f;

    private float _currentSpeed;
    
    private bool _isAvailable;

    private bool _isGrounded;
    
    private bool _isSpeeding;
    private bool _isBacking;

    private bool _isTiltStarted;
    private bool _isOnTilt;
    
    private bool _isWheelieStarted;
    private bool _isOnWheelie;

    
    
    private float _turnSpeed = 0;

    private Transform _colliderTransform;
    private Transform _modelTransform;

    private Tweener _tiltModel;
    private Tweener _tiltCollider;

    private Tweener _wheelieModel;
    private Tweener _wheelieCollider;

    private Tweener _jumpTween;
    
    private void Awake() {

        StartMovement();
        
        _colliderTransform = playerCollider.transform;
        _modelTransform = playerModel.transform;
    }
    
    private void StartMovement() {

        _isAvailable = true;

        _backwardSpeed *= -1;
        
        // StartCoroutine(InputCheck());
        // StartCoroutine(MovementCheck());
        
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

        _jumpTween = _modelTransform.DOScale(new Vector3(1.25f, .75f, 1f), .15f).OnComplete(() => {

            _modelTransform.DOScale(new Vector3(.75f, 1.25f, 1f), .25f);
            transform.DOMoveY(10f, .25f).OnComplete(() => {

                transform.DOMoveY(0, .25f);
                _modelTransform.DOScale(Vector3.one, .25f)
                    .OnComplete(() => _modelTransform.DOScale(new Vector3(1.25f, .75f, 1f), .15f)
                        .OnComplete(() => _modelTransform.DOScale(Vector3.one, .15f)));

            });
        });
    }
    

    #endregion

    private void FixedUpdate() {
        
        if(Physics.Raycast(transform.position, -transform.up, .75f)) {

            _isGrounded = true;
            
            return;
        }

        _isGrounded = false;
    }

    /*
    private IEnumerator MovementCheck() {

        while(_isAvailable) {
            
            if(Physics.Raycast(transform.position, -transform.up, .75f)) {
                
                // Debug.DrawRay(transform.position, -transform.up * .75f, Color.red);

                _isGrounded = true;
                
                yield return new WaitForFixedUpdate();
            }

            _isGrounded = false;
            
            
            yield return new WaitForFixedUpdate();
        }
    }
    */
    private void Update() {
        
        if(!_isAvailable) { return; }

        if(!_isGrounded) { return; }

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
                
                if(_currentSpeed > 0) {
                    
                    _currentSpeed -= _brakeStrength * Time.deltaTime;
                    transform.position += transform.forward * (verticalInput * _currentSpeed * Time.deltaTime);
                    break;
                }

                _currentSpeed = _backwardSpeed;
                transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
                break;
            }

            default: {
                
                if(_currentSpeed > 0) {
                    
                    _currentSpeed -= _forwardSpeedFalloff * Time.deltaTime;
                    Debug.Log($"slowing? {_currentSpeed}");
                    transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
                    break;
                }

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

            _forwardSpeed = 20f;
            _turnSpeed = 200f;
        }

        if(Input.GetKeyDown(KeyCode.Space)) {
            
            if(!_isGrounded) { return; }

            JumpTween();
        }

        if(Input.GetKeyUp(KeyCode.Space)) {
            
        }

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