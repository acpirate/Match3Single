using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Naming convention: 
/// Class methods upper case start: ThisIsAClassMethodName
/// Enumerated types names all caps : THISISANENUMTYPENAME
/// Enumerated values all caps: THISISANENUMVALUE
/// Class member variables camel case: thisIsAClassVariable
/// </summary>


    //enums here
public enum SWAPDIRECTION { UP, DOWN, LEFT, RIGHT};

public enum TILETYPE { RED, BLUE, GREEN, YELLOW, BROWN, PURPLE, WHITE, NONE };

public enum GAMESTATE { CANSELECT, CANTSELECT, TRYSWAP,
    RETURNSWAP, REPLACEMATCHES, AFTERMATCH, MATCHCASCADEDELAY,
    QUITCONFIRM, PAUSEANIM, PAUSED, UNPAUSEANIM};

//static helper methods

public static class HelperObjects  {

    public static bool showCoords = false;

    //get a random item of the given type
    public static T GetRandomEnum<T>()
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(0, A.Length));
        return V;
    }

}


//data structures

public struct Coords
{
    public int x;
    public int y;

    //plus operator allows adding two coordinates together
    public static Coords operator +(Coords c1, Coords c2)
    {
        return new Coords(c1.x + c2.x, c1.y + c2.y);
    }
    //== operator returns true if the coordinates have the same value
    public static bool operator ==(Coords c1, Coords c2)
    {
        bool returnValue = false;
        if (c1.x == c2.x && c1.y == c2.y)
        {
            returnValue = true;
        }
        return returnValue;
    }
    //!= operator returns true if the coordinates do not have the same value
    public static bool operator !=(Coords c1, Coords c2)
    {
        bool returnValue = true;
        if (c1.x == c2.x && c1.y == c2.y)
        {
            returnValue = false;
        }
        return returnValue;
    }
    //overriding == and != operators require this
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Coords coordsToCompareWith = (Coords)obj;

        return (coordsToCompareWith.x == x && coordsToCompareWith.y == y);
    }

    //overriding gethash code, required for overriding equals
    public override int GetHashCode()
    {
        return x ^ y;
    }


    // Override the ToString
    public override string ToString()
    {
        return (x.ToString() + "," + y.ToString());
    }


    public Coords(int inX, int inY)
    {
        x = inX;
        y = inY;
    }

    public string CoordString()
    {
        return x.ToString() + "," + y.ToString();
    }
}


public struct Match
{
    public List<Coords> matchCoords;
    public TILETYPE matchType;

    public Match(TILETYPE inShape)
    {
        matchCoords = new List<Coords>();
        matchType = inShape;

    }

    // Override the ToString
    public override string ToString()
    {
        string matchString = "";

        matchString += "- " + matchCoords.Count.ToString() + " piece match of type " + matchType.ToString() + "\n";
        matchString += "- Coords: ";

        foreach (Coords coords in matchCoords)
        {
            matchString += coords.ToString() + "|";
        }

        return matchString;
    }
}

public struct Swap
{
    public Coords piece1Coords;
    public Coords piece2Coords;

    public Swap (Coords piece1InCoords, Coords piece2InCoords)
    {
        piece1Coords = piece1InCoords;
        piece2Coords = piece2InCoords;
    }

    public SWAPDIRECTION SwapType()
    {
        SWAPDIRECTION myType = SWAPDIRECTION.UP;

        if (piece1Coords.x > piece2Coords.x) myType = SWAPDIRECTION.LEFT;
        if (piece1Coords.x < piece2Coords.x) myType = SWAPDIRECTION.RIGHT;
        if (piece1Coords.y > piece2Coords.y) myType = SWAPDIRECTION.UP;
        if (piece1Coords.y < piece2Coords.y) myType = SWAPDIRECTION.DOWN;

        return myType;
    }

    // Override the ToString
    public override string ToString()
    {
        return "piece1: " + piece1Coords.ToString() + " piece2: " + piece2Coords.ToString();
    }
}