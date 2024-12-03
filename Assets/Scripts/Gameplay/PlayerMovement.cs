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
    [SerializeField] private float _multipliedForwardSpeed = 35f;
    [SerializeField] private float _backwardSpeed = 5f;
    [SerializeField] private float _brakeStrength = 10f;

    [SerializeField] private float _tiltAngle = 15f;
    [SerializeField] private float _tiltTime = .25f;
    [SerializeField] private float _wheelieAngle = -55f;
    [SerializeField] private float _wheelieTime = .25f;

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
        
        
        StartCoroutine(InputCheck());
        StartCoroutine(MovementCheck());
        
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
    }

    private void JumpTween() {
        
        if(_jumpTween.IsPlaying()) { return; }
        
        _jumpTween = _modelTransform.DOScale(new Vector3(1.25f,.75f,1f), .15f).OnComplete(() => {

            _modelTransform.DOScale(new Vector3(.75f, 1.25f, 1f), .25f);
            transform.DOMoveY(10f, .25f).OnComplete(() => {
                        
                transform.DOMoveY(0, .25f);
                _modelTransform.DOScale(Vector3.one, .25f)
                    .OnComplete(() => _modelTransform.DOScale(new Vector3(1.25f,.75f,1f), .15f)
                        .OnComplete(() => _modelTransform.DOScale(Vector3.one, .15f)));
                        
            });
        });
    }
    

    #endregion
    private IEnumerator MovementCheck() {

        while(_isAvailable) {

            // Debug.DrawRay(transform.position, -transform.up * .75f, Color.red);
            
            if(Physics.Raycast(transform.position, -transform.up, .75f)) {

                _isGrounded = true;
                
                yield return null;
            }

            _isGrounded = false;
            
            
            yield return null;
        }
    }

    private IEnumerator InputCheck() {

        while(_isAvailable) {

            if(!_isGrounded) { yield return null; }

            var verticalInput = Input.GetAxis("Vertical");
            var steer = Input.GetAxis("Horizontal");
            
            if(verticalInput > 0) {

                _isSpeeding = true;
                transform.position += transform.forward * (verticalInput * _forwardSpeed * Time.deltaTime);
                
            }
            else if(verticalInput < 0) {

                _isBacking = true;
                transform.position -= transform.forward * (_backwardSpeed * Time.deltaTime);
            }
            else {
                
                _isSpeeding = false;
                _isBacking = false;
            }
            
            var angles = transform.rotation.eulerAngles;
            var rotation = transform.rotation;

            var colliderRotation = _colliderTransform.localRotation;
            var modelRotation = _modelTransform.localRotation;
            
            if(_isSpeeding) { _turnSpeed = 200; }
            else if(_isBacking) { _turnSpeed = 50; }
            else { _turnSpeed = 0; }


            if(Input.GetKey(KeyCode.LeftShift) && _isSpeeding) {

                
                WheelieTween(-_wheelieAngle, modelRotation, colliderRotation, true);
                // _modelTransform.DOLocalRotate(new Vector3(-55f, modelRotation.y, modelRotation.z), .25f);
                // _colliderTransform.DOLocalRotate(new Vector3(-55f, colliderRotation.y, colliderRotation.z), .25f);

                _forwardSpeed = _multipliedForwardSpeed;
                _turnSpeed = 300f;
            }
            else if(Input.GetKeyUp(KeyCode.LeftShift)) {

                WheelieTween(-0, modelRotation, colliderRotation, false);
                // _modelTransform.DOLocalRotate(new Vector3(0, modelRotation.y, modelRotation.z), .25f);
                // _colliderTransform.DOLocalRotate(new Vector3(0, colliderRotation.y, colliderRotation.z), .25f);
                
                _forwardSpeed = 20f;
                _turnSpeed = 200f;
            }
            

            if(Input.GetKeyDown(KeyCode.Space)) {
                
                JumpTween();
                
            }
            
            if(Input.GetKeyUp(KeyCode.Space)) {
                

                
            }
            
            if(steer > 0 && (_isSpeeding || _isBacking)) {
                
                if(_isSpeeding) {

                    TiltTween(-_tiltAngle, modelRotation, colliderRotation, true);
                    // _modelTransform.DOLocalRotate(new Vector3(modelRotation.x, modelRotation.y, -_tiltAngle), .25f);
                    // _colliderTransform.DOLocalRotate(new Vector3(colliderRotation.x, colliderRotation.y, -_tiltAngle), .25f);
                }
                else if(_isBacking) {

                    TiltTween(_tiltAngle, modelRotation, colliderRotation, true);
                    // _modelTransform.DOLocalRotate(new Vector3(modelRotation.x, modelRotation.y, _tiltAngle), .25f);
                    // _colliderTransform.DOLocalRotate(new Vector3(colliderRotation.x, colliderRotation.y, _tiltAngle), .25f);
                }
                
                angles.y += steer * _turnSpeed * Time.deltaTime;
                rotation.eulerAngles = angles;
                transform.rotation = rotation;
            }
            else if(steer < 0 && (_isSpeeding || _isBacking)) {

                if(_isSpeeding) {
                    
                    TiltTween(_tiltAngle, modelRotation, colliderRotation, true);
                    // _modelTransform.DOLocalRotate(new Vector3(modelRotation.x, modelRotation.y, _tiltAngle), .25f);
                    // _colliderTransform.DOLocalRotate(new Vector3(colliderRotation.x, colliderRotation.y, _tiltAngle), .25f);
                }
                else if(_isBacking) {
                    
                    TiltTween(-_tiltAngle, modelRotation, colliderRotation, true);
                    // _modelTransform.DOLocalRotate(new Vector3(modelRotation.x, modelRotation.y, -_tiltAngle), .25f);
                    // _colliderTransform.DOLocalRotate(new Vector3(colliderRotation.x, colliderRotation.y, -_tiltAngle), .25f);
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
                // _modelTransform.DOLocalRotate(new Vector3(0, 0, 0), .15f);
                // _colliderTransform.DOLocalRotate(new Vector3(0, 0, 0), .15f);
            }
            
            yield return null;
        } 
    }

}