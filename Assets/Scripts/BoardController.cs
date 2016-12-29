using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour {

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
}
