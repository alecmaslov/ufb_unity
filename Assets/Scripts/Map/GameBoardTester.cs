using System.Linq;
using UFB.StateSchema;
using UnityEngine;

namespace UFB.Map
{
    public class GameBoardTester : MonoBehaviour
    {
        private GameMapController _gameBoard;

        private void Awake()
        {
            _gameBoard = GetComponent<GameMapController>();
        }

        public void SpawnBoard(MapState mapState)
        {
            _gameBoard.SpawnBoard(mapState);
        }

        public void StretchRandomTile()
        {
            var index = Random.Range(0, _gameBoard.Tiles.Count);
            StretchTileAt(index);
        }

        public void StretchTileAt(int index)
        {
            var tile = _gameBoard.Tiles.Values.ToArray()[index];
            tile.Stretch(10f, 0.5f);

            // Add MeshFilter and MeshRenderer to make it visible
            var meshFilter = tile.gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx"); // This gets Unity's default cube mesh

            var meshRenderer = tile.gameObject.AddComponent<MeshRenderer>();

            // Create a new bright material
            Material brightMat = new Material(Shader.Find("Standard"));
            brightMat.color = Color.magenta; // Set to a bright color, e.g. magenta
            meshRenderer.material = brightMat;
        }
    }
}
