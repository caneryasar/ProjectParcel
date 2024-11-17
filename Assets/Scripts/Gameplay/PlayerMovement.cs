using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    // public ScooterInfo scooter;

    public Transform collider;
    public Transform model;

    [SerializeField] private float _forwardSpeed = 50f;
    [SerializeField] private float _backwardSpeed = 10f;
    [SerializeField] private float _brakeStrength = 25f;

    [SerializeField] private float _skewAngle = 15f;
    
    private Rigidbody _rigidbody;

    private bool _isAvailable;

    private bool _isGrounded;
    
    private bool _isSpeeding;
    private bool _isBacking;

    private float _turnSpeed = 0;

    private Transform _colliderTransform;
    private Transform _modelTransform;

    private Tweener _modelRotateRight;
    private Tweener _colliderRotateRight;
    
    private Tweener _modelRotateLeft;
    private Tweener _colliderRotateLeft;

    private Tweener _modelRotateDefault;
    private Tweener _colliderRotateDefault;
    
    private void Awake() {

        /*
        _modelRotateRight = _modelTransform.DOLocalRotate(new Vector3(0, 0, -15f), .25f).OnComplete(() => _modelRotateRight.Kill());
        _colliderRotateRight = _colliderTransform.DOLocalRotate(new Vector3(0, 0, -15f), .25f).OnComplete(() => _colliderRotateRight.Kill());
        _modelRotateLeft = _modelTransform.DOLocalRotate(new Vector3(0, 0, 15f), .25f).OnComplete(() => _modelRotateLeft.Kill());
        _colliderRotateLeft = _colliderTransform.DOLocalRotate(new Vector3(0, 0, 15f), .25f).OnComplete(() => _colliderRotateLeft.Kill());
        _modelRotateDefault = _modelTransform.DOLocalRotate(new Vector3(0, 0, 0), .15f).OnComplete(() => _modelRotateDefault.Kill());
        _colliderRotateDefault = _colliderTransform.DOLocalRotate(new Vector3(0, 0, 0), .15f).OnComplete(() => _colliderRotateDefault.Kill());
        */
        
        _rigidbody = GetComponent<Rigidbody>();

        StartMovement();
        
        _colliderTransform = collider.transform;
        _modelTransform = model.transform;
    }

    //todo: make movement with wheel collider at some point.
    
    public void StartMovement() {

        _isAvailable = true;

        _modelRotateDefault.Play();
        _colliderRotateDefault.Play();
        
        StartCoroutine(InputCheck());
        StartCoroutine(MovementCheck());
    }

    private IEnumerator MovementCheck() {

        while(_isAvailable) {

            Debug.DrawRay(transform.position, -transform.up * .75f, Color.red);
            
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

            // var colliderAngles = _colliderTransform.rotation.eulerAngles;
            var colliderRotation = _colliderTransform.localRotation;

            // var modelAngles = _modelTransform.rotation.eulerAngles;
            var modelRotation = _modelTransform.localRotation;
            
            if(_isSpeeding) { _turnSpeed = 100; }
            else if(_isBacking) { _turnSpeed = 25; }
            else { _turnSpeed = 0; }
            
                
            if(steer > 0) {
                
                // Debug.Log($"model euler: {modelRotation}, collider euler: {colliderRotation}");
                
                /*if(modelRotation.z < _skewAngle && (_isBacking || _isSpeeding)) {
                    
                    modelRotation.z -= (steer * _skewAngle) * Time.deltaTime;
                    colliderRotation.z -= (steer * _skewAngle) * Time.deltaTime;
                    _modelTransform.localRotation = modelRotation;
                    _colliderTransform.localRotation = colliderRotation;
                }*/
                
                _modelRotateRight = _modelTransform.DOLocalRotate(new Vector3(0, 0, -15f), .25f);
                _colliderRotateRight = _colliderTransform.DOLocalRotate(new Vector3(0, 0, -15f), .25f);
                
                angles.y += steer * _turnSpeed * Time.deltaTime;
                rotation.eulerAngles = angles;
                transform.rotation = rotation;
            }
            else if(steer < 0) {

                /*
                if(modelRotation.z < _skewAngle && (_isBacking || _isSpeeding)) {
                    
                    modelRotation.z -= (steer * _skewAngle) * Time.deltaTime;
                    colliderRotation.z -= (steer * _skewAngle) * Time.deltaTime;
                    _modelTransform.localRotation = modelRotation;
                    _colliderTransform.localRotation = colliderRotation;
                }
                */
                
                _modelRotateLeft = _modelTransform.DOLocalRotate(new Vector3(0, 0, 15f), .25f);
                _colliderRotateLeft = _colliderTransform.DOLocalRotate(new Vector3(0, 0, 15f), .25f);
                
                angles.y += steer * _turnSpeed * Time.deltaTime;
                rotation.eulerAngles = angles;
                transform.rotation = rotation;
            }
            else {
                
                rotation.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
                transform.rotation = rotation;
                
                /*
                modelRotation.eulerAngles = Vector3.zero;
                _modelTransform.localRotation = modelRotation;
                
                colliderRotation.eulerAngles = Vector3.zero;
                _colliderTransform.localRotation = colliderRotation;
                */
        
                _modelRotateDefault = _modelTransform.DOLocalRotate(new Vector3(0, 0, 0), .15f);
                _colliderRotateDefault = _colliderTransform.DOLocalRotate(new Vector3(0, 0, 0), .15f);
            }
            
            yield return null;
        }
    }

}