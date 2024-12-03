using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTarget : MonoBehaviour {

    private EventArchive _eventArchive;
    
    // Start is called before the first frame update
    void Start() {

        _eventArchive = FindObjectOfType<EventArchive>();

        _eventArchive.OnCheckpointChange += Direct;
    }

    private void Direct(Transform target) {
        
        StopAllCoroutines();

        StartCoroutine(LookAtTarget(target));
    }

    private IEnumerator LookAtTarget(Transform targetTransform) {

        while(true) {

            transform.LookAt(targetTransform.position);
            
            yield return null;
        }
        
    }
    
    // Update is called once per frame
    void Update() {
    }
}