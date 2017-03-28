using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants  {


    //integer: width and height of game board
   public static readonly int BOARDSIZE = 8;
    //seconds: time it takes swap to complete
   public static readonly float SWAPANIMATIONTIME = .2f;
    //seconds: time it takes to move tiles to position
    public static readonly float MOVEANIMATIONTIME = .8f;
    //Unity units: initial height of replacement tiles (should be outside of camera view)
    public static readonly float REPLACETILESTARTHEIGHT = 10;
    //seconds: amount of time between match chains
    public static readonly float MATCHCHAINDELAY = .2f;
    //string: name of playerpref for the Mode 1 high score
    public static readonly string MODE1HIGHSCOREPREF = "mode1highscore";
    //seconds: delay time after score shows up before it starts to move
    public static readonly float SCOREMOVEDELAY = .2f;
    //amount score increases for each chain
    public static readonly float CHAINMATCHMULTIPLIER = .25f;
    //seconds: time it takes to transition into or out of a scene
    public static readonly float SCENETRANSITIONTIME = .33f;
	//seconds: bonus time
	public static readonly float BONUSTIME = 10f;
	//percent: maximum bonus amount as a percent of the match score
	public static readonly float MAXBONUS = 100f;
	//seconds: time until hint is shown
	public static readonly float HINTTIME = 5f;
}