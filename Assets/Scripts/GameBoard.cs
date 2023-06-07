using UnityEngine;
using System.Collections;

public struct Coordinate {
    public int x;
    public int y;
}

public class Tile {
    public string id;
    public Coordinate coordinate; 
}


public class GameBoard {

    string name;
    Tile[,] tiles;

}