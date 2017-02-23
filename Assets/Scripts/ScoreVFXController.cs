using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreVFXController : MonoBehaviour {

    public Vector3 scoreDestination;
    public float speed;

    Text scoreDisplay;

    float moveStart;

    void Awake()
    {
        moveStart = Time.time + Constants.SCOREMOVEDELAY;
        scoreDisplay = GetComponentInChildren<Text>();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time>moveStart) MoveScore();
        CheckForDeath();
    }

    public void Initialize(string inScoreDisplay, Color textColor)
    {
        scoreDisplay.text = inScoreDisplay;
        scoreDisplay.color = textColor;
    }

    void MoveScore()
    {

        transform.position = Vector3.Lerp(transform.position, scoreDestination, speed);

    }

    void CheckForDeath()
    {
        if ((transform.position-scoreDestination).sqrMagnitude<.1f)
        {
            Destroy(gameObject);
        }
    }
}
