using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TerminalController : MonoBehaviour {

    public Text terminalDisplay;
    public InputField terminalInput;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ProcessCommand()
    {
        AddLineToTerminalDisplay(terminalInput.text);
        ResetInput();
    }

    void ResetInput()
    {
        terminalInput.text = "";
        terminalInput.ActivateInputField();
    }

    void AddLineToTerminalDisplay(string stringToAdd)
    {
        terminalDisplay.text += "\n - " + stringToAdd;
    }
}
