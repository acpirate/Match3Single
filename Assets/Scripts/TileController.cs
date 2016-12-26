using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TILETYPE { RED, BLUE, GREEN, YELLOW, BROWN, PURPLE, WHITE };

public class TileController : MonoBehaviour {

    TILETYPE myType;

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

	// Use this for initialization
	void Start () {
        myMaterial = GetComponent<MeshRenderer>().material;

        Randomize();
	}
	
	// Update is called once per frame
	void Update () {
        if (isSelected) SelectionAnimation();
	}

    private void OnMouseDown()
    {
       // Debug.Log("selected " + myCoords.ToString());
        if (!(isSelected)) {
            isSelected = true;
            originalPosition = transform.position;
        }
        else
        {
            isSelected = false;
            transform.position = originalPosition;
        }
    }

    public void setCoords(int x, int y)
    {
        myCoords = new Coords(x, y);
    }

    void SelectionAnimation()
    {
        float xOffset = Random.Range(-selectionOffset, selectionOffset);
        float yOffset = Random.Range(-selectionOffset, selectionOffset);

        Vector3 newPosition = new Vector3(originalPosition.x + xOffset,
            originalPosition.y, originalPosition.z + yOffset);

        transform.position = newPosition;
    }

    void Randomize()
    {
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


}
