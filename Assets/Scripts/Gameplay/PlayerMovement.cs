using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    // public ScooterInfo scooter;

    public Transform rearWheel;
    public Transform frontWheel;
    
    private Rigidbody _rigidbody;

    private bool _isAvailable;

    private bool _isGrounded;
    
    private void Awake() {

        _rigidbody = GetComponent<Rigidbody>();

        StartMovement();
    }


    public void StartMovement() {

        _isAvailable = true;

        StartCoroutine(InputCheck());
        StartCoroutine(GroundCheck());
    }

    private IEnumerator GroundCheck() {

        while(_isAvailable) {

            Debug.DrawRay(transform.position, -transform.up * .25f, Color.red);
            
            if(Physics.Raycast(transform.position, -transform.up, .25f)) {

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
            
            if(Input.GetKey("up") || Input.GetKeyDown("w")) {
                
                Debug.Log("adding force forwards");
                
                _rigidbody.AddForceAtPosition(transform.forward * 60, rearWheel.position, ForceMode.Force);
            }

            if(Input.GetKey("down") || Input.GetKeyDown("s")) {
                
                Debug.Log("adding force backwards");
                
                if(_rigidbody.velocity.z >= 0) {
                    
                    _rigidbody.AddForceAtPosition(transform.forward * -30, frontWheel.position, ForceMode.Force);

                    yield return null;
                }
                    
                _rigidbody.AddForceAtPosition(transform.forward * -15, rearWheel.position, ForceMode.Force);
            }
            
            yield return null;
        }
    }

}