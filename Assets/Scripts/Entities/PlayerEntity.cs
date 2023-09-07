using UnityEngine;
using UFB.Map;
using System.Collections.Generic;
using System.Collections;
using System;
using UFB.Core;

namespace UFB.Entities
{

    [RequireComponent(typeof(PositionAnimator))]
    [RequireComponent(typeof(TileAttachable))]
    public class PlayerEntity : MonoBehaviour
    {

        public string PlayerId { get; set; }
        public string CharacterName { get; set; }
        public TileAttachable TileAttachable { get; private set; }
        public TileEntity CurrentTile => TileAttachable.CurrentTile;

        // public Action OnMoveComplete;
        // public TileEntity CurrentTile => _currentTile;
        // public Coordinates Coordinates => _currentTile.Coordinates;
        // private TileEntity _currentTile;

        [SerializeField] Transform _modelTransform;

        private GameBoard _gameBoard;
        private Animator _animator;
        private bool _isMoving = false;
        private PositionAnimator _positionAnimator;

        // idea - add in player ability to face in the most optimal direction

        public void Initialize(string characterName, GameBoard gameBoard, TileEntity startingTile, string playerId)
        {
            PlayerId = playerId;
            // _animator = GetComponentInChildren<Animator>();
            _modelTransform = transform.Find("Container");
            _animator = _modelTransform.GetComponent<Animator>();
            _gameBoard = gameBoard;
            // CharacterName = characterName;
            CharacterName = name.Split("__")[1].Replace("(Clone)", "");
            ForceMoveToTile(startingTile, 0);
        }


        private void OnEnable()
        {
            _positionAnimator = GetComponent<PositionAnimator>();
            TileAttachable = GetComponent<TileAttachable>();
        }

        public void TraverseRoute()
        {
            // player moves over a series of tiles, and events at those 
            // tiles can be triggered
        }

        public void ApplyToCurrentTile(Action<TileEntity> action)
        {
            action?.Invoke(TileAttachable.CurrentTile);
        }


        // THIS SHOULD BE IN GAMEBOARD
        public void PreviewRoute(Coordinates destination)
        {
            var path = _gameBoard.Pathfind(TileAttachable.CurrentTile.Coordinates, destination);
            if (path == null)
            {
                Debug.LogError("No path found from " + TileAttachable.CurrentTile.Coordinates.ToString() + " to " + destination.ToString());
                return;
            }
            foreach (TileEntity tile in path)
            {
                tile.Stretch(1.1f, 1.5f);
                tile.ChangeColor(Color.red, 0.5f);
                // tile.Glow(1.5f);
            }
        }


        public void FocusCamera()
        {
            CameraController.Controller.FocusOn(_modelTransform);
            // CameraController.Instance.FocusOnTile(_currentTile);
            // eventually have a static camera script that implements an ICameraController,
            // which has a single instance of an ICameraController that we can call Focus() on
        }

        public void ToggleHop()
        {
            if (_isMoving) HopEnd();
            else HopStart();
        }


        public void HopStart()
        {
            if (_isMoving)
            {
                Debug.LogError("Trying to start hop when already moving");
                return;
            }
            _isMoving = true;
            TileAttachable.DetachFromTile();
            _animator.SetTrigger("HopStart");
        }


        public void HopEnd(TileEntity tile = null)
        {
            if (!_isMoving)
            {
                Debug.LogError("Trying to end hop when not moving");
                return;
            }

            if (tile == null)
            {
                if (TileAttachable.LastTile == null)
                {
                    Debug.LogError("Trying to end hop with no last tile");
                    return;
                }
                tile = TileAttachable.LastTile;
            }
            _animator.SetTrigger("HopEnd");
            TileAttachable.AttachToTile(tile);
        }

        /// <summary>
        /// Called when the animation of player hopping ends
        /// </summary>
        public void OnHopEndAnimationExit()
        {
            _isMoving = false;
            _gameBoard.RunRippleEffect(TileAttachable.CurrentTile);
        }

        public void ForceMoveToTileAnimate(TileEntity tile, float duration = 0.5f, Action onComplete = null)
        {
            HopStart();

            CoroutineHelpers.DelayedAction(() => {
                _positionAnimator.AnimateTo(tile.AttachedPosition, duration, () =>
                {
                    Debug.Log("Finished moving to tile " + tile.Coordinates.ToString());
                    HopEnd(tile);
                    // TileAttachable.AttachToTile(tile);
                    onComplete?.Invoke();
                });
            }, duration, this);

        }

        /// <summary>
        /// Forcefully moves entity to tile
        /// </summary>
        public void ForceMoveToTile(TileEntity tile, float duration = 0.5f, Action onComplete = null)
        {
            TileAttachable.DetachFromTile();
            _positionAnimator.AnimateTo(tile.AttachedPosition, duration, () =>
            {
                Debug.Log("Finished moving to tile " + tile.Coordinates.ToString());
                TileAttachable.AttachToTile(tile);
                onComplete?.Invoke();
            });
        }

        /// <summary>
        /// Tries to use pathfinding algorithm to move to the tile
        /// </summary>
        public void TryMoveToTile(TileEntity tile)
        {
            var path = _gameBoard.Pathfind(TileAttachable.CurrentTile, tile);
            if (path == null)
            {
                Debug.LogError("No path found from " + TileAttachable.CurrentTile.Coordinates.ToString() + " to " + tile.Coordinates.ToString());
                return;
            }
            StartCoroutine(MoveAlongPath(path, 0.1f));
        }

        private IEnumerator MoveAlongPath(List<TileEntity> path, float time)
        {
            // Hop();
            _animator.SetTrigger("HopStart");
            yield return new WaitForSeconds(2f);
            TileAttachable.DetachFromTile();

            _isMoving = true;
            foreach (TileEntity tile in path)
            {
                Vector3 startingPos = transform.position;
                float elapsedTime = 0;
                Vector3 destination = tile.AttachedPosition;

                while (elapsedTime < time)
                {
                    transform.position = Vector3.Lerp(startingPos, destination, (elapsedTime / time));
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                if (tile != path[path.Count - 1])
                {
                    tile.OnPassThrough(this);
                }

                transform.position = destination;
                // _currentTile = tile;
            }

            // path[path.Count - 1].AttachEntity(this.gameObject);
            // SetTile(path[path.Count - 1]);
            TileAttachable.AttachToTile(path[path.Count - 1]);
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
