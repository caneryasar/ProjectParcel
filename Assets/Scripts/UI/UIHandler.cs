using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
    [FormerlySerializedAs("_titleEndPosition")] public Transform titleEndPosition;
    [FormerlySerializedAs("_playEndPosition")] public Transform playEndPosition;
    [FormerlySerializedAs("_customizeEndPosition")] public Transform customizeEndPosition;
    [FormerlySerializedAs("_settingsEndPosition")] public Transform settingsEndPosition;
    [FormerlySerializedAs("_quitEndPosition")] public Transform quitEndPosition;
    
    [Header("GameOver Elements")] 
    public GameObject endtitle;
    public GameObject score;
    public Button replayButton;
    public Button endquitButton;
    
    [FormerlySerializedAs("_gameOverTitleEndPosition")] public Transform gameOverTitleEndPosition;
    [FormerlySerializedAs("_gameOverScoreEndPosition")] public Transform gameOverScoreEndPosition;
    [FormerlySerializedAs("_gameOverRestartEndPosition")] public Transform gameOverRestartEndPosition;
    [FormerlySerializedAs("_gameOverQuitEndPosition")] public Transform gameOverQuitEndPosition;

    private EventArchive _eventArchive;
    
    
    private void Awake() {
        
        mainMenu.SetActive(true);
        gameplay.SetActive(false);
        countdown.SetActive(false);
        gameOver.SetActive(false);
        // settings?.SetActive(false);
        // customization?.SetActive(false);


        _eventArchive = FindObjectOfType<EventArchive>();
    }

    private void Start() {

        playButton.interactable = false;
        customizeButton.interactable = false;
        settingsButton.interactable = false;
        quitButton.interactable = false;

        title.transform.DOMove(titleEndPosition.position, .25f, true).OnComplete(() => {

            playButton.transform.DOMove(playEndPosition.position, .25f, true).OnComplete(() => playButton.interactable = true);
            customizeButton.transform.DOMove(customizeEndPosition.position, .35f, true);
            settingsButton.transform.DOMove(settingsEndPosition.position, .45f, true);
            quitButton.transform.DOMove(quitEndPosition.position, .55f, true).OnComplete(() => quitButton.interactable = true);
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

    private void GameOver() {

       

        replayButton.transform.DOMove(gameOverRestartEndPosition.position, .25f, true).OnComplete(() => playButton.interactable = true);
        score.transform.DOMove(gameOverScoreEndPosition.position, .35f, true);
        endtitle.transform.DOMove(gameOverTitleEndPosition.position, .45f, true);
        quitEndPosition.transform.DOMove(gameOverQuitEndPosition.position, .55f, true).OnComplete(() => quitButton.interactable = true);
        
        
    }


}