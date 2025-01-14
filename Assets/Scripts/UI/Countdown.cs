using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour {

    public TextMeshProUGUI countdownDigits;
    public float startTime;
    public float addedTime;

    private float _remainingTime;
    private EventArchive _eventArchive;

    private bool _isAvaliable;
    
    private void OnEnable() {

        _eventArchive = FindObjectOfType<EventArchive>();
        _eventArchive.OnDeliveryDropoff += AddTime;
        _eventArchive.OnGameOver += () => _isAvaliable = false;
        
        StartCounter();
    }

    private void StartCounter() {
        
        _isAvaliable = true;
        _remainingTime = startTime;
    }

    private void AddTime() { _remainingTime += addedTime; }

    private void Update() {

        if(!_isAvaliable) { return; }

        if(_remainingTime <= 0) {

            _remainingTime = 0;
            
            countdownDigits.text = "00:00";

            _eventArchive.InvokeOnGameOver();
            
            return;
        }
        
        _remainingTime -= Time.deltaTime;
        
        var minutes = Mathf.FloorToInt(_remainingTime / 60f);
        var seconds = Mathf.FloorToInt(_remainingTime % 60f);
        
        var timeFormatted = $"{minutes:00}:{seconds:00}";

        countdownDigits.text = timeFormatted;
    }


}