using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject ScoreVFXPrefab;

    public static GameController Instance;

    public GAMESTATE gameState = GAMESTATE.CANSELECT;

    public int score = 0;


    public bool dialogConfirm = false;
    public bool dialogDeny = false;

    static readonly int match3Score = 10;
    static readonly int match4Score = 15;
    static readonly int matchBlobScore = 20;

    public float endAnimTime;

    public int chain = 0;

    void Awake()
    {
        //fake singleton pattern
        Instance = this;
    }

	// Use this for initialization
	void Start () {

        int possibleMatches = 0;
        while (possibleMatches < 1)
        {
            BoardController.Instance.CreateBoard();
            PreventInitialMatches();
            possibleMatches = PossibleMatches().Count;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (gameState != GAMESTATE.CANSELECT) GameStateAction();
    }

    //add score to score total
    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;
        UIController.Instance.ShowScore(score);
    }

    //calculate the score of a match
    public void ScoreMatch(Match matchToScore)
    {
        //ScoreVFX(matchToScore);
        int calculatedScore = 0;

        if (matchToScore.matchCoords.Count==3)
        {
            calculatedScore = match3Score * 3;
        }
        if (matchToScore.matchCoords.Count==4)
        {
            calculatedScore = match4Score * 4;
        }
        if (matchToScore.matchCoords.Count > 4)
        {
            calculatedScore = matchToScore.matchCoords.Count * matchBlobScore;
        }

        //calculate chain multipler
        calculatedScore = Mathf.RoundToInt(calculatedScore * (1 + Constants.CHAINMATCHMULTIPLIER * chain));

        ScoreVFX(matchToScore, calculatedScore.ToString());
        AddScore(calculatedScore);
    }

    //generate animated score
    void ScoreVFX(Match inMatch, string inScore)
    {
        Vector3 scoreVFXSpawnLocation = BoardController.Instance.CalculateMatchCenter(inMatch);

        GameObject scoreVFX = Instantiate(ScoreVFXPrefab, scoreVFXSpawnLocation, Quaternion.identity);

        Color scoreColor = BoardController.Instance.GetTileAtCoords(inMatch.matchCoords[0]).GetComponent<TileController>().GetColor();

        scoreVFX.GetComponent<ScoreVFXController>().Initialize(inScore, scoreColor);
    }


    //end the game
    public void EndGame()
    {
        HandleHighScore();
        SceneManager.LoadScene("GameOver");

    }

    //prevent the board from having any matches
    void PreventInitialMatches()
    {

        //prevent initial board from having any matches
        List<Match> currentMatches = new List<Match>();
        currentMatches = GetBaseMatches();
        //avoid infinite loop
        int initialMatchCounter = 0;
        while (currentMatches.Count > 0)
        {
            initialMatchCounter++;
            foreach (Match match in currentMatches)
            {
                foreach (Coords matchCoords in match.matchCoords)
                {
                    BoardController.Instance.RandomizeTile(matchCoords.x, matchCoords.y);
                }
            }

            currentMatches = GetBaseMatches();
            //prevent infinte loop
            if (initialMatchCounter > 10) currentMatches = new List<Match>();
        }
        //Debug.Log("loops until no matches " + initialMatchCounter);
    }


    public List<Match> GetMatches()
    {
        List<Match> baseMatches = GetBaseMatches();
        List<Match> calculatedMatches = MatchController.Instance.DesignateBlobMatches(baseMatches);

        return calculatedMatches;
    }

    //returns the matches of 3+ in a straight line
    public List<Match> GetBaseMatches()
    {
        return MatchController.Instance.getBaseMatches();
    }

    //returns list of possible matches
    public List<Swap> PossibleMatches()
    {
        List<Swap> foundMatches = new List<Swap>();

        for (int colCounter = 0; colCounter < Constants.BOARDSIZE; colCounter++)
        {
            for (int rowCounter = 0; rowCounter < Constants.BOARDSIZE; rowCounter++)
            {
                //east swap check
                if (colCounter < Constants.BOARDSIZE - 1)
                {
                    BoardController.Instance.MakeSwap(new Coords(colCounter, rowCounter), new Coords(colCounter + 1, rowCounter));
                    if (GetBaseMatches().Count > 0)
                    {
                        Swap tempSwap;
                        tempSwap.piece1Coords = new Coords(colCounter, rowCounter);
                        tempSwap.piece2Coords = new Coords(colCounter + 1, rowCounter);
                        foundMatches.Add(tempSwap);
                    }
                    BoardController.Instance.MakeSwap(new Coords(colCounter, rowCounter), new Coords(colCounter + 1, rowCounter));
                }
                //south swap
                if (rowCounter < Constants.BOARDSIZE - 1)
                {
                    BoardController.Instance.MakeSwap(new Coords(colCounter, rowCounter), new Coords(colCounter, rowCounter + 1));

                    if (GetBaseMatches().Count > 0)
                    {
                        Swap tempSwap;
                        tempSwap.piece1Coords = new Coords(colCounter, rowCounter);
                        tempSwap.piece2Coords = new Coords(colCounter, rowCounter + 1);
                        foundMatches.Add(tempSwap);
                    }
                    BoardController.Instance.MakeSwap(new Coords(colCounter, rowCounter), new Coords(colCounter, rowCounter + 1));

                }
            }
        }


        return foundMatches;
    }

    //gamestate iteration
    void GameStateAction()
    {
        if (GameController.Instance.gameState == GAMESTATE.TRYSWAP && Time.time > endAnimTime)
        {
           // BoardController.Instance.callSnap = true;
            CheckMatches();
        }
        if (GameController.Instance.gameState == GAMESTATE.RETURNSWAP && Time.time > endAnimTime)
        {
            //BoardController.Instance.callSnap = true;
            GameController.Instance.gameState = GAMESTATE.CANSELECT;
        }
        if (GameController.Instance.gameState == GAMESTATE.REPLACEMATCHES && Time.time > endAnimTime)
        {
            GameController.Instance.gameState = GAMESTATE.AFTERMATCH;
            AfterMatchCheck();
        }
        if (GameController.Instance.gameState == GAMESTATE.MATCHCASCADEDELAY && Time.time > endAnimTime)
        {
            BoardController.Instance.RemoveMatchedTiles();
        }
        if (GameController.Instance.gameState == GAMESTATE.QUITCONFIRM)
        {
            QuitDialogCheck();
        }
    }

    //action when quit dialog box is active
    void QuitDialogCheck()
    {
        if (dialogConfirm) EndGame();
        if (dialogDeny)
        {
            dialogDeny = false;
            UIController.Instance.QuitDialogDeny();
        }
    }
    

    //checks to see if new matches are created after new tiles are added and current
    //tiles fall
    void AfterMatchCheck()
    {
        if (GetBaseMatches().Count > 0)
        {
            BoardController.Instance.triedSwap = BoardController.Instance.nullSwap;
            GameController.Instance.gameState = GAMESTATE.MATCHCASCADEDELAY;
            endAnimTime = Time.time + Constants.MATCHCASCADEDELAY;
            //after the match increase the chain count;
            chain++;
            //RemoveMatchedTiles();
        }
        else
        {
            //set chain count to zero if there are no possible matches after a match
            chain = 0;
            NoPossibleMatchesCheck();
            GameController.Instance.gameState = GAMESTATE.CANSELECT;
            BoardController.Instance.callSnap = true;
        }
    }

    void NoPossibleMatchesCheck()
    {
        if (PossibleMatches().Count < 1)
        {
            GameController.Instance.EndGame();
        }

        //if there aren't any possible matches
        //go to game over scene
    }

    //check to see if there are any matches after a swap
    void CheckMatches()
    {
        //failed to find any matches
        if (MatchController.Instance.getBaseMatches().Count < 1)
        {
            BoardController.Instance.ReturnSwap();
        }
        else
        {
            BoardController.Instance.RemoveMatchedTiles();
        }
    }

    //HighScore

    void HandleHighScore()
    {
        if (!PlayerPrefs.HasKey(Constants.MODE1HIGHSCOREPREF))
        {
            SetHighScore(score);
        }
        else
        {
            if (score > PlayerPrefs.GetInt(Constants.MODE1HIGHSCOREPREF))
            {
                SetHighScore(score);
            }       

        }
    }

    void SetHighScore(int scoreToSet)
    {
        PlayerPrefs.SetInt(Constants.MODE1HIGHSCOREPREF, scoreToSet);
    }
}
