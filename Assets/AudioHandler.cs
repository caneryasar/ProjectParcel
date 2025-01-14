using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour {

    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource bikeSource;
    
    public AudioClip bgMusic;
    
    public AudioClip bikeIdle;
    public AudioClip bikeGas;

    public AudioClip collect;
    public AudioClip drop;
    
    public AudioClip shortHorn;
    public AudioClip longHorn;

    private EventArchive _eventArchive;

    private bool isStarted;


    private void Awake() {

        _eventArchive = FindObjectOfType<EventArchive>();
        _eventArchive.OnMoveInput += BikeSoundChange;
        _eventArchive.OnDeliveryPickup += PlayDeliveryPickup;
        _eventArchive.OnDeliveryDropoff += PlayDeliveryDropoff;
        _eventArchive.OnGameStart += () => isStarted = true;
        _eventArchive.OnCountDown += Horn;
        _eventArchive.OnGo += Go;
    }

    private void Go() {
        
        sfxSource.volume = .5f;
        sfxSource.PlayOneShot(shortHorn);
    }

    private void Horn() {
        
        sfxSource.volume = .5f;
        sfxSource.PlayOneShot(longHorn);
    }

    // Start is called before the first frame update
    void Start() {
        
        musicSource.loop = true;
        bikeSource.loop = true;
        
        musicSource.clip = bgMusic;
        musicSource.Play();
        
        bikeSource.clip = bikeIdle;
        bikeSource.Play();
    }

    private void BikeSoundChange(Vector2 input) {

        if(input.y > 0) {
            
            if(bikeSource.clip == bikeGas && isStarted) { return; }
        
            bikeSource.volume = .1725f;
            bikeSource.clip = bikeGas;
            bikeSource.Play();
         
            return;
        }
            
        if(bikeSource.clip == bikeIdle) { return; }

        bikeSource.volume = .4f;
        bikeSource.clip = bikeIdle;
        bikeSource.Play();
    }

    private void PlayDeliveryPickup() {
        
        sfxSource.volume = 1f;
        sfxSource.PlayOneShot(collect);
        
    }

    private void PlayDeliveryDropoff() {
        
        sfxSource.volume = 1f;
        sfxSource.PlayOneShot(drop);
    }
}