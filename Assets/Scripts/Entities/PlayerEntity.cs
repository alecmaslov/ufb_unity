using UnityEngine;
using UFB.Map;
// using System.Collections.Generic;
using System.Collections;

namespace UFB.Entities {

    public class PlayerEntity : MonoBehaviour
    {
        public string CharacterName { get; set; }

        GameTile _currentTile;
        GameBoard _gameBoard;


        private void Start() {

        }

        public void TraverseRoute() {
            // player moves over a series of tiles, and events at those 
            // tiles can be triggered
        }

        public void MoveToTile(GameTile tile) {
            _currentTile = tile;
            StartCoroutine(MoveTo(new Vector3(tile.Coordinates.X, 0, tile.Coordinates.Y), 1f));
        }

        public void MoveToTile(TileEntity tile) {
            MoveToTile(tile.GameTile);
        }

        public void MoveToTile(string tileId) {
            TileEntity tile = _gameBoard.GetTileById(tileId);
            MoveToTile(tile);
        }

        private IEnumerator MoveTo(Vector3 destination, float time) {
            float elapsedTime = 0;
            Vector3 startingPos = transform.position;
            while (elapsedTime < time) {
                transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            transform.position = destination;
        }
    }
    
}
