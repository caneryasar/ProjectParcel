using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTarget : MonoBehaviour {

    private EventArchive _eventArchive;
    
    
    void Start() {

        _eventArchive = FindObjectOfType<EventArchive>();

        _eventArchive.OnCheckpointChange += Direct;
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