using System;
using Colyseus.Schema;
using UFB.StateSchema;
using System.Collections.Generic;

namespace UFB.Network.ApiTypes
{
    public static class ApiConversions
    {
        public static MapState MapStateFromApiResponse(Map map, MapTile[] tiles)
        {
            try
            {
                MapState mapState = new MapState
                {
                    id = map.id,
                    name = map.name,
                    resourceAddress = map.resourceAddress,
                    gridWidth = 26,
                    gridHeight = 26
                    // gridWidth = Convert.ToInt32(map.gridWidth),
                    // gridHeight = Convert.ToInt32(map.gridHeight)
                };
                foreach (MapTile tile in tiles)
                {
                    TileState tileState = new TileState
                    {
                        id = tile.id,
                        tileCode = tile.tileCode,
                        type = tile.type
                    };

                    ArraySchema<byte> walls = new ArraySchema<byte>();
                    Dictionary<int, byte> wallsDict = new Dictionary<int, byte>();

                    for (int i = 0; i < tile.walls.Length; i++)
                    {
                        wallsDict[i] = (byte)tile.walls[i]; // Cast each int to a byte
                    }

                    walls.SetItems(wallsDict);

                    CoordinatesState coordinatesState = new CoordinatesState
                    {
                        x = tile.x,
                        y = tile.y
                    };

                    tileState.coordinates = coordinatesState;

                    mapState.tiles[tile.id] = tileState;
                }

                return mapState;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw e;
            }
        }
    }
}
