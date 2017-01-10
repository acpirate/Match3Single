using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TerminalController : MonoBehaviour {

    public delegate void CommandAction(string[] splitCommand);
    public static event CommandAction OnCommand;

    public static bool deactivating = false;

    public Text terminalDisplay;
    public InputField terminalInput;
    public BoardController boardController;

    string helpMessage = "";
    //used to display help message in lateupdate if the command isn't handled
    bool commandHandled = false;

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
            commandHandled = false;
            AddLineToTerminalDisplay("> " + terminalInput.text);
            OnCommand(terminalInput.text.Split(' '));
            DisplayHelp(terminalInput.text.Split(' ')[0]);


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
        terminalDisplay.text += "\n" + stringToAdd;
    }

    public void OnEnable()
    {
        OnCommand += ListBaseMatches;
        helpMessage += "LISTBASEMATCHES |";
        OnCommand += ToggleCoords;
        helpMessage += "TOGGLECOORDS | ";
        OnCommand += SetTile;
        helpMessage += "SETTILE | ";
        OnCommand += ResetBoard;
        helpMessage += "RESETBOARD | ";
        OnCommand += ListPossibleMatches;
        helpMessage += "LISTPOSSIBLEMATCHES |";
    //    OnCommand += Command1;
    //    OnCommand += Command2;
    //    OnCommand("this is the command string");
    }

    public void OnDisable()
    {
        helpMessage = "";
        OnCommand -= ToggleCoords;
        OnCommand -= SetTile;
        OnCommand -= ResetBoard;
        OnCommand -= ListBaseMatches;
        OnCommand -= ListPossibleMatches;
    //    OnCommand -= Command1;
    //    OnCommand -= Command2;
    }

    public void DisplayHelp(string commandString)
    {
        if (!commandHandled)
        {
            AddLineToTerminalDisplay(" - INVALID COMMAND");
            AddLineToTerminalDisplay(" - COMMAND LIST: " + helpMessage);
        }
         
    }


 /*   void Command1(string commandString)
    {
        Debug.Log("command1 " + commandString);
    }

    void Command2(string commandString)
    {
        Debug.Log("command2 " + commandString);
    } */

// CONSOLE COMMANDS

    void ToggleCoords(string[] splitCommand)
    {
        if (splitCommand[0].ToUpper() != "TOGGLECOORDS") return;
        commandHandled = true;

        boardController.ToggleCoords();

        AddLineToTerminalDisplay(" - toggling the coordinate display");

        HelperObjects.showCoords = !HelperObjects.showCoords;

    }

    void SetTile(string[] splitCommand)
    {


        //command validate
        if (splitCommand[0].ToUpper() != "SETTILE") return;
        commandHandled = true;


        bool validParams = true;

        //usage string
        string usage = "SETTILE [0-7] [0-7] [ BLUE | BROWN | GREEN | PURPLE | RED | WHITE | YELLOW]";

        //validate number of params
        if (splitCommand.Length != 4)
        {
            validParams = false;
        }

        //validate number params values
        int paramValue = -9999;
        if (validParams  && !(int.TryParse(splitCommand[1],out paramValue)))
        {
            validParams = false;
        }
        if (validParams && (paramValue > 7 || paramValue < 0))
        {
            validParams = false;
        }
        if (validParams && !(int.TryParse(splitCommand[2], out paramValue)))
        {
            validParams = false;
        }
        if (validParams && (paramValue > 7 || paramValue < 0))
        {
            validParams = false;
        }

        if (validParams)
        {

            //validate string param value
            HashSet<string> validTypes = new HashSet<string>();
            validTypes.Add("BLUE");
            validTypes.Add("BROWN");
            validTypes.Add("GREEN");
            validTypes.Add("PURPLE");
            validTypes.Add("RED");
            validTypes.Add("WHITE");
            validTypes.Add("YELLOW");

            if (!validTypes.Contains(splitCommand[3].ToUpper()))
            {
                validParams = false;
            }
        }

        if (!validParams)
        {
            AddLineToTerminalDisplay(" - " + usage);
            return;
        }

        AddLineToTerminalDisplay(" - Setting tile at " + splitCommand[1] + "," + splitCommand[2] + " to " + splitCommand[3].ToUpper());
        boardController.SetTileTypeFromString(int.Parse(splitCommand[1]), int.Parse(splitCommand[2]), splitCommand[3].ToUpper());


    }
    //randomizes the board tiles
    void ResetBoard(string[] splitCommand)
    {
        if (splitCommand[0].ToUpper() != "RESETBOARD") return;
        commandHandled = true;
        boardController.CreateBoard();
        AddLineToTerminalDisplay(" - RESETTING THE BOARD");

    }

    //shows the straight line matches available on the board 
    void ListBaseMatches(string[] splitCommand)
    {
        if (splitCommand[0].ToUpper() != "LISTBASEMATCHES") return;
        commandHandled = true;

        List<Match> matches = boardController.GetBaseMatches();

        if (matches.Count < 1)
        {
            AddLineToTerminalDisplay(" - NO MATCHES");
            return;
        }
        foreach (Match match in boardController.GetBaseMatches())
        {
            AddLineToTerminalDisplay(match.ToString());
        }
       

    }

    void ListPossibleMatches(string[] splitCommand)
    {
        if (splitCommand[0].ToUpper() != "LISTPOSSIBLEMATCHES") return;
        commandHandled = true;

        List<Swap> possibleMatches = boardController.PossibleMatches();

        if (possibleMatches.Count <1 )
        {
            AddLineToTerminalDisplay(" - NO POSSIBLE MATCHES");
            return;
        }

        foreach (Swap swap in boardController.PossibleMatches())
        {
            AddLineToTerminalDisplay("Possible Matche:" + swap.ToString());
        }
    }



}
