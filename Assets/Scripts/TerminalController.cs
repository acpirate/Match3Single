using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TerminalController : MonoBehaviour {

    public static bool deactivating = false;

    public Text terminalDisplay;
    public InputField terminalInput;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleConsole();
        }		
        
	}

    void LateUpdate()
    {
        deactivating = false;
    }

    //execute the console command
    public void ProcessCommand()
    {
        // dont do it if the terminal isn't active
       if (!deactivating)
        {
            AddLineToTerminalDisplay(terminalInput.text);
            //empty the input field
            ResetInput();
        }
    }


    //turn the console on and off when tab is hit
    void ToggleConsole()
    {
        deactivating = terminalInput.gameObject.activeInHierarchy;
        terminalInput.gameObject.SetActive(!terminalInput.gameObject.activeInHierarchy);
        terminalDisplay.gameObject.transform.parent.parent.gameObject.SetActive(!terminalDisplay.gameObject.transform.parent.parent.gameObject.activeInHierarchy);

        if (terminalInput.gameObject.activeInHierarchy) terminalInput.ActivateInputField();
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
