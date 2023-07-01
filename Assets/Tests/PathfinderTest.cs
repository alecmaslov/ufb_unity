using UnityEngine;
using UFB.Entities;
using UFB.Gameplay;
using UFB.Map;

namespace UFB.Development {
    
    public class PathfinderTest : MonoBehaviour
    {
        // [SerializeField] private int targetX;
        // [SerializeField] private int targetY;

        public float duration = 1f;
        public float durationIncrement = 0.1f;

        public float stretchAmount = 10f;

        public void PathfindBetween(Coordinates start, Coordinates end)
        {
            GameController.Instance.GameBoard.IterateTiles(t => t.ResetAppearance());

            Debug.Log("Pathfinding between " + start + " and " + end);
            var path = GameController.Instance.GameBoard.Pathfind(start, end);
            if (path.Count == 0)
            {
                Debug.Log("No path found");
                return;
            }
            
            float _duration = duration;
            foreach (var tile in path)
            {
                Debug.Log(tile);
                tile.Stretch(stretchAmount, duration);
                _duration += durationIncrement;
            }
        }
    }

}

