using UnityEngine;
using UFB.Map;
using System.Collections.Generic;
using System.Collections;
using System;
using UFB.Core;

namespace UFB.Entities {

    [RequireComponent(typeof(PositionAnimator))]
    public class PlayerEntity : MonoBehaviour
    {
        public string CharacterName { get; set; }

        public Action OnMoveComplete;
        public TileEntity CurrentTile => _currentTile;
        public Coordinates Coordinates => _currentTile.Coordinates;

        private TileEntity _currentTile;
        private GameBoard _gameBoard;
        private Animator _animator;
        private bool _isMoving = false;
        private PositionAnimator _positionAnimator;

        // idea - add in player ability to face in the most optimal direction

        public void Initialize(string characterName, GameBoard gameBoard, TileEntity startingTile) {
            _animator = GetComponentInChildren<Animator>();
            _gameBoard = gameBoard;
            CharacterName = characterName;
            ForceMoveToTile(startingTile, 0.1f);
        }


        private void OnEnable() {
            _positionAnimator = GetComponent<PositionAnimator>();
        }

        /// <summary>
        /// Sets the player's current tile as the given tile
        /// </summary>
        private void SetTile(TileEntity tile) {
            _currentTile = tile;
            tile.AttachEntity(this.gameObject);
            OnMoveComplete?.Invoke();
        }

        public void TraverseRoute() {
            // player moves over a series of tiles, and events at those 
            // tiles can be triggered
        }

        // public void MoveToTile(GameTile tile) {
        //     var tileEntity = _gameBoard.GetTileByCoordinates(tile.Coordinates);
        //     MoveToTile(tileEntity);
        // }


        // THIS SHOULD BE IN GAMEBOARD
        public void PreviewRoute(Coordinates destination)
        {
            var path = _gameBoard.Pathfind(_currentTile.Coordinates, destination);
            if (path == null) {
                Debug.LogError("No path found from " + _currentTile.Coordinates.ToString() + " to " + destination.ToString());
                return;
            }
            foreach (TileEntity tile in path) {
                tile.Stretch(1.1f, 1.5f);
                // tile.Glow(1.5f);
            }
        }


        public void FocusCamera() 
        {
            CameraController.Controller.FocusOn(transform);
            // CameraController.Instance.FocusOnTile(_currentTile);
            // eventually have a static camera script that implements an ICameraController,
            // which has a single instance of an ICameraController that we can call Focus() on
        }

        public void Hop()
        {
            if (_isMoving) {
                _animator.SetTrigger("HopEnd");
                _isMoving = false;
            }
            else {
                _animator.SetTrigger("HopStart");
                _isMoving = true;
            }
        }

        /// <summary>
        /// Forcefully moves entity to tile
        /// </summary>
        public void ForceMoveToTile(TileEntity tile, float duration = 0.5f, Action onComplete = null)
        {
            _positionAnimator.AnimateTo(tile.EntityPosition, duration, () => {
                Debug.Log("Finished moving to tile " + tile.Coordinates.ToString());
                SetTile(tile);
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// Tries to use pathfinding algorithm to move to the tile
        /// </summary>
        public void TryMoveToTile(TileEntity tile) {
            var path = _gameBoard.Pathfind(_currentTile.Coordinates, tile.Coordinates);
            if (path == null) {
                Debug.LogError("No path found from " + _currentTile.Coordinates.ToString() + " to " + tile.Coordinates.ToString());
                return;
            }
            StartCoroutine(MoveAlongPath(path, 0.1f));
        }

        private IEnumerator MoveAlongPath(List<TileEntity> path, float time) {
            // Hop();
            _animator.SetTrigger("HopStart");
            yield return new WaitForSeconds(2f);
            _isMoving = true;
            foreach (TileEntity tile in path) {
                Vector3 startingPos = transform.position;
                float elapsedTime = 0;
                Vector3 destination = tile.EntityPosition;
                
                while (elapsedTime < time) {
                    transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / time));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                if (tile != path[path.Count - 1]) {
                    tile.OnPassThrough(this);
                }

                transform.position = destination;
                // _currentTile = tile;
            }

            // path[path.Count - 1].AttachEntity(this.gameObject);
            SetTile(path[path.Count - 1]);
            _animator.SetTrigger("HopEnd");
            _isMoving = false;
        }

        // private IEnumerator MoveTo(Vector3 destination, float time) {
        //     float elapsedTime = 0;
        //     Vector3 startingPos = transform.position;
        //     while (elapsedTime < time) {
        //         transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / time));
        //         elapsedTime += Time.deltaTime;
        //         yield return null;
        //     }
        //     transform.position = destination;
        // }
    }
    
}
