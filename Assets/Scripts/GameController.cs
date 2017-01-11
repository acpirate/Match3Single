using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public static GAMESTATE gameState = GAMESTATE.CANSELECT;

    public int score = 0;

    public Text scoreDisplay;


    static readonly int match3Score = 10;
    static readonly int match4Score = 15;
    static readonly int matchBlobScore = 20;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreDisplay.text = "Score: " + score.ToString();
    }

    public void ScoreMatch(Match matchToScore)
    {
        if (matchToScore.matchCoords.Count==3)
        {
            AddScore(match3Score *3);
        }
        if (matchToScore.matchCoords.Count==4)
        {
            AddScore(match4Score * 4);
        }
        if (matchToScore.matchCoords.Count > 4)
        {
            AddScore(matchToScore.matchCoords.Count * matchBlobScore);
        }
    }

    public void EndGame()
    {
        SceneManager.LoadScene("GameOver");
    }
}
