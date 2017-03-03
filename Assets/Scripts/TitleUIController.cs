﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIController : MonoBehaviour {

    public void GoToGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void GoToTitle()
    {
        SceneManager.LoadScene("Title");
    }
    public void GoToSelectScreen()
    {
        SceneManager.LoadScene("ModeSelect");
    }

    public void QuitTitleClick()
    {
        Application.Quit();
    }
}