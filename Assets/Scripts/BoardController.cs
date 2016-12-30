using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {

    GameObject selectedTile;

    public GameObject TilePrefab;
    public float boardLeft = -7f;
    public float boardBottom = -7f;
    public float tileSpacing = 2f;

    public MatchController matchController;

    GameObject[,] TileArray;

    // Use this for initialization
	void Start () {
        TileArray = new GameObject[Constants.BOARDSIZE,Constants.BOARDSIZE];
        CreateBoard();
        PreventInitialMatches();

        selectedTile = null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SnapPositionTiles()
    {
        for (int boardX = 0; boardX < Constants.BOARDSIZE; boardX++)
        {
            for (int boardY = 0; boardY < Constants.BOARDSIZE; boardY++)
            {
                //Debug.Log(boardX + " " + boardY);
                float tileX = boardLeft + tileSpacing * boardX;
                float tileZ = boardBottom + tileSpacing * boardY;
                Vector3 tilePosition = new Vector3(tileX, 1, tileZ);

                //Debug.Log(TileArray[boardX, boardY]);

                TileArray[boardX, boardY].transform.position = tilePosition;

            }
        }
    }

    public void CreateBoard()
    {
        //resets the board before it can be created
        foreach (GameObject tile in TileArray)
        {
            if (tile != null) Destroy(tile);
        }


        for (int boardX = 0; boardX < Constants.BOARDSIZE; boardX++)
        {
            for (int boardY = 0; boardY < Constants.BOARDSIZE; boardY++)
            {
                GameObject tempTile = Instantiate(TilePrefab);
                //parent the transform with the board
                tempTile.transform.SetParent(transform);
                //tell the tile its coordinates so it can tell the board when it is seleted
                tempTile.GetComponent<TileController>().setCoords(boardX, boardY);
                //add the tile to the board array
                TileArray[boardX, boardY] = tempTile;

            
           
            }
        }


        SnapPositionTiles();

    }

    void PreventInitialMatches()
    {

        //prevent initial board from having any matches
        List<Match> currentMatches = new List<Match>();
        currentMatches = getBaseMatches();
        //avoid infinite loop
        int initialMatchCounter = 0;
        while (currentMatches.Count > 0)
        {
            initialMatchCounter++;
            foreach (Match match in currentMatches)
            {
                foreach (Coords matchCoords in match.matchCoords)
                {
                    //Debug.Log(matchCoords.x + " ," + matchCoords.y);
                    TileArray[matchCoords.x, matchCoords.y].GetComponent<TileController>().Randomize();
                }
            }

            currentMatches = getBaseMatches();
            //prevent infinte loop
            if (initialMatchCounter > 10) currentMatches = new List<Match>();
        }
        //Debug.Log("loops until no matches " + initialMatchCounter);
    }

    public void ToggleCoords()
    {
        foreach (GameObject tile in TileArray)
        {
            tile.GetComponent<TileController>().ToggleCoordsDisplay();
        }
    }

    public void SetTileTypeFromString(int tileX, int tileY, string typeString)
    {
        TileArray[tileX, tileY].GetComponent<TileController>().SetTileTypeFromString(typeString);
    }

    public GameObject getTileAtCoords(Coords tileCoords)
    {

        return TileArray[tileCoords.x, tileCoords.y];

    }

    public List<Match> getBaseMatches()
    {
        return matchController.getBaseMatches();
    }

    public List<GameObject> GetNeighbors(GameObject queriedPiece)
    {
        List<GameObject> returnNeighbors = new List<GameObject>();

        Coords pieceIndex = GetIndexOf(queriedPiece);

        //south neighbor
        if (pieceIndex.y > 0) returnNeighbors.Add(TileArray[pieceIndex.x, pieceIndex.y - 1]);
        //north neighbor
        if (pieceIndex.y < Constants.BOARDSIZE - 1) returnNeighbors.Add(TileArray[pieceIndex.x, pieceIndex.y + 1]);
        //east neighbor
        if (pieceIndex.x > 0) returnNeighbors.Add(TileArray[pieceIndex.x - 1, pieceIndex.y]);
        //west neighbor
        if (pieceIndex.x < Constants.BOARDSIZE - 1) returnNeighbors.Add(TileArray[pieceIndex.x + 1, pieceIndex.y]);


        //Debug.Log(returnNeighbors.Count);

        return returnNeighbors;
    }


    public Coords GetIndexOf(GameObject queriedPiece)
    {
        Coords returnCoords = new Coords(-1, -1);


        for (int colCounter = 0; colCounter < Constants.BOARDSIZE; colCounter++)
        {
            for (int rowCounter = 0; rowCounter < Constants.BOARDSIZE; rowCounter++)
            {
                if (TileArray[colCounter, rowCounter] == queriedPiece)
                {
                    returnCoords.x = colCounter;
                    returnCoords.y = rowCounter;
                    break;
                }
                if (returnCoords.x != -1 && returnCoords.y != -1) break;
            }
        }
        //sanity checking
        if (returnCoords.x == -1 && returnCoords.y == -1)
        {
            Debug.LogError("tried to get the index of a piece that isn't on the board");
            Application.Quit();
        }

        return returnCoords;
    }

    public void TileSelected(GameObject clickedTile)
    {
        //no tile selected
        if (selectedTile == null) {
            selectedTile = clickedTile;
        }
        //a tile is selected
        else
        {
            //clicked tile is next to currently selected tile
            if (GetNeighbors(clickedTile).Contains(selectedTile))
            {
                Debug.Log("valid swap!");
                selectedTile.GetComponent<TileController>().ToggleSelected();
                clickedTile.GetComponent<TileController>().ToggleSelected();
                selectedTile = null;
            }
            //clicked tile is not next to currently selected tile
            else
            {
                selectedTile.GetComponent<TileController>().ToggleSelected();
                selectedTile = clickedTile;
            }
        }

    }
}
