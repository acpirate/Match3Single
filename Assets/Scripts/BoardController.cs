﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardController : MonoBehaviour {

    GameObject selectedTile;

    public GameObject TilePrefab;

    public float boardLeft = -7f;
    public float boardBottom = -7f;
    public float tileSpacing = 2f;

    public MatchController matchController;
    public GameObject SwapAnimPrefab;
    public GameObject MoveAnimPrefab;

    //float tileReturnDelay = .3f;
    float endAnimTime;

    Swap triedSwap;
    Swap nullSwap;

    //gameboard
    GameObject[,] TileArray;
    //call for snap tile position
    bool callSnap = false;

    // Use this for initialization
    void Start() {
        nullSwap = new Swap(new Coords(-9999, -9999), new Coords(-9999, -9999));
        triedSwap = nullSwap;

        TileArray = new GameObject[Constants.BOARDSIZE, Constants.BOARDSIZE];

        int possibleMatches = 0;
        while (possibleMatches<1)
        { 
            CreateBoard();
            PreventInitialMatches();
            possibleMatches = PossibleMatches().Count;
        }

        selectedTile = null;
	}
	
	// Update is called once per frame
	void Update () {
	    if (GameController.gameState == GAMESTATE.TRYSWAP && Time.time > endAnimTime)
        {
            //callSnap = true;
            CheckMatches();
        }	
        if (GameController.gameState == GAMESTATE.RETURNSWAP && Time.time > endAnimTime)
        {
            callSnap = true;
            GameController.gameState = GAMESTATE.CANSELECT;
        }
        if (GameController.gameState == GAMESTATE.REPLACEMATCHES && Time.time > endAnimTime)
        {
            GameController.gameState = GAMESTATE.AFTERMATCH;
            AfterMatchCheck();
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

    //checks to see if new matches are created after new tiles are added and current
    //tiles fall
    void AfterMatchCheck()
    {
        if (GetBaseMatches().Count > 0)
        {
            triedSwap = nullSwap;
            RemoveMatchedTiles();
        }
        else
        {
            NoPossibleMatchesCheck();
            GameController.gameState = GAMESTATE.CANSELECT;
            callSnap = true;
        }
    }

    void NoPossibleMatchesCheck()
    {
        if (PossibleMatches().Count<1)
        {
            SceneManager.LoadScene("GameOver");
        }

        //if there aren't any possible matches
        //go to game over scene
    }

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
                    MakeSwap(TileArray[colCounter, rowCounter], TileArray[colCounter + 1, rowCounter]);
                    if (GetBaseMatches().Count > 0)
                    {
                        Swap tempSwap;
                        tempSwap.piece1Coords = new Coords(colCounter, rowCounter);
                        tempSwap.piece2Coords = new Coords(colCounter + 1, rowCounter);
                        foundMatches.Add(tempSwap);
                    }
                    MakeSwap(TileArray[colCounter, rowCounter], TileArray[colCounter + 1, rowCounter]);
                }
                //south swap
                if (rowCounter < Constants.BOARDSIZE - 1)
                {
                    MakeSwap(TileArray[colCounter, rowCounter], TileArray[colCounter, rowCounter + 1]);
                    if (GetBaseMatches().Count > 0)
                    {
                        Swap tempSwap;
                        tempSwap.piece1Coords = new Coords(colCounter, rowCounter);
                        tempSwap.piece2Coords = new Coords(colCounter, rowCounter + 1);
                        foundMatches.Add(tempSwap);
                    }
                    MakeSwap(TileArray[colCounter, rowCounter], TileArray[colCounter, rowCounter + 1]);
                }
            }
        }


        return foundMatches;
    }

    public void MakeSwap(GameObject piece1, GameObject piece2)
    {
        Coords piece1Coords = GetIndexOf(piece1);
        Coords piece2Coords = GetIndexOf(piece2);

        GameObject tempPiece = piece1;
        TileArray[piece1Coords.x, piece1Coords.y] = piece2;
        TileArray[piece2Coords.x, piece2Coords.y] = tempPiece;

        //SnapToWorldPosition(piece1Coords.x,piece1Coords.y);
        //SnapToWorldPosition(piece2Coords.x,piece2Coords.y);

    }

    void RemoveMatchedTiles()
    {
        endAnimTime = Time.time + Constants.MOVEANIMATIONTIME + .01f;
        GameController.gameState = GAMESTATE.REPLACEMATCHES;
        foreach (Match match in GetBaseMatches())
        {
            foreach(Coords coord in match.matchCoords)
            {
                TileArray[coord.x, coord.y].GetComponent<TileController>().RemoveForMatch();
            }
        }
        MoveTilesDown();
        //SnapPositionTiles();
        ReplaceRemovedTiles();

    }

    void MoveTilesDown()
    {
        for (int col = 0; col<Constants.BOARDSIZE; col++)
        {
            int missingCounter = 0;
            for(int row = 0; row<Constants.BOARDSIZE; row++)
            {
                if (TileArray[col, row].GetComponent<TileController>().dead)
                {
                  //  Debug.Log("tile missing at " + col + "," + row);
                    missingCounter++;
                  //  Debug.Log("missing counter is " + missingCounter);
                    TileArray[col, row] = null;
                }
                else
                {
                    if (missingCounter>0)
                    {
                    //    Debug.Log("moving tile " + col + "," + row + "down");
                        GameObject tempTile = TileArray[col, row];

                        TileArray[col, row - missingCounter] = tempTile;
                        tempTile.GetComponent<TileController>().setCoords(new Coords(col, row - missingCounter));
                     //   Debug.Log("new coods for moved tile are " + col + "," + (row-missingCounter));
                        TileArray[col, row] = null;
                        GameObject moveAnim = Instantiate(MoveAnimPrefab, tempTile.transform.position, Quaternion.identity);
                        tempTile.transform.parent = moveAnim.transform;
                        moveAnim.GetComponent<MoveAnimationScript>().SetMoveTarget(CalculateWorldPosition(col, row - missingCounter));
                    }
                }
            }
        }
    }

    void ReplaceRemovedTiles()
    {
        for (int col = 0; col<Constants.BOARDSIZE; col++)
        {
            for (int row =0; row<Constants.BOARDSIZE; row++)
            {
                if (TileArray[col, row]==null)
                {
                    SpawnReplacedTile(col, row);
                }
            }
        }
        //SnapPositionTiles();
        //GameController.gameState = GAMESTATE.CANSELECT;
        triedSwap = nullSwap;
    }

    void SpawnReplacedTile(int col, int row)
    {
        GameObject tempTile = Instantiate(TilePrefab);
        tempTile.GetComponent<TileController>().setCoords(new Coords(col, row));
        TileArray[col, row] = tempTile;

        Vector3 worldCoords = CalculateWorldPosition(col, row);
        Vector3 tileSpawnLocation = new Vector3(worldCoords.x, Constants.REPLACETILESTARTHEIGHT, worldCoords.z);

        tempTile.transform.position = tileSpawnLocation;
        GameObject moveAnim = Instantiate(MoveAnimPrefab, tempTile.transform.position, Quaternion.identity);
        moveAnim.GetComponent<MoveAnimationScript>().SetMoveTarget(worldCoords);
        tempTile.transform.SetParent(moveAnim.transform);
    }


    //check to see if there are any matches after a swap
    void CheckMatches()
    {
        //failed to find any matches
        if (matchController.getBaseMatches().Count<1)
        {
            ReturnSwap();   
        }
        else
        {
            RemoveMatchedTiles();
        }
    }

    //return tiles if there is no match
    void ReturnSwap()
    {
        HandleSwap(TileArray[triedSwap.piece1Coords.x, triedSwap.piece1Coords.y],
            TileArray[triedSwap.piece2Coords.x, triedSwap.piece2Coords.y]);
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

                if (TileArray[boardX, boardY] != null)
                {
                    TileArray[boardX, boardY].transform.position = tilePosition;
                    TileArray[boardX, boardY].transform.rotation = Quaternion.identity;
                }
                //TileArray[boardX, boardY].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
                //TileArray[boardX, boardY].GetComponent<Rigidbody>().velocity = Vector3.zero;
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
                //fire the handle swap method
                HandleSwap(selectedTile, clickedTile);
                //unselect the tiles
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
        //Debug.Log("tile1 coords " + GetIndexOf(tile1).ToString() + " tile2 coords " + GetIndexOf(tile2).ToString());

        TileController tile1Controller = tile1.GetComponent<TileController>();
        TileController tile2Controller = tile2.GetComponent<TileController>();

        GameObject tempTile = tile1;

        Coords tile1Coords = GetIndexOf(tile1);
        Coords tile2Coords = GetIndexOf(tile2);

        //board logic, switch tiles in board and tell the tile what its new coordinates are
        TileArray[tile1Coords.x, tile1Coords.y] = tile2;
        tile2Controller.setCoords(tile1Coords);
        TileArray[tile2Coords.x, tile2Coords.y] = tempTile;
        tile1Controller.setCoords(tile2Coords);

        //set the position of the swap handler object between the tiles
        Vector3 swapHandlerPostion = (tile1.transform.position + tile2.transform.position) / 2;

        //instantiate the swap handler
        GameObject swapHandler = Instantiate(SwapAnimPrefab, swapHandlerPostion, Quaternion.identity);

        //parent the tiles to the swap handler
        tile1.transform.parent = swapHandler.transform;
        tile2.transform.parent = swapHandler.transform;
       

        //create the tried swap
        if (triedSwap.Equals(nullSwap))
        {
            triedSwap = new Swap(tile1Coords, tile2Coords);
            GameController.gameState = GAMESTATE.TRYSWAP;
        }
        else
        {
            GameController.gameState = GAMESTATE.RETURNSWAP;
        }
        //determine swap direction
        SWAPDIRECTION swapDirection = triedSwap.SwapType();

        if (GameController.gameState == GAMESTATE.RETURNSWAP)
        {
            swapDirection = new Swap(triedSwap.piece2Coords, triedSwap.piece1Coords).SwapType();
            triedSwap = nullSwap;

        }
        //set the correct direction in the swap handler
        swapHandler.GetComponent<SwapAnimationScript>().myDirection = swapDirection;

        endAnimTime = Time.time + Constants.SWAPANIMATIONTIME;
    }


    public Vector3 CalculateWorldPosition(int col, int row)
    {
        float xPosition = boardLeft + col * tileSpacing;
        float yPosition = boardBottom + row * tileSpacing;

        Vector3 piecePosition = new Vector3(xPosition, 1f, yPosition);

        return piecePosition;
    }
}
