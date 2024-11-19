using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;


//todo: make movement with wheel collider at some point.

public class PlayerMovement : MonoBehaviour {

    public Transform playerCollider;
    public Transform playerModel;

    [SerializeField] private float _forwardSpeed = 20f;
    [SerializeField] private float _multipliedForwardSpeed = 35f;
    [SerializeField] private float _backwardSpeed = 5f;
    [SerializeField] private float _brakeStrength = 10f;

    [SerializeField] private float _skewAngle = 15f;

    private bool _isAvailable;

    private bool _isGrounded;
    
    private bool _isSpeeding;
    private bool _isBacking;

    private float _turnSpeed = 0;

    private Transform _colliderTransform;
    private Transform _modelTransform;
    private Vector3 _defRotation;
    private Vector3 _defPosition;
    private Vector3 _defScale;
    
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
                
                _modelTransform.DOLocalRotate(new Vector3(-55f, modelRotation.y, modelRotation.z), .25f);
                _colliderTransform.DOLocalRotate(new Vector3(-55f, colliderRotation.y, colliderRotation.z), .25f);

                _forwardSpeed = _multipliedForwardSpeed;
                _turnSpeed = 300f;
            }

            if(Input.GetKeyUp(KeyCode.LeftShift)) {
                
                _modelTransform.DOLocalRotate(new Vector3(0, modelRotation.y, modelRotation.z), .25f);
                _colliderTransform.DOLocalRotate(new Vector3(0, colliderRotation.y, colliderRotation.z), .25f);
                
                _forwardSpeed = 20f;
            }

            if(Input.GetKeyDown(KeyCode.Space)) {
                

                _modelTransform.DOScale(new Vector3(1.25f,.75f,1f), .15f).OnComplete(() => {

                    _modelTransform.DOScale(new Vector3(.75f, 1.25f, 1f), .25f);
                    transform.DOMoveY(10f, .25f).OnComplete(() => {
                        
                        transform.DOMoveY(0, .25f);
                        _modelTransform.DOScale(Vector3.one, .25f)
                            .OnComplete(() => _modelTransform.DOScale(new Vector3(1.25f,.75f,1f), .15f)
                                .OnComplete(() => _modelTransform.DOScale(Vector3.one, .15f)));
                        
                    });
                });
                
            }
            
            if(steer > 0 && (_isSpeeding || _isBacking)) {
                
                if(_isSpeeding) {
                    
                    _modelTransform.DOLocalRotate(new Vector3(modelRotation.x, modelRotation.y, -_skewAngle), .25f);
                    _colliderTransform.DOLocalRotate(new Vector3(colliderRotation.x, colliderRotation.y, -_skewAngle), .25f);
                }
                else if(_isBacking) {
                    
                    _modelTransform.DOLocalRotate(new Vector3(modelRotation.x, modelRotation.y, _skewAngle), .25f);
                    _colliderTransform.DOLocalRotate(new Vector3(colliderRotation.x, colliderRotation.y, _skewAngle), .25f);
                }
                
                angles.y += steer * _turnSpeed * Time.deltaTime;
                rotation.eulerAngles = angles;
                transform.rotation = rotation;
            }
            else if(steer < 0 && (_isSpeeding || _isBacking)) {

                if(_isSpeeding) {
                    
                    _modelTransform.DOLocalRotate(new Vector3(modelRotation.x, modelRotation.y, _skewAngle), .25f);
                    _colliderTransform.DOLocalRotate(new Vector3(colliderRotation.x, colliderRotation.y, _skewAngle), .25f);
                }
                else if(_isBacking) {
                    
                    _modelTransform.DOLocalRotate(new Vector3(modelRotation.x, modelRotation.y, -_skewAngle), .25f);
                    _colliderTransform.DOLocalRotate(new Vector3(colliderRotation.x, colliderRotation.y, -_skewAngle), .25f);
                }
                
                
                angles.y += steer * _turnSpeed * Time.deltaTime;
                rotation.eulerAngles = angles;
                transform.rotation = rotation;
            }
            else {
                
                rotation.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
                transform.rotation = rotation;
        
                _modelTransform.DOLocalRotate(new Vector3(0, 0, 0), .15f);
                _colliderTransform.DOLocalRotate(new Vector3(0, 0, 0), .15f);
            }
            
            yield return null;
        }
    }

}