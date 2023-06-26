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
        
        public void TileStretchEffect()
        {
            // if (_gameBoard == null) {
            //     _gameBoard = GetComponent<GameBoard>();
            // }
            _gameBoard.IterateTiles((tile, normIndex) => {
                var tileEntity = tile.GetComponent<TileEntity>();
                tileEntity.Stretch(Random.Range(0f, 20f), Random.Range(10.5f, 40.5f));
            });
        }


        public void RunEffect(string effectName, float rate) {
            switch (effectName) {
                case "tileStretch":
                    TileStretchEffect();
                    break;
                default:
                    Debug.Log($"Effect {effectName} not found");
                    break;
            }
        }

    }

}
