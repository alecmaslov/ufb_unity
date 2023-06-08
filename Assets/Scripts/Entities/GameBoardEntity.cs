using UnityEngine;
using System.Collections.Generic;
using UFB.Map;

namespace UFB.Entities {

    // handles spawning in tiles, managing them,
    // and anything related to any parent gameobject
    // of the tiles
    public class GameBoardEntity : MonoBehaviour
    {
        private List<TileEntity> _tiles;
        private GameMap _map;
    }

}