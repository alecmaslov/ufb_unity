using UnityEngine;
using UFB.Map;
using UFB.Core;

namespace UFB.Effects
{
    [System.Serializable]
    public class RippleTilesEffect : IEffect
    {
        public Tile CurrentTile;
        public float Rate = 10f;
        public float Intensity = 5f;
        public float StretchDuration = 0.2f;
        public float Decay = 0.3f;
        private GameBoard _gameBoard;

        public RippleTilesEffect(
            Tile centerTile,
            float rate = 10f,
            float intensity = 5f,
            float stretchDuration = 0.2f,
            float decay = 0.3f)
        {
            Rate = rate;
            Intensity = intensity;
            StretchDuration = stretchDuration;
            Decay = decay;
            CurrentTile = centerTile;
            _gameBoard = ServiceLocator.Current.Get<GameBoard>();
        }
        
        public void Execute()
        {
            _gameBoard.IterateTiles(t => TileCallback(t, CurrentTile));
        }


        private void TileCallback(Tile tile, Tile currentTile)
        {
            if (tile == currentTile) return; // skip center tile
            float distance = currentTile.Coordinates.DistanceTo(tile.Coordinates);
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