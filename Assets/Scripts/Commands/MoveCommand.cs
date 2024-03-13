using UnityEngine;
using UFB.Map;

namespace UFB.Commands
{
    public class MoveCommand : Command
    {
        private BaseTile _tile;
        private Vector3 _destination;

        public MoveCommand(BaseTile tile, Vector3 destination)
        {
            _tile = tile;
            _destination = destination;
        }

        public override void Execute()
        {
            // logic for moving the tile to the destination
        }
    }
}
