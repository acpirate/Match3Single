using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{

    public static MatchController Instance;


    void Awake()
    {
        Instance = this;
    }

    //return a list of all of the straight/line matches currently showing
    public List<Match> getBaseMatches()
    {

        List<Match> baseMatches = new List<Match>();

        TILETYPE baseType = TILETYPE.NONE;
        int runCount = 1;

        //vertical matches
        for (int colCounter = 0; colCounter < Constants.BOARDSIZE; colCounter++)
        {
            baseType = TILETYPE.NONE;
            runCount = 1;
            Match tempMatch = new Match();
            for (int rowCounter = 0; rowCounter < Constants.BOARDSIZE; rowCounter++)
            {
                TileController tempTileController = BoardController.Instance.GetTileAtCoords(new Coords(colCounter, rowCounter)).GetComponent<TileController>();
                if (tempTileController.myType == baseType)
                {
                    runCount++;
                    if (runCount == 3)
                    {
                        tempMatch.matchCoords.Add(new Coords(colCounter, rowCounter));
                        tempMatch.matchCoords.Add(new Coords(colCounter, rowCounter - 1));
                        tempMatch.matchCoords.Add(new Coords(colCounter, rowCounter - 2));
                        baseMatches.Add(tempMatch);
                    }
                    if (runCount > 3)
                    {
                        tempMatch.matchCoords.Add(new Coords(colCounter, rowCounter));
                    }
                }
                //the piece doesnt match the previous match
                else
                {
                    //add the previous match to the matchlist if the runcount > 2
                    runCount = 1;
                    baseType = tempTileController.myType;
                    tempMatch = new Match(baseType);

                }

            }
        }

        //horizontal matches
        for (int rowCounter = 0; rowCounter < Constants.BOARDSIZE; rowCounter++)
        {
            baseType = TILETYPE.NONE;
            runCount = 1;
            Match tempMatch = new Match();
            for (int colCounter = 0; colCounter < Constants.BOARDSIZE; colCounter++)
            {
                TileController tempTileController = BoardController.Instance.GetTileAtCoords(new Coords(colCounter, rowCounter)).GetComponent<TileController>();
                if (tempTileController.myType == baseType)
                {
                    runCount++;
                    if (runCount == 3)
                    {
                        tempMatch.matchCoords.Add(new Coords(colCounter, rowCounter));
                        tempMatch.matchCoords.Add(new Coords(colCounter - 1, rowCounter));
                        tempMatch.matchCoords.Add(new Coords(colCounter - 2, rowCounter));
                        baseMatches.Add(tempMatch);
                    }
                    if (runCount > 3)
                    {
                        tempMatch.matchCoords.Add(new Coords(colCounter, rowCounter));
                    }
                }
                //the piece doesnt match the previous match
                else
                {
                    //add the previous match to the matchlist if the runcount > 2
                    runCount = 1;
                    baseType = tempTileController.myType;
                    tempMatch = new Match(baseType);

                }

            }
        }


        return baseMatches;

    }
}
