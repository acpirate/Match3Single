using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMasterController : MonoBehaviour {

    public static SoundMasterController instance;

    public AudioClip buttonClick;
    public AudioClip tileClick;
    public AudioClip tileSwap;
    public AudioClip swapError;
    public AudioClip matchSound;
    public AudioSource tileVibrateAudioSource;
    public AudioSource tileSwapAudioSource;

    AudioSource myAudioSource;


    void Awake()
    {
        //singleton pattern
        if (instance==null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            myAudioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayButtonClick()
    {
        myAudioSource.PlayOneShot(buttonClick);
    }

    public void PlayTileClick()
    {
        myAudioSource.PlayOneShot(tileClick);
    }

    public void PlayMatchSound()
    {
        myAudioSource.PlayOneShot(matchSound,.4f);
    }

    public void StartTileVibrate()
    {
        tileVibrateAudioSource.Play();
    }

    public void StopTileVibrate()
    {
        tileVibrateAudioSource.Stop();
    }

    public void PlayTileSwap()
    {

        tileSwapAudioSource.Stop();
        tileSwapAudioSource.clip = tileSwap;
        tileSwapAudioSource.Play();
    }

    public void ReverseTileSwap()
    {
        tileSwapAudioSource.Stop();
        tileSwapAudioSource.clip = swapError;
        tileSwapAudioSource.Play();
    }
}
