using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour {

    public float currentScore;

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
    public TextMeshProUGUI pickup;
    public TextMeshProUGUI dropoff;
    public Button replayButton;
    public Button endquitButton;
    
    [FormerlySerializedAs("_gameOverTitleEndPosition")] public Transform gameOverTitleEndPosition;
    [FormerlySerializedAs("_gameOverScoreEndPosition")] public Transform gameOverScoreEndPosition;
    [FormerlySerializedAs("_gameOverRestartEndPosition")] public Transform gameOverRestartEndPosition;
    [FormerlySerializedAs("_gameOverQuitEndPosition")] public Transform gameOverQuitEndPosition;

    
    [Header("Gameplay Elements")] 
    public TextMeshProUGUI scoreText;
    
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

        scoreText.text = $"{currentScore}$";

        title.transform.DOMove(titleEndPosition.position, .25f, true).OnComplete(() => {

            playButton.transform.DOMove(playEndPosition.position, .25f, true).OnComplete(() => playButton.interactable = true);
            customizeButton.transform.DOMove(customizeEndPosition.position, .35f, true);
            settingsButton.transform.DOMove(settingsEndPosition.position, .45f, true);
            quitButton.transform.DOMove(quitEndPosition.position, .55f, true).OnComplete(() => quitButton.interactable = true);
        });

        _eventArchive.OnDeliveryPickup += () => {
            
            pickup.transform.localScale = Vector3.zero;
            pickup.gameObject.SetActive(true);
            pickup.transform.DOScale(Vector3.one, 1f).OnComplete(() =>
                DOVirtual.DelayedCall(.5f, () =>
                    pickup.DOFade(0, .5f).OnComplete(() => {

                        pickup.gameObject.SetActive(false);
                        pickup.alpha = 1f;
                    })));
        };
        
        _eventArchive.OnDeliveryDropoff += () => {
            
            dropoff.transform.localScale = Vector3.zero;
            dropoff.gameObject.SetActive(true);
            dropoff.transform.DOScale(Vector3.one, 1f).OnComplete(() =>
                DOVirtual.DelayedCall(.5f, () =>
                    pickup.DOFade(0, .5f).OnComplete(() => {

                        dropoff.gameObject.SetActive(false);
                        dropoff.alpha = 1f;
                    })));
            
            currentScore += 100;
            scoreText.text = $"{currentScore}$";
        };

        _eventArchive.OnGameOver += GameOver;
    }

    public void StartButton() {
        
        mainMenu.SetActive(false);
        gameplay.SetActive(false);
        countdown.SetActive(true);
        gameOver.SetActive(false);

        _eventArchive.InvokeOnGettingReadyToStart();
        
        StartCountdown();
    }

    private void StartCountdown() {

        var countdownText = countdown.GetComponentInChildren<TextMeshProUGUI>();

        _eventArchive.InvokeOnCountDown();
        
        countdownText.text = "3";

        countdownText.DOFade(0, 1).OnComplete(() => {

            countdownText.alpha = 100;
            countdownText.text = "2";
            countdownText.color = Color.yellow;
            _eventArchive.InvokeOnCountDown();

            countdownText.DOFade(0, 1).OnComplete(() => {


                countdownText.alpha = 100;
                countdownText.text = "1";
                countdownText.color = Color.red;
                _eventArchive.InvokeOnCountDown();
                
                
                countdownText.DOFade(0, 1).OnComplete(() => {


                    countdownText.alpha = 100;
                    countdownText.text = "GO!";
                    countdownText.color = Color.green;
                    _eventArchive.InvokeOnGo();

                    countdownText.DOFade(0, 1);
                        
                    DOVirtual.DelayedCall(1f, () => {
                            
                        _eventArchive.InvokeOnGameStart();
                        gameplay.SetActive(true);
                        countdown.SetActive(false);
                    });
                });
            });
        });
    }

    private void GameOver() {
        
        mainMenu.SetActive(false);
        gameplay.SetActive(false);
        countdown.SetActive(false);
        gameOver.SetActive(true);
        
        score.GetComponent<TextMeshProUGUI>().text = $"MONEY EARNED: {currentScore}$";

        score.transform.DOMove(gameOverScoreEndPosition.position, .35f, true);
        endtitle.transform.DOMove(gameOverTitleEndPosition.position, .45f, true);
        replayButton.transform.DOMove(gameOverRestartEndPosition.position, .25f, true).OnComplete(() => replayButton.interactable = true);
        endquitButton.transform.DOMove(gameOverQuitEndPosition.position, .55f, true).OnComplete(() => endquitButton.interactable = true);
        
    }

    public void Replay() {

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit() {
        
        Application.Quit();
    }


}