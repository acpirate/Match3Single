using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardController : MonoBehaviour {

    public static BoardController Instance;

    GameObject selectedTile;

    public GameObject TilePrefab;

    public float boardLeft = -7f;
    public float boardBottom = -7f;
    public float tileSpacing = 2f;

    public MatchController matchController;
    public GameObject SwapAnimPrefab;
    public GameObject MoveAnimPrefab;

    //float tileReturnDelay = .3f;


    public Swap triedSwap;
    public Swap nullSwap;

    //gameboard
    GameObject[,] TileArray;
    //call for snap tile position
    public bool callSnap = false;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start() {
        nullSwap = new Swap(new Coords(-9999, -9999), new Coords(-9999, -9999));
        triedSwap = nullSwap;

        TileArray = new GameObject[Constants.BOARDSIZE, Constants.BOARDSIZE];


        selectedTile = null;
	}
	
	// Update is called once per frame
	void Update () {

	}

    void LateUpdate()
    {
      if (callSnap)
        {
            SnapPositionTiles();
            callSnap = false;
        }
    }
    
 


    public void MakeSwap(Coords tile1Coords, Coords tile2Coords)
    {
        GameObject tile1Holder = TileArray[tile1Coords.x, tile1Coords.y];
        GameObject tile2Holder = TileArray[tile2Coords.x, tile2Coords.y];

        GameObject tempTile = tile1Holder;
        TileArray[tile1Coords.x, tile1Coords.y] = tile2Holder;
        TileArray[tile2Coords.x, tile2Coords.y] = tempTile;
        
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

    public void RemoveMatchedTiles()
    {
        GameController.Instance.endAnimTime = Time.time + Constants.MOVEANIMATIONTIME + .01f;
        GameController.Instance.gameState = GAMESTATE.REPLACEMATCHES;
        foreach (Match match in GameController.Instance.GetMatches())
        {
            //score the match
            GameController.Instance.ScoreMatch(match);
            //remove the match
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




    //return tiles if there is no match
    public void ReturnSwap()
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
    //move individual tile
    public void SnapPositionIndividualTile(GameObject tileToSnap)
    {
        Coords tileCoords = GetIndexOf(tileToSnap);
        float tileX = boardLeft + tileSpacing * tileCoords.x;
        float tileZ = boardLeft + tileSpacing * tileCoords.y;

        Vector3 tilePosition = new Vector3(tileX, 1, tileZ);

        tileToSnap.transform.position = tilePosition;
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
        TileController clickedTileController = clickedTile.GetComponent<TileController>();

        //no tile selected
        if (selectedTile == null) {
            selectedTile = clickedTile;
            clickedTileController.SetSelected(true);
            return;
        }
        //a tile is selected
        else
        {
            TileController selectedTileController = selectedTile.GetComponent<TileController>();

     
            //test to see if selected tile was clicked
            if (selectedTileController.getCoords().Equals(clickedTileController.getCoords()))
            {
                selectedTile = null;
                clickedTileController.SetSelected(false);
                return;
            }
            //clicked tile is next to currently selected tile
            if (GetNeighbors(clickedTile).Contains(selectedTile))
            {
                //fire the handle swap method
                HandleSwap(selectedTile, clickedTile);
                //unselect the tiles
                selectedTileController.SetSelected(false);
                clickedTileController.SetSelected(true);
                clickedTileController.SetSelected(false);
                selectedTile = null;
            }
            //clicked tile is not next to currently selected tile
            else
            {
                selectedTileController.SetSelected(false);
                clickedTileController.SetSelected(true);
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
            GameController.Instance.gameState = GAMESTATE.TRYSWAP;
        }
        else
        {
            GameController.Instance.gameState = GAMESTATE.RETURNSWAP;
        }
        //determine swap direction
        SWAPDIRECTION swapDirection = triedSwap.SwapType();

        if (GameController.Instance.gameState == GAMESTATE.RETURNSWAP)
        {
            swapDirection = new Swap(triedSwap.piece2Coords, triedSwap.piece1Coords).SwapType();
            triedSwap = nullSwap;

        }
        //set the correct direction in the swap handler
        swapHandler.GetComponent<SwapAnimationScript>().myDirection = swapDirection;

        GameController.Instance.endAnimTime = Time.time + Constants.SWAPANIMATIONTIME;
    }


    public Vector3 CalculateWorldPosition(int col, int row)
    {
        float xPosition = boardLeft + col * tileSpacing;
        float yPosition = boardBottom + row * tileSpacing;

        Vector3 piecePosition = new Vector3(xPosition, 1f, yPosition);

        return piecePosition;
    }

    public void RandomizeTile(int col, int row)
    {

        //Debug.Log(matchCoords.x + " ," + matchCoords.y);
        TileArray[col, row].GetComponent<TileController>().Randomize();
    }

    public Vector3 CalculateMatchCenter(Match inMatch)
    {
        Vector3 matchCenter = Vector3.zero;

        foreach (Coords coords in inMatch.matchCoords)
        {
            matchCenter += CalculateWorldPosition(coords.x, coords.y);
        }

        matchCenter = matchCenter / inMatch.matchCoords.Count;

        return matchCenter;
    }
}
