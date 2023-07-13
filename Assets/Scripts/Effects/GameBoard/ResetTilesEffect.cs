using UnityEngine;
using UFB.Entities;

namespace UFB.Effects
{
    public class ResetTilesEffect : IEffect
    {
        private readonly GameBoard _gameBoard;
        private readonly float _rate;

        public ResetTilesEffect(GameBoard gameBoard, float rate)
        {
            _gameBoard = gameBoard;
            _rate = rate;
        }

        public void Execute()
        {
            _gameBoard.IterateTiles((tile, normIndex) =>
            {
                var tileEntity = tile.GetComponent<TileEntity>();
                tileEntity.Stretch(0.1f, _rate);
            });
        }
    }

}