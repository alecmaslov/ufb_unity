using UnityEngine;
using System.Collections;
using UFB.Core;
using UnityEngine.AddressableAssets;

namespace UFB.Camera
{
    public class PositionableCamera : MonoBehaviour, ICameraPositioner
    {
        public AssetReference cameraTargetPrefab;

        private PositionAnimator _positionAnimator;
        private RotationAnimator _rotationAnimator;

        public delegate void OnPositionUpdateHandler(Vector3 newPos);
        public event OnPositionUpdateHandler OnPositionUpdate;

        public delegate void OnRotationUpdateHandler(Quaternion newRot);
        public event OnRotationUpdateHandler OnRotationUpdate;

        public delegate void OnTransformStartHandler();
        public event OnTransformStartHandler OnTransformStart;

        public delegate void OnPositionCompleteHandler(Vector3 newPos);
        public event OnPositionCompleteHandler OnPositionComplete;

        // public delegate void OnRotationUpdateHandler(Quaternion newRot);
        // public event OnRotationUpdateHandler OnRotationUpdate;

        private Transform _target;

        private void Start()
        {
            cameraTargetPrefab
                .InstantiateAsync(transform.position, transform.rotation, transform.parent)
                .Completed += (obj) =>
            {
                var go = obj.Result;
                _positionAnimator = go.GetComponent<PositionAnimator>();
                _rotationAnimator = go.GetComponent<RotationAnimator>();

                _positionAnimator.Executor.OnUpdate += (newPos) =>
                {
                    OnPositionUpdate?.Invoke(newPos);
                };

                _positionAnimator.Executor.OnComplete += (newPos) =>
                {
                    OnPositionComplete?.Invoke(newPos);
                };

                _rotationAnimator.Executor.OnUpdate += (newRot) =>
                {
                    OnRotationUpdate?.Invoke(newRot);
                };

                go.GetComponent<MeshRenderer>().enabled = false;
                _target = go.transform;
            };
        }

        public void TransformTo(Vector3 position, Quaternion rotation, float duration)
        {
            OnTransformStart?.Invoke();
            _positionAnimator.AnimateTo(position, duration);
            _rotationAnimator.AnimateTo(rotation, duration);
        }

        public void ToggleTargetVisibility()
        {
            _target.GetComponent<MeshRenderer>().enabled = !_target
                .GetComponent<MeshRenderer>()
                .enabled;
        }

        public void FocusOn(Transform transform)
        {
            _target.transform.LookAt(transform);
        }

        public void Stop()
        {
            _positionAnimator.Stop();
            _rotationAnimator.Stop();
        }

        public void ApplyTransform(Transform t)
        {
            t.SetPositionAndRotation(_target.position, _target.rotation);
        }
    }
}
