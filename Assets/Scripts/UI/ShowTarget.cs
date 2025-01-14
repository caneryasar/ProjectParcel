using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ShowTarget : MonoBehaviour {

    private EventArchive _eventArchive;

    private Transform _currentTarget;

    private List<GameObject> _parts;


    private void Awake() {

        _eventArchive = FindObjectOfType<EventArchive>();

        // _eventArchive.OnCheckpointChange += Direct;
        _eventArchive.OnCheckpointChange += t => {
            
            _currentTarget = t;
            
            if(gameObject.activeSelf) {Direct(t);}
        };

        /*
        this.ObserveEveryValueChanged(_ => _currentTarget).Subscribe(t => {
            
            if(!gameObject.activeSelf){ return; }
            
            Direct(t);
        });
    */
    }

    private void OnEnable() {

        Direct(_currentTarget);
    }

    private void Direct(Transform target) {
        
        StopAllCoroutines();

        var lookAt = new Vector3(target.position.x, transform.position.y, target.position.z);
        
        StartCoroutine(LookAtTarget(lookAt));
    }

    private IEnumerator LookAtTarget(Vector3 targetTransform) {

        while(true) {

            transform.LookAt(targetTransform);
            
            yield return null;
        }
        
    }
}