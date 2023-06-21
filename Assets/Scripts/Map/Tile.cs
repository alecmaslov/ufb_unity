
namespace UFB.Map {
    
    // when a player lands on a coordinate, the
    // way the game state knows which one is by 
    // getting the tiles coordinate
    public struct Coordinate {
        public int x;
        public int y;
    }

    public class Tile {
        public string Id { get => $"tile_{ColumnName}_{RowName}"; }

        public string ColumnName => _tileColumns[Coordinate.x];
        public string RowName => Coordinate.y.ToString();

        public Coordinate Coordinate { get; set; }

        private readonly string[] _tileColumns =  { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", 
                                                "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", 
                                                "V", "W", "X", "Y", "Z" }; 
    }
}

