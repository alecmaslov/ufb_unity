using UnityEngine;
using UFB.Effects;

namespace UFB.Entities {

    public class EffectsManager : MonoBehaviour
    {

        private GameBoard _gameBoard;

        public GameObject effectsControllerObject;

        void Start()
        {
            var gameBoards = GameObject.FindGameObjectsWithTag("GameBoard");
            if (gameBoards.Length > 1) {
                Debug.LogError("More than one GameBoard found");
                return;
            }
            if (gameBoards.Length == 0) {
                Debug.LogError("No GameBoard found");
                return;
            }
            _gameBoard = gameBoards[0].GetComponent<GameBoard>();
            Debug.Log("EffectsManager Start " + _gameBoard);
        }

        public void RunEffect(string effectName)
        {
            // var gameBoardEffects = new GameBoardEffects(_gameBoard);
            var effectsController = effectsControllerObject.GetComponent<IEffectsController>();
            effectsController.RunEffect(effectName, 1f);
        }

    }
}