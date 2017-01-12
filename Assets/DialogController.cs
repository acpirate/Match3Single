using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour {

    public Text dialogText;
    public Text confirmText;
    public Text denyText;
   

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void InitDialog(string inDialogText, string inConfirmText, string inDenyText)
    {
        dialogText.text = inDialogText;
        confirmText.text = inConfirmText;
        denyText.text = inDenyText;
    }

    public void OnConfirm()
    {
        GameController.Instance.dialogConfirm = true;
    }

    public void OnDeny()
    {
        GameController.Instance.dialogDeny = true;
    }
}
