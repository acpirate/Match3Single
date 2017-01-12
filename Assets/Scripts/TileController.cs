using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TileController : MonoBehaviour {

    Text myCoordDisplay;

    public GameObject RemoveTileVFXPrefab;

    public TILETYPE myType;

    Vector3 originalPosition;
    bool isSelected = false;
    float selectionOffset = .1f;

    public bool dead = false;


    public Color redTile;
    public Color blueTile;
    public Color greenTile;
    public Color yellowTile;
    public Color brownTile;
    public Color purpleTile;
    public Color whiteTile;

    Material myMaterial;
    Coords myCoords;

    void Awake()
    {
        myCoordDisplay = GetComponentInChildren<Text>();
        myMaterial = GetComponent<MeshRenderer>().material;


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
        if (GameController.Instance.gameState == GAMESTATE.CANSELECT)
        {
            ToggleSelected();
            BoardController.Instance.TileSelected(gameObject);
        }
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

    public void setCoords(Coords inCoords)
    {
        myCoords = inCoords;
        if (HelperObjects.showCoords) myCoordDisplay.text = myCoords.ToString();
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

    public void RemoveForMatch()
    {
        GameObject tempVFX = Instantiate(RemoveTileVFXPrefab, transform.position, Quaternion.identity);

        ParticleSystem vfxSystem = tempVFX.GetComponentInChildren<ParticleSystem>();

        vfxSystem.Stop();
        vfxSystem.Clear();


        var main = vfxSystem.main;
        Color tempColor = myMaterial.GetColor("_Color");
        tempColor = new Color(tempColor.r, tempColor.g, tempColor.b, 1);
        main.startColor = tempColor;
       tempVFX.GetComponentInChildren<ParticleSystem>().Play();
        Destroy(tempVFX, 3f);
        dead = true;
        Destroy(gameObject);

    }

}
