using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMasterController : MonoBehaviour {

    string sceneTarget = "";

    public Animator faderAnimator;

    public static SceneMasterController instance;



    void Awake()
    {
            instance = this;
    }

    public void GoToGame()
    {
        SetupSceneSwitch("Game");
    }
    public void GoToTitle()
    {
       SetupSceneSwitch("Title");
    }

    public void GoToSelectScreen()
    {
        SetupSceneSwitch("ModeSelect");
    }

    public void GoToGameOverScreen()
    {
        SetupSceneSwitch("GameOver");
    }

    public void QuitTitleClick()
    {
        Application.Quit();
    }

    public void GoToCredits()
    {
        SetupSceneSwitch("Credits");
    }

    void SetupSceneSwitch(string nameOfScene)
    {

        faderAnimator.SetTrigger("FadeOutTrigger");
        Debug.Log("called fade out trigger");
        MusicController.instance.MusicFadeOut();
        //Debug.Log("in scene switch");
        sceneTarget = nameOfScene;
    }

    public void DoSceneChange()
    {
        MusicController.instance.MusicFadeIn();
        SceneManager.LoadScene(sceneTarget);
        
    }


}
