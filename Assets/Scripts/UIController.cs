using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour {

    public static UIController Instance;

    public GameObject dialogPanel;
    public Text scoreDisplay;
    public Text chainDisplay;

	public TextMeshProUGUI bonusDisplayText;
	public Image bonusOuterCircleImage;

	public GameObject pausePanel;

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
		UpdateBonusDisplay();
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

	public void UpdateBonusDisplay() 
	{
		float angle = GameController.Instance.bonusTimeLeft/Constants.BONUSTIME;

		string bonusDisplayString = string.Format("{0:00}%",GameController.Instance.bonusTimeLeft/Constants.BONUSTIME*100f);

		bonusOuterCircleImage.fillAmount = angle;

		bonusDisplayText.text = bonusDisplayString;

	}

	public void PauseClicked()
	{
//		Debug.Log("pauseclicked");
		if (GameController.Instance.gameState==GAMESTATE.CANSELECT) 
		{
			GameController.Instance.gameState = GAMESTATE.PAUSED;
			pausePanel.SetActive(true);
			MusicController.instance.MusicFadeOut();
			Time.timeScale = 0;

		}
		else if (GameController.Instance.gameState==GAMESTATE.PAUSED)
		{
			GameController.Instance.gameState = GAMESTATE.CANSELECT;
			pausePanel.SetActive(false);
			MusicController.instance.MusicFadeIn();
			Time.timeScale = 1;
			//unpause
		}
	}

}
