﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour {

    public static MusicController instance;


    public AudioClip titleMusic;
    public AudioClip selectMusic;
    public AudioClip gameplayMusic;
    public AudioClip creditsMusic;
    public AudioClip gameoverMusic;
    public AudioMixer musicMixer;
    public AudioMixerSnapshot musicOutSnap;
    public AudioMixerSnapshot musicInSnap;
	public AudioMixer masterMixer;


    Animator myAnimator;
    AudioSource mySource;



    void Awake()
    {
        //singleton pattern
        DontDestroyOnLoad(gameObject);
        if (instance == null) {
            instance = this;
            }
        else Destroy(gameObject);

        mySource = GetComponentInChildren<AudioSource>();
        myAnimator = GetComponent<Animator>();
		masterMixer.updateMode = AudioMixerUpdateMode.UnscaledTime;
		musicMixer.updateMode = AudioMixerUpdateMode.UnscaledTime;

    }


    void OnEnable()
    {
        SceneManager.activeSceneChanged += SceneChange;
    }

    void OnDisable()
    {
        SceneManager.activeSceneChanged -= SceneChange;
    }

	// Use this for initialization
	void Start () {
        MusicFadeIn();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void PlayTitleMusic()
    {
        if (mySource.isPlaying) mySource.Stop();

        mySource.clip = titleMusic;
        mySource.Play();
    }

    public void PlaySelectMusic()
    {
        if (mySource.isPlaying) mySource.Stop();

        mySource.clip = selectMusic;
        mySource.Play();
    }

    public void PlayGameplayMusic()
    {
        if (mySource.isPlaying) mySource.Stop();

        mySource.clip = gameplayMusic;
        mySource.Play();
    }

    public void PlayGameoverMusic()
    {
        if (mySource.isPlaying) mySource.Stop();

        mySource.clip = gameoverMusic;
        mySource.Play();
    }
    
    public void PlayCreditsMusic()
    {
        if (mySource.isPlaying) mySource.Stop();

        mySource.clip = creditsMusic;
        mySource.Play();
    }

    public void MusicFadeOut()
    {

        musicOutSnap.TransitionTo(.3f);
    }

    public void MusicFadeIn()
    {
        musicInSnap.TransitionTo(2f);
        //myAnimator.SetTrigger("MusicFadeIn");
    }

    void SceneChange(Scene scene1, Scene scene2)
    {
        if (scene2.name=="Title")
        {
            PlayTitleMusic();
        }
        if (scene2.name=="Game")
        {
            PlayGameplayMusic();
        }
        if (scene2.name=="GameOver")
        {
            PlayGameoverMusic();
        }
        if (scene2.name=="Credits")
        {
            PlayCreditsMusic();
        }
        if (scene2.name=="ModeSelect")
        {
            PlaySelectMusic();
        }
    }
		
}
