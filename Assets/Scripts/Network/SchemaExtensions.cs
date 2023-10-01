using UFB.Map;
using UFB.Network.RoomMessageTypes;
using System;
using Colyseus.Schema;

namespace UFB.StateSchema
{
    public static class SchemaExtensions
    {
        public static Coordinates ToCoordinates(this CoordinatesState schema)
        {
            // Convert float to int (you might want to round or handle this differently)
            int x = Convert.ToInt32(schema.x);
            int y = Convert.ToInt32(schema.y);

            return new Coordinates(x, y);
        }

        public static void FromCoordinates(this CoordinatesState schema, Coordinates coordinates)
        {
            schema.x = coordinates.X;
            schema.y = coordinates.Y;
        }

        public static TileState TileStateAtCoordinates(
            this MapState mapState,
            Coordinates coordinates
        )
        {
            foreach (TileState tileState in mapState.tiles.Values)
            {
                if (tileState.coordinates.ToCoordinates().Equals(coordinates))
                {
                    return tileState;
                }
            }
            return null;
        }

        public static float Percent(this RangedValueState state)
        {
            return state.current / state.max;
        }

    }
}
