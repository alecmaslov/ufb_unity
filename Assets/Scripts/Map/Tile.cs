
namespace UFB.Map {
    
    // when a player lands on a coordinate, the
    // way the game state knows which one is by 
    // getting the tiles coordinate
    public struct Coordinate {
        public int x;
        public int y;
    }

    public class Tile {
        public string Id { get => $"tile_{Coordinate.x}_{Coordinate.y}"; }
        public Coordinate Coordinate { get; set; } 
    }
}

