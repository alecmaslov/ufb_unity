using UFB.StateSchema;
using UnityEngine;
using System;
using SchemaTest.InstanceSharingTypes;
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
    public class Tile : MonoBehaviour, IRaycastSelectable
    {
        public string Id => _tileState.id;
        public Coordinates Coordinates => _tileState.coordinates.ToCoordinates();
        public Vector3 Position => transform.position;
        public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1));
        public bool IsMoving { get; private set; }

        private MeshMapTile _meshMapTile;
        private TileState _tileState;
        private Interpolator<float> _heightInterpolator;
        private Vector3 _initialPosition;

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
            _tileState = tileState;
            name = Coordinates.Id;
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
    }
}
