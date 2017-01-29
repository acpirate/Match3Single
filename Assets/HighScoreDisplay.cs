using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreDisplay : MonoBehaviour {

    public Text highScoreText;

	// Use this for initialization
	void Start () {
		if (PlayerPrefs.HasKey(Constants.MODE1HIGHSCOREPREF))
        {
            highScoreText.text = "High Score:" + PlayerPrefs.GetInt(Constants.MODE1HIGHSCOREPREF).ToString();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
