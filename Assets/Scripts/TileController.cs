using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TILETYPE { RED, BLUE, GREEN, YELLOW, BROWN, PURPLE, WHITE, NONE };

public class TileController : MonoBehaviour {

    Text myCoordDisplay;

    public TILETYPE myType;

    Vector3 originalPosition;
    bool isSelected = false;
    float selectionOffset = .1f;


    public Color redTile;
    public Color blueTile;
    public Color greenTile;
    public Color yellowTile;
    public Color brownTile;
    public Color purpleTile;
    public Color whiteTile;

    Material myMaterial;
    Coords myCoords;

    BoardController boardController;

    void Awake()
    {
        myCoordDisplay = GetComponentInChildren<Text>();
        myMaterial = GetComponent<MeshRenderer>().material;

        boardController = GameObject.FindGameObjectWithTag("Board").GetComponent<BoardController>();

        Randomize();
    }


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (isSelected) SelectionAnimation();
	}

    private void OnMouseDown()
    {
        // Debug.Log("selected " + myCoords.ToString());
        ToggleSelected();
        boardController.TileSelected(gameObject);
    }

    public void ToggleSelected()
    {
        if (!(isSelected))
        {
            isSelected = true;
            originalPosition = transform.position;
        }
        else
        {
            isSelected = false;
            transform.position = originalPosition;
        }
    }

    public void ToggleCoordsDisplay()
    {
        if (myCoordDisplay.text != "")
        {
            myCoordDisplay.text = ""; }
        else {
            myCoordDisplay.text = myCoords.ToString();
        }
    }

    public void setCoords(int x, int y)
    {
        myCoords = new Coords(x, y);
        //myCoordDisplay.text = myCoords.ToString();
    }

    void SelectionAnimation()
    {
        float xOffset = Random.Range(-selectionOffset, selectionOffset);
        float yOffset = Random.Range(-selectionOffset, selectionOffset);

        Vector3 newPosition = new Vector3(originalPosition.x + xOffset,
            originalPosition.y, originalPosition.z + yOffset);

        transform.position = newPosition;
    }

    public void Randomize()
    {
        myType = TILETYPE.NONE;

        while (myType.Equals(TILETYPE.NONE))
            myType = HelperObjects.GetRandomEnum<TILETYPE>();

        switch (myType)
        {
            case TILETYPE.BLUE:
                myMaterial.color = blueTile;
                break;
            case TILETYPE.BROWN:
                myMaterial.color = brownTile;
                break;
            case TILETYPE.GREEN:
                myMaterial.color = greenTile;
                break;
            case TILETYPE.PURPLE:
                myMaterial.color = purpleTile;
                break;
            case TILETYPE.RED:
                myMaterial.color = redTile;
                break;
            case TILETYPE.WHITE:
                myMaterial.color = whiteTile;
                break;
            case TILETYPE.YELLOW:
                myMaterial.color = yellowTile;
                break;
            default:
                Debug.Log("Invalid tile type " + myType.ToString());
                Application.Quit();
                break;
        }
    }
        
    public void SetTileTypeFromString(string tileString)
    {
        if (tileString.Equals("BLUE"))
        {
            myType = TILETYPE.BLUE;
            myMaterial.color = blueTile;
        }
        if (tileString.Equals("BROWN"))
        {
            myType = TILETYPE.BROWN;
            myMaterial.color = brownTile;
        }
        if (tileString.Equals("GREEN"))
        {
            myType = TILETYPE.GREEN;
            myMaterial.color = greenTile;
        }
        if (tileString.Equals("PURPLE"))
        {
            myType = TILETYPE.PURPLE;
            myMaterial.color = purpleTile;
        }
        if (tileString.Equals("RED"))
        {
            myType = TILETYPE.RED;
            myMaterial.color = redTile;
        }
        if (tileString.Equals("WHITE"))
        {
            myType = TILETYPE.WHITE;
            myMaterial.color = whiteTile;
        }
        if (tileString.Equals("YELLOW"))
        {
            myType = TILETYPE.YELLOW;
            myMaterial.color = yellowTile;
        }


    }


}
