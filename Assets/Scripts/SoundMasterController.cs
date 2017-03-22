using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMasterController : MonoBehaviour {

    public static SoundMasterController instance;

    public AudioClip buttonClick;

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

    public void PlayClick()
    {
        myAudioSource.PlayOneShot(buttonClick);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
