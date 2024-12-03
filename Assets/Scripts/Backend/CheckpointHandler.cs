using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CheckpointHandler : MonoBehaviour {

    private EventArchive _eventArchive;
    
    public List<Transform> stores;

    public List<Transform> customers;

    public GameObject PickupPoint;
    public GameObject DropoffPoint;
    
    
    private void Awake() {

        PickupPoint.SetActive(false);
        DropoffPoint.SetActive(false);
        
        stores = new List<Transform>();
        customers = new List<Transform>();
        
        var storeObjects = GameObject.FindGameObjectsWithTag("Store");
        var customerObjects = GameObject.FindGameObjectsWithTag("Customer");
        
        foreach(var storeObject in storeObjects) {
            
            stores.Add(storeObject.transform);
        }
        
        foreach(var customerObject in customerObjects) {
            
            customers.Add(customerObject.transform);
        }
    }

    private void Start() {

        _eventArchive = FindObjectOfType<EventArchive>();
        
        _eventArchive.OnDeliveryPickup += PickupComplete;
        _eventArchive.OnDeliveryDropoff += DropoffComplete;

        StartCycle();
    }

    private void StartCycle() {

        var pickupTarget = stores[Random.Range(0, stores.Count)];

        PickupPoint.transform.position = pickupTarget.position;
        PickupPoint.transform.rotation = pickupTarget.rotation;
        PickupPoint.SetActive(true);
        
        _eventArchive.InvokeOncheckpointChange(pickupTarget);
    }

    private void PickupComplete() {
        
        PickupPoint.SetActive(false);

        var dropoffTarget = customers[Random.Range(0, customers.Count)];
        DropoffPoint.transform.position = dropoffTarget.position;
        DropoffPoint.transform.rotation = dropoffTarget.rotation;
        DropoffPoint.SetActive(true);
        
        
        _eventArchive.InvokeOncheckpointChange(dropoffTarget);
    }

    private void DropoffComplete() {
        
        DropoffPoint.SetActive(false);
        
        var pickupTarget = stores[Random.Range(0, stores.Count)];

        PickupPoint.transform.position = pickupTarget.position;
        PickupPoint.transform.rotation = pickupTarget.rotation;
        PickupPoint.SetActive(true);
        
        _eventArchive.InvokeOncheckpointChange(pickupTarget);
    }
}