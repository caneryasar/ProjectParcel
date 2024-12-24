using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerCameraChanger : MonoBehaviour {

    public CinemachineVirtualCamera mainMenuVC;
    public CinemachineVirtualCamera gameplayVC;

    private EventArchive _eventArchive;

    private void Awake() {
        
        _eventArchive = FindObjectOfType<EventArchive>();
        _eventArchive.OnGettingReadyToStart += ChangeCamera;
    }

    private void Start() {

    }

    private void ChangeCamera() {
        
        mainMenuVC.gameObject.SetActive(false);
    }

}