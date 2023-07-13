using UnityEngine;
using UFB.Entities;

namespace UFB.Effects {

    public class GameBoardEffects : MonoBehaviour, IEffectsController {

        private GameBoard _gameBoard;

        // public GameBoardEffects(GameBoard gameBoard) {
        //     _gameBoard = gameBoard;
        // }

        private void OnEnable()
        {
            _gameBoard = GetComponent<GameBoard>();
        }
        
        public void RandomTileStretch()
        {
            _gameBoard.IterateTiles((tile, normIndex) => {
                var tileEntity = tile.GetComponent<TileEntity>();
                tileEntity.Stretch(Random.Range(0f, 20f), Random.Range(0.5f, 1.5f));
            });
        }

        public void ResetTileStretch()
        {
            _gameBoard.IterateTiles((tile, normIndex) => {
                var tileEntity = tile.GetComponent<TileEntity>();
                tileEntity.Stretch(0f, 1f);
            });
        }

        public void RadialBurst()
        {

        }


        public void RunEffect(string effectName, float rate) {
            switch (effectName) {
                case "randomTileStretch":
                    RandomTileStretch();
                    break;
                default:
                    Debug.Log($"Effect {effectName} not found");
                    break;
            }
        }

    }

}
