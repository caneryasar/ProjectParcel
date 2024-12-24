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

    [SerializeField] private Transform _selectedStore;
    [SerializeField] private Transform _selectedCustomer;
    
    
    private void Awake() {

        PickupPoint.SetActive(false);
        DropoffPoint.SetActive(false);
        
        stores = new List<Transform>();
        customers = new List<Transform>();
        
        var storeObjects = GameObject.FindGameObjectsWithTag("Store");
        var customerObjects = GameObject.FindGameObjectsWithTag("Customer");
        
        foreach(var storeObject in storeObjects) {
            
            stores.Add(storeObject.transform.GetChild(0));
        }
        
        foreach(var customerObject in customerObjects) {
            
            customers.Add(customerObject.transform.GetChild(0));
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

        _selectedStore = pickupTarget;
        
        PickupPoint.transform.position = new Vector3(pickupTarget.position.x, PickupPoint.transform.position.y, pickupTarget.position.z);
        PickupPoint.transform.rotation = pickupTarget.rotation;
        PickupPoint.SetActive(true);
        
        _eventArchive.InvokeOncheckpointChange(pickupTarget);
    }

    private void PickupComplete() {
        
        PickupPoint.SetActive(false);

        var dropoffTarget = customers[Random.Range(0, customers.Count)];
        DropoffPoint.transform.position = new Vector3(dropoffTarget.position.x, DropoffPoint.transform.position.y, dropoffTarget.position.z);
        DropoffPoint.transform.rotation = dropoffTarget.rotation;
        DropoffPoint.SetActive(true);

        _selectedCustomer = dropoffTarget;
        
        _eventArchive.InvokeOncheckpointChange(dropoffTarget);
    }

    private void DropoffComplete() {
        
        DropoffPoint.SetActive(false);
        
        var pickupTarget = stores[Random.Range(0, stores.Count)];

        _selectedStore = pickupTarget;
        
        PickupPoint.transform.position = new Vector3(pickupTarget.position.x, PickupPoint.transform.position.y, pickupTarget.position.z);
        PickupPoint.transform.rotation = pickupTarget.rotation;
        PickupPoint.SetActive(true);
        
        _eventArchive.InvokeOncheckpointChange(pickupTarget);
    }
}