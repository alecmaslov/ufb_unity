using System;
using UFB.Events;
using UFB.StateSchema;
using UnityEngine;

namespace UFB.Events
{
    public class TileClickedEvent
    {
        public UFB.Map.BaseTile tile;

        public TileClickedEvent(UFB.Map.BaseTile tile)
        {
            this.tile = tile;
        }
    }
}

namespace UFB.Map
{
    public class BaseTile : MonoBehaviour, IClickable, ITile
    {
        public string Id => Parameters.Id;
        public Coordinates Coordinates => Parameters.Coordinates;
        public Vector3 Position => transform.position;
        public TileParameters Parameters { get; private set; }

        [SerializeField]
        private AnimationCurve _curve = new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(0.5f, 1)
        );

        private ITileRenderer _tileRenderer;
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
                    _tileRenderer.SetHeight(newHeight);

                    // here it can call _meshMapTile set height, then we will MOVE the actual gameObject
                    // up. This allows us to take advantage of parenting the gameobject to the tile,
                    // making attachment so much easier
                    transform.position = _initialPosition + new Vector3(0, newHeight, 0);
                },
                () => _tileRenderer.Height,
                _curve,
                (newHeight) =>
                {
                    _tileRenderer.SetHeight(newHeight);
                }
            );

            _heightInterpolator.SetSnapshot(0f, "init");
        }

        public void SetRenderer(ITileRenderer tileRenderer)
        {
            _tileRenderer = tileRenderer;
        }

        public void SetParameters(TileParameters parameters)
        {
            Parameters = parameters;
            name = Coordinates.GameId + $" ({Coordinates.X}, {Coordinates.Y})";
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
        }

        public void OnClick()
        {
            Debug.Log($"Clicked on tile {Coordinates.Id}");
            EventBus.Publish(new TileClickedEvent(this));
        }

        public override string ToString()
        {
            return $"[Tile] {name} | Coordinates {Coordinates.Id}";
        }
    }

    // public class 
}
