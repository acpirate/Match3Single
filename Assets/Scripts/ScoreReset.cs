using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreReset : MonoBehaviour {

    public void Mode1ScoreReset()
    {
        PlayerPrefs.SetInt(Constants.MODE1HIGHSCOREPREF, 0);
    }


}
