using UnityEngine;
using UFB.Effects;
using UFB.Gameplay;

namespace UFB.Entities {

    public class EffectsManager : MonoBehaviour
    {
        public void RunEffect(string effectName)
        {
            GameBoard gameBoard = GameController.Instance.GameBoard;
            IEffectsController effectsController = gameBoard.GetComponent<IEffectsController>();
            effectsController.RunEffect(effectName, 1f);
        }

    }
}