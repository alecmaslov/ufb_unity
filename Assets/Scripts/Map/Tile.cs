using UFB.StateSchema;
using UnityEngine;
using System;
using UFB.Events;

namespace UFB.Events
{
    public class TileClickedEvent
    {
        public UFB.Map.Tile tile;

        public TileClickedEvent(UFB.Map.Tile tile)
        {
            this.tile = tile;
        }
    }
}

namespace UFB.Map
{
    public class Tile : MonoBehaviour, IClickable
    {
        public string Id => _state.id;
        public Coordinates Coordinates => _state.coordinates.ToCoordinates();
        public Vector3 Position => transform.position;
        public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1));
        public bool IsMoving { get; private set; }

        public string TilePosText => Coordinates.ColumnName + (int.Parse(Coordinates.RowName) + 1).ToString("D2");

        private MeshMapTile _meshMapTile;
        private TileState _state;
        private Interpolator<float> _heightInterpolator;
        private Vector3 _initialPosition;
        private TileWalls _tileWalls;

        private void Awake()
        {
            _initialPosition = transform.position;
            _heightInterpolator = new Interpolator<float>(
                this,
                Mathf.Lerp,
                (newHeight) =>
                {
                    _meshMapTile.SetHeight(newHeight);

                    // here it can call _meshMapTile set height, then we will MOVE the actual gameObject
                    // up. This allows us to take advantage of parenting the gameobject to the tile,
                    // making attachment so much easier
                    transform.position = _initialPosition + new Vector3(0, newHeight, 0);
                    IsMoving = true;
                },
                () => _meshMapTile.Height,
                curve,
                (newHeight) =>
                {
                    _meshMapTile.SetHeight(newHeight);
                    IsMoving = false;
                }
            );

            _heightInterpolator.SetSnapshot(0f, "init");
        }

        public void Initialize(MeshMapTile meshMapTile, TileState tileState)
        {
            _meshMapTile = meshMapTile;
            _state = tileState;
            name = Coordinates.GameId + $" ({Coordinates.X}, {Coordinates.Y})" + Id;

            //if(tileState.type == "Bridge" || tileState.type == "Floor" || tileState.type == "Void")
            //{
                //SpawnWalls();
            //}
        }

        public void Stretch(float height, float duration = 0.5f, Action onComplete = null)
        {
            _heightInterpolator.LerpTo(height, duration, onComplete);
        }

        public void ResetStretch(float duration)
        {
            // _heightInterpolator.LerpTo(0f, duration);
            _heightInterpolator.LerpToSnapshot("init", duration);
        }

        public void AttachGameObject(GameObject gameObject, bool zeroLocalPosition = false)
        {
            gameObject.transform.SetParent(transform);
            if (zeroLocalPosition)
                gameObject.transform.localPosition = Vector3.zero;
            if(_state.type == "Upper")
            {
                gameObject.transform.localPosition += Vector3.up * 2.15f;
            }

        }

        public TileState GetTileState() { return _state; }

        public void OnClick()
        {
            Debug.Log($"Clicked on tile {Coordinates.Id}");
            EventBus.Publish(new TileClickedEvent(this));
        }

        public void PrintState()
        {
            Debug.Log($"Name: {name} | Tile {Coordinates.Id} | Meshmap {_meshMapTile.Coordinates} | Coordinates {Coordinates}");
        }

        private void SpawnWalls()
        {
            if (_state.walls == null)
                return;

            _tileWalls = gameObject.AddComponent<TileWalls>();
            _tileWalls.SpawnWalls(_state.walls);
        }
    }
}
