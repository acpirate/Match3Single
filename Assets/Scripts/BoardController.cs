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

    float tileReturnDelay = .3f;
    float tileReturnTime;

    //gameboard
    GameObject[,] TileArray;
    //call for snap tile position
    bool callSnap = false;

    // Use this for initialization
	void Start () {
        TileArray = new GameObject[Constants.BOARDSIZE,Constants.BOARDSIZE];
        CreateBoard();
        PreventInitialMatches();

        selectedTile = null;
	}
	
	// Update is called once per frame
	void Update () {
	    if (GameController.gameState == GameState.CANTCLICK && Time.time > tileReturnTime)
        {
            callSnap = true;
            GameController.gameState = GameState.CANCLICK;
        }	
	}

    void LateUpdate()
    {
      if (callSnap)
        {
            SnapPositionTiles();
            callSnap = false;
        }
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
                Vector3 tilePosition = CalculateWorldPosition(boardX, boardY);

                //Debug.Log(TileArray[boardX, boardY]);

                TileArray[boardX, boardY].transform.position = tilePosition;
                TileArray[boardX, boardY].transform.rotation = Quaternion.identity;
                TileArray[boardX, boardY].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                TileArray[boardX, boardY].GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    public void SnapPositionIndividualTile(GameObject tileToSnap)
    {
        Coords tileCoords = GetIndexOf(tileToSnap);
        float tileX = boardLeft + tileSpacing * tileCoords.x;
        float tileZ = boardLeft + tileSpacing * tileCoords.y;

        Vector3 tilePosition = new Vector3(tileX, 1, tileZ);
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
            //    tempTile.transform.SetParent(transform);
                //tell the tile its coordinates so it can tell the board when it is seleted
                tempTile.GetComponent<TileController>().setCoords(new Coords(boardX, boardY));
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
                    //Debug.Log(matchCoords.x + " ," + matchCoords.y);
                    TileArray[matchCoords.x, matchCoords.y].GetComponent<TileController>().Randomize();
                }
            }

            currentMatches = GetBaseMatches();
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

    //returns the tile object as the array coordinates
    public GameObject GetTileAtCoords(Coords tileCoords)
    {

        return TileArray[tileCoords.x, tileCoords.y];

    }

    //returns the matches of 3+ in a straight line
    public List<Match> GetBaseMatches()
    {
        return matchController.getBaseMatches();
    }

    //returns up, down, left right neighbors of the input tile as a list
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

    //returns the coordinates of the chosen tile
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


    //attempts to select a tile
    //if the a neighbor tile is already selected a swap will be attempted
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
                HandleSwap(clickedTile, selectedTile);
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

    public void HandleSwap(GameObject tile1, GameObject tile2)
    {
    //    Debug.Log("tile1 coords " + GetIndexOf(tile1).ToString() + " tile2 coords " + GetIndexOf(tile2).ToString());


        TileController tile1Controller = tile1.GetComponent<TileController>();
        TileController tile2Controller = tile2.GetComponent<TileController>();

        GameObject tempTile = tile1;

        Coords tile1Coords = GetIndexOf(tile1);
        Coords tile2Coords = GetIndexOf(tile2);

        //switch tiles in board
        TileArray[tile1Coords.x, tile1Coords.y] = tile2;
        tile2Controller.setCoords(tile1Coords);
        TileArray[tile2Coords.x, tile2Coords.y] = tempTile;
        tile1Controller.setCoords(tile2Coords);
        //reset tile positions
        //callSnap = true;
        tile1.GetComponent<Rigidbody>().AddExplosionForce(500f, new Vector3(0, -.5f, 0f), 0f);
        tile2.GetComponent<Rigidbody>().AddExplosionForce(500f, new Vector3(0, -.5f, 0f), 0f);
        GameController.gameState = GameState.CANTCLICK;
        tileReturnTime = Time.time + tileReturnDelay;
    }


    public Vector3 CalculateWorldPosition(int col, int row)
    {
        float xPosition = boardLeft + col * tileSpacing;
        float yPosition = boardBottom + row * tileSpacing;

        Vector3 piecePosition = new Vector3(xPosition, 1f, yPosition);

        return piecePosition;
    }
}
