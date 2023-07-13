using UnityEngine;
using UFB.Map;
using System.Collections.Generic;
using System.Collections;

namespace UFB.Entities {

    public class PlayerEntity : MonoBehaviour
    {
        public string CharacterName { get; set; }

        TileEntity _currentTile;
        GameBoard _gameBoard;

        public void Initialize(string characterName, GameBoard gameBoard, TileEntity startingTile) {
            _gameBoard = gameBoard;
            CharacterName = characterName;
            // _currentTile = startingTile;
            SetTile(startingTile);
        }


        private void Start() {
            // _currentTile = GetComponent
        }

        public void SetTile(TileEntity tile) {
            _currentTile = tile;
            tile.AttachEntity(this.gameObject);
        }

        public void TraverseRoute() {
            // player moves over a series of tiles, and events at those 
            // tiles can be triggered
        }

        // public void MoveToTile(GameTile tile) {
        //     var tileEntity = _gameBoard.GetTileByCoordinates(tile.Coordinates);
        //     MoveToTile(tileEntity);
        // }

        public void MoveToTile(TileEntity tile) {
            var path = _gameBoard.Pathfind(_currentTile.Coordinates, tile.Coordinates);
            if (path == null) {
                Debug.LogError("No path found from " + _currentTile.Coordinates.ToString() + " to " + tile.Coordinates.ToString());
                return;
            }
            // StartCoroutine(MoveTo(tile.gameObject.transform.position, 1f));
            StartCoroutine(MoveAlongPath(path, 0.1f));
            _currentTile = tile;
        }

        public void MoveToTile(string tileId) {
            TileEntity tile = _gameBoard.GetTileById(tileId);

            MoveToTile(tile);
        }

        private IEnumerator MoveAlongPath(List<TileEntity> path, float time) {
            foreach (TileEntity tile in path) {
                Vector3 startingPos = transform.position;
                float elapsedTime = 0;
                Vector3 destination = tile.gameObject.transform.position;
                
                while (elapsedTime < time) {
                    transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / time));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                transform.position = destination;
                // yield return StartCoroutine(MoveTo(tile.gameObject.transform.position, 1f));
                // _currentTile = tile;
            }

            path[path.Count - 1].AttachEntity(this.gameObject);
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
