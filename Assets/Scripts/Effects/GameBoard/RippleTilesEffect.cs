using UnityEngine;
using UFB.Entities;
using System.Collections.Generic;

namespace UFB.Effects
{
    [System.Serializable]
    public class RippleTilesEffect : IEffect
    {
        private readonly GameBoard _gameBoard;
        public TileEntity CurrentTile;
        public float Rate = 10f;
        public float Intensity = 5f;
        public float StretchDuration = 0.2f;
        public float Decay = 0.3f;

        public RippleTilesEffect(
            GameBoard gameBoard,
            TileEntity centerTile,
            float rate = 10f,
            float intensity = 5f,
            float stretchDuration = 0.2f,
            float decay = 0.3f)
        {
            _gameBoard = gameBoard;
            Rate = rate;
            Intensity = intensity;
            StretchDuration = stretchDuration;
            Decay = decay;
            CurrentTile = centerTile;
        }

        public void ExecuteOnTile(TileEntity tile)
        {
            _gameBoard.IterateTiles(t => TileCallback(t, tile));
        }

        public void Execute()
        {
            _gameBoard.IterateTiles(t => TileCallback(t, CurrentTile));
        }


        private void TileCallback(TileEntity tile, TileEntity currentTile)
        {
            if (tile == currentTile) return; // skip center tile
            float distance = currentTile.DistanceTo(tile);
            float intensity = Intensity * Mathf.Exp(-Decay * distance);
            CoroutineHelpers.DelayedAction(() =>
            {
                tile.Stretch(intensity, StretchDuration, () =>
                {
                    CoroutineHelpers.DelayedAction(() =>
                    {
                        tile.ResetStretch(StretchDuration);
                    }, 0.01f, _gameBoard);
                });
            }, distance * (1 / Rate), _gameBoard);
        }
    }

}