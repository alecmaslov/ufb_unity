using UnityEngine;
using UFB.Map;

namespace UFB.Effects
{
    public class RandomTileStretchEffect : IEffect
    {
        private readonly GameBoard _gameBoard;
        private readonly float _rate;

        public RandomTileStretchEffect(GameBoard gameBoard, float rate)
        {
            _gameBoard = gameBoard;
            _rate = rate;
        }

        public void Execute()
        {
            _gameBoard.IterateTiles((tile, normIndex) =>
            {
                tile.Stretch(Random.Range(0f, 20f), _rate);
            });
        }
    }

}