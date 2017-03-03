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

    //sort matches into 3, 4, and 5 matches
    public List<Match> DesignateBlobMatches(List<Match> reportedMatches)
    {
        List<Match> calculatedMatches = new List<Match>(reportedMatches);



        //iterate over the match list
        for (int outerMatchIndex = 0; outerMatchIndex < calculatedMatches.Count; outerMatchIndex++)
        {
            Match outerMatch = calculatedMatches[outerMatchIndex];
            //if the match exists (not removed because it is clustered with antoher match
            if (outerMatch.matchCoords.Count != 0)
            {
                //iterate over the matches again
                for (int innerMatchIndex = 0; innerMatchIndex < calculatedMatches.Count; innerMatchIndex++)
                {
                    //Debug.Log("in inner match loop");
                    Match innerMatch = calculatedMatches[innerMatchIndex];
                    //if the match exists and isn't removed because it was clustered with another match and the shapes match
                    if (outerMatchIndex != innerMatchIndex && innerMatch.matchCoords.Count != 0 && innerMatch.matchType == outerMatch.matchType)
                    {
                        //if there are overlapping coords between the outermatchcoords and neighbors and the inner match coords
                        if (CorrespondingCoords(MatchAndNeighborCoords(outerMatch),
                                                   innerMatch.matchCoords))
                        {
                            foreach (Coords innerMatchCoords in innerMatch.matchCoords)
                            {
                                bool existsInOuterMatch = false;
                                foreach (Coords outerMatchCoords in outerMatch.matchCoords)
                                {
                                    if (innerMatchCoords == outerMatchCoords)
                                    {
                                        existsInOuterMatch = true;
                                    }
                                }
                                if (!existsInOuterMatch) outerMatch.matchCoords.Add(innerMatchCoords);

                            }//end adding matches 
                            calculatedMatches[innerMatchIndex].matchCoords.Clear();
                            //Debug.Log("after matching set innermatch flag "+calculatedMatches[innerMatchIndex].removeFlag);
                        }
                    }
                }
            }
        }
        //clear matches flagged for removal

        for (int i = calculatedMatches.Count - 1; i > -1; i--)
        {
            //Debug.Log("in caculated matches match num "+i+" remove flag is " + calculatedMatches[i].removeFlag);
            if (calculatedMatches[i].matchCoords.Count == 0)
            {
                //Debug.Log("remove match");
                calculatedMatches.RemoveAt(i);
            }
        }
        calculatedMatches.TrimExcess();


        return calculatedMatches;
    }

    //returns true if the there are any equalities in the two input coordinate lists
    bool CorrespondingCoords(List<Coords> testMatchCoords1, List<Coords> testMatchCoords2)
    {
        bool coordsMatch = false;

        foreach (Coords outerCoords in testMatchCoords1)
        {
            foreach (Coords innerCoords in testMatchCoords2)
            {
                if (outerCoords == innerCoords)
                {
                    coordsMatch = true;
                    break;
                }
            }
            if (coordsMatch) break;
        }

        return coordsMatch;
    }


    //returns the coords of the match and any adjacent for 5 matches
    List<Coords> MatchAndNeighborCoords(Match inMatch)
    {
        List<Coords> coordsStorage = new List<Coords>();

        //iterate over each piece 
        foreach (Coords pieceCoords in inMatch.matchCoords)
        {
            bool mainListCheck = false;
            foreach (Coords storageCoords in coordsStorage)
            {
                if (storageCoords == pieceCoords)
                    mainListCheck = true;
            }
            if (!(mainListCheck))
            {
                coordsStorage.Add(pieceCoords);
            }

            //check each neighbor to see if they are in the main list
            foreach (Coords pieceAndNeighborCoords in NeighborCoords(pieceCoords))
            {
                bool neighborCheck = false;
                foreach (Coords storageCoords in coordsStorage)
                {
                    if (pieceAndNeighborCoords == storageCoords)
                        neighborCheck = true;
                }
                if (!(neighborCheck))
                {
                    coordsStorage.Add(pieceAndNeighborCoords);
                }
            }

        }

        return coordsStorage;
    }

    //returns the neighboring coords to the input coords
    List<Coords> NeighborCoords(Coords inCoords)
    {
        List<Coords> neighborCoords = new List<Coords>();
        //north neighbor
        if (inCoords.y < Constants.BOARDSIZE - 1)
        {
            neighborCoords.Add(new Coords(inCoords.x, inCoords.y + 1));
        }
        //south neighbor
        if (inCoords.y > 0)
        {
            neighborCoords.Add(new Coords(inCoords.x, inCoords.y - 1));
        }
        //east neighbor
        if (inCoords.x < Constants.BOARDSIZE - 1)
        {
            neighborCoords.Add(new Coords(inCoords.x + 1, inCoords.y));
        }
        //west neighbor
        if (inCoords.x > 0)
        {
            neighborCoords.Add(new Coords(inCoords.x - 1, inCoords.y));
        }

        return neighborCoords;
    }
}
