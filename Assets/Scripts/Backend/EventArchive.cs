using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventArchive : MonoBehaviour {

    public event Action OnDeliveryPickup, OnDeliveryDropoff;
    public event Action<Transform> OnCheckpointChange; 
    
    
    public void InvokeOnDeliveryPickup() {
        
        OnDeliveryPickup?.Invoke();
    }

    public void InvokeOnDeliveryDropoff() {
        
        OnDeliveryDropoff?.Invoke();
    }

    public void InvokeOncheckpointChange(Transform newPoint) {
        
        OnCheckpointChange?.Invoke(newPoint);
    }
}