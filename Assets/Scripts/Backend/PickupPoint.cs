using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPoint : MonoBehaviour {

    
    private EventArchive _eventArchive;
    private BoxCollider _pointCollider;

    // Start is called before the first frame update
    void Start() {
        
        _eventArchive = FindObjectOfType<EventArchive>();
    }

    private void OnTriggerEnter(Collider other) {
        
        Debug.Log($"something entered trigger: {other.name}, {other.tag}");
        
        if(other.CompareTag("Player")) {
            
            _eventArchive.InvokeOnDeliveryPickup();
        }
    }
}