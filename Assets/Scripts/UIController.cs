using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public static UIController Instance;

    public GameObject dialogPanel;
    public Text scoreDisplay;
    public Text chainDisplay;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start () {
        dialogPanel.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        UpdateChainDisplay();
	}

    public void UpdateChainDisplay()
    {
        chainDisplay.text = "Chain: " + GameController.Instance.chain.ToString();
    }

    public void QuitButtonClicked()
    {
        dialogPanel.SetActive(true);

        DialogController dialogController = dialogPanel.GetComponent<DialogController>();

        dialogController.InitDialog("Really Quit?", "Yes", "No");
        GameController.Instance.gameState = GAMESTATE.QUITCONFIRM;
    }

    public void QuitDialogDeny()
    {
        dialogPanel.SetActive(false);
        GameController.Instance.gameState = GAMESTATE.CANSELECT;
    }

    public void ShowScore(int scoreToShow)
    {
        scoreDisplay.text = "Score: " + scoreToShow.ToString();
    }


}
