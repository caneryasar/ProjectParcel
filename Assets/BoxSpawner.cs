using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoxSpawner : MonoBehaviour {

    public Transform hand;
    public GameObject box;

    public List<Texture> boxTextures;
    
    private GameObject _box;
    private Material _boxMat;

    private EventArchive _eventArchive;

    private void Awake() {
        
        _eventArchive = FindObjectOfType<EventArchive>();
    }

    private void Start() {
        

        _box = Instantiate(box, hand);
        // _box.transform.position = hand.transform.position;
        // _box.transform.localScale = hand.transform.localScale;
        // _box.transform.rotation = hand.transform.rotation;
        
        _box.SetActive(false);
        _boxMat = _box.GetComponent<Renderer>().materials[1];


        _eventArchive.OnDeliveryPickup += ShowBox;
        _eventArchive.OnDeliveryDropoff += HideBox;

    }

    public void ShowBox() {
        
        _boxMat.mainTexture = boxTextures[Random.Range(0, boxTextures.Count)];
        
        _box.SetActive(true);
    }

    private void HideBox() {
        
        _box.SetActive(false);
    }


}
