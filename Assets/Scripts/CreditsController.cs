using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CreditsController : MonoBehaviour {

    public TextMeshProUGUI creditsTextDisplay;

	// Use this for initialization
	void Start ()
    {
       // MusicController.instance.PlayCreditsMusic();
        //  Debug.Log(((TextAsset)Resources.Load("Credits")).text);
        creditsTextDisplay.text = ((TextAsset)Resources.Load("TextFiles/Credits")).text;
    }
	
}
