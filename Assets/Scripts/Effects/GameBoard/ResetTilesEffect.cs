using UnityEngine;
using UFB.Map;
using UFB.Core;

namespace UFB.Effects
{
    public class ResetTilesEffect : IEffect
    {
        private readonly GameMapController _gameBoard;
        private readonly float _rate;

        public ResetTilesEffect(float rate)
        {
            _gameBoard = ServiceLocator.Current.Get<GameMapController>();
            _rate = rate;
        }

        public void Execute()
        {
            _gameBoard.IterateTiles((tile, normIndex) =>
            {
                tile.Stretch(0.1f, _rate);
            });
        }
    }

}