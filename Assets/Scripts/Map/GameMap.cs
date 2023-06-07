using UnityEngine;
using System.Collections;

namespace UFB.Map {

    public struct Coordinate {
        public int x;
        public int y;
    }

    public class Tile {
        public string id;
        public Coordinate coordinate; 
    }


    public class GameMap {

        string name;
        Tile[,] tiles;
    }

}