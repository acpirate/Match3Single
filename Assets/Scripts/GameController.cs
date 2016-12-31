using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {CANCLICK, CANTCLICK}

public class GameController : MonoBehaviour {

    public static GameState gameState = GameState.CANCLICK;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
