using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    [Header("Panels")]
    public GameObject mainMenu;
    public GameObject gameplay;
    public GameObject countdown;
    public GameObject gameOver;
    
    //MainMenu Elements
    [Header("MainMenu Elements")] 
    public GameObject title;
    public Button playButton;
    public Button customizeButton;
    public Button settingsButton;
    public Button quitButton;
    
    //Positions;
    public Transform _titleEndPosition;
    public Transform _playEndPosition;
    public Transform _customizeEndPosition;
    public Transform _settingsEndPosition;
    public Transform _quitEndPosition;
    
    public Transform _gameOverTitleEndPosition;
    public Transform _gameOverScoreEndPosition;
    public Transform _gameOverRestartEndPosition;
    public Transform _gameOverQuitEndPosition;

    private EventArchive _eventArchive;
    
    
    private void Awake() {
        
        mainMenu.SetActive(true);
        gameplay.SetActive(false);
        countdown.SetActive(false);
        // settings?.SetActive(false);
        // customization?.SetActive(false);


        _eventArchive = FindObjectOfType<EventArchive>();
    }

    private void Start() {

        playButton.interactable = false;
        customizeButton.interactable = false;
        settingsButton.interactable = false;
        quitButton.interactable = false;

        title.transform.DOMove(_titleEndPosition.position, .25f, true).OnComplete(() => {

            playButton.transform.DOMove(_playEndPosition.position, .25f, true).OnComplete(() => playButton.interactable = true);
            customizeButton.transform.DOMove(_customizeEndPosition.position, .35f, true);
            settingsButton.transform.DOMove(_settingsEndPosition.position, .45f, true);
            quitButton.transform.DOMove(_quitEndPosition.position, .55f, true).OnComplete(() => quitButton.interactable = true);
        });
        
    }

    public void StartButton() {
        
        mainMenu.SetActive(false);
        gameplay.SetActive(false);
        countdown.SetActive(true);

        _eventArchive.InvokeOnGettingReadyToStart();
        
        StartCountdown();
    }

    private void StartCountdown() {

        var countdownText = countdown.GetComponentInChildren<TextMeshProUGUI>();

        countdownText.text = "3";

        countdownText.DOFade(0, 1).OnComplete(() => {

            countdownText.alpha = 100;
            countdownText.text = "2";
            countdownText.color = Color.yellow;

            countdownText.DOFade(0, 1).OnComplete(() => {


                countdownText.alpha = 100;
                countdownText.text = "1";
                countdownText.color = Color.red;
                
                
                countdownText.DOFade(0, 1).OnComplete(() => {


                    countdownText.alpha = 100;
                    countdownText.text = "GO!";
                    countdownText.color = Color.green;
                }).OnComplete(() => {
                    
                    _eventArchive.InvokeOnGameStart();
                    gameplay.SetActive(true);
                    countdown.SetActive(false);
                });
            });
        });
    }


}