using UFB.StateSchema;
using UnityEngine;
using System;
using SchemaTest.InstanceSharingTypes;

namespace UFB.Map
{
    public class Tile : MonoBehaviour
    {
        public string Id => _tileState.id;
        public Coordinates Coordinates => _tileState.coordinates.ToCoordinates();
        public Vector3 Position => transform.position;
        public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1));
        public bool IsMoving { get; private set; }

        private MeshMapTile _meshMapTile;
        private TileState _tileState;
        private Interpolator<float> _heightInterpolator;

        private void Awake()
        {
            _heightInterpolator = new Interpolator<float>(
                this,
                Mathf.Lerp,
                (newHeight) =>
                {
                    _meshMapTile.SetHeight(newHeight);
                    IsMoving = true;
                },
                () => _meshMapTile.Height,
                curve,
                (newHeight) =>
                {
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

        public void Stretch(float heightScalar, float duration = 0.5f, Action onComplete = null)
        {
            // _meshMapTile.Stretch(intensity, duration, callback);
            // here it can call _meshMapTile set height, then we will MOVE the actual gameObject
            // up. This allows us to take advantage of parenting the gameobject to the tile,
            // making attachment so much easier


            _heightInterpolator.LerpTo(heightScalar, duration, onComplete);
        }

        public void ResetStretch(float duration)
        {
            _heightInterpolator.LerpToSnapshot("init", duration);
        }

        public void AttachGameObject(GameObject gameObject, bool zeroLocalPosition = false)
        {
            gameObject.transform.SetParent(transform);
            if (zeroLocalPosition)
                gameObject.transform.localPosition = Vector3.zero;
        }
    }
}
