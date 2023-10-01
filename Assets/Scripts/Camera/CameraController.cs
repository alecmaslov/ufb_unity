using UnityEngine;
using UFB.Core;
using UFB.Events;
using Newtonsoft.Json;
using UFB.Camera;

namespace UFB.Events
{
    public class CameraOrbitAroundEvent
    {
        // this should also contain a CameraState
        // so the camera can move to that location
        public Transform target;
        public float duration = 0.5f;

        public CameraOrbitAroundEvent(Transform target, float duration)
        {
            this.target = target;
            this.duration = duration;
        }
    }

    public class SetCameraStateEvent
    {
        public CameraState cameraState;

        public SetCameraStateEvent(CameraState cameraState)
        {
            this.cameraState = cameraState;
        }
    }

    public class SetCameraPresetStateEvent
    {
        public CameraController.PresetState presetState;
    }
}

namespace UFB.Camera
{
    [RequireComponent(typeof(OrbitCamera))]
    [RequireComponent(typeof(ZoomController))]
    [RequireComponent(typeof(PositionAnimator))]
    [RequireComponent(typeof(RotationAnimator))]
    public class CameraController : MonoBehaviour
    {
        public ZoomController Zoom { get; private set; }
        public OrbitCamera Orbit { get; private set; }
        public CameraState topDownState;

        public enum PresetState
        {
            TopDown,
            Front,
            Back,
            Left,
            Right,
        }

        private PositionAnimator _positionAnimator;
        private RotationAnimator _rotationAnimator;

        public bool UseOrbit
        {
            get { return _useOrbit; }
            set
            {
                if (value && _positionableMoving)
                {
                    _positionAnimator.Stop();
                    _rotationAnimator.Stop();
                    Orbit.SolvePosition(transform.position);
                }
                _useOrbit = value;
            }
        }

        private bool _useOrbit = false;
        private bool _positionableMoving = false;

        private void Awake()
        {
            Zoom = GetComponent<ZoomController>();

            Orbit = GetComponent<OrbitCamera>();
            _positionAnimator = GetComponent<PositionAnimator>();
            _rotationAnimator = GetComponent<RotationAnimator>();

            _positionAnimator.Executor.OnComplete += (Vector3 newPos) =>
            {
                Debug.Log($"[CameraController] Position complete: {newPos} | Solving position");
                // _orbitCamera.SolvePosition(newPos);
                _positionableMoving = false;
            };
        }

        private void OnEnable()
        {
            EventBus.Subscribe<CameraOrbitAroundEvent>(OnOrbitAroundEvent);
            EventBus.Subscribe<SetCameraStateEvent>(OnSetCameraStateEvent);
            EventBus.Subscribe<SetCameraPresetStateEvent>(OnCameraSetPresetStateEvent);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CameraOrbitAroundEvent>(OnOrbitAroundEvent);
            EventBus.Unsubscribe<SetCameraStateEvent>(OnSetCameraStateEvent);
            EventBus.Unsubscribe<SetCameraPresetStateEvent>(OnCameraSetPresetStateEvent);
        }

        private void Update()
        {
            // the positionable stuff will be applied to the actual object
            if (UseOrbit && !_positionableMoving)
            {
                Orbit.ApplyTransform(transform);
            }
        }

        public void StopRepositioning()
        {
            _positionAnimator.Stop();
            _rotationAnimator.Stop();
        }

        public void TransformTo(Vector3 position, Quaternion rotation, float duration)
        {
            _positionableMoving = true;
            UseOrbit = false;
            _positionAnimator.AnimateTo(position, duration);
            _rotationAnimator.AnimateTo(rotation, duration);
        }

        public void OrbitAround(Transform t)
        {
            UseOrbit = true;
            Orbit.FocusOn(t);
        }

        public void OnOrbitAroundEvent(CameraOrbitAroundEvent orbitAroundEvent)
        {
            OrbitAround(orbitAroundEvent.target);
        }

        private void OnCameraSetPresetStateEvent(SetCameraPresetStateEvent e)
        {
            switch (e.presetState)
            {
                case PresetState.TopDown:
                    LoadCameraState(topDownState);
                    break;
                case PresetState.Front:
                    break;
                case PresetState.Back:
                    break;
                case PresetState.Left:
                    break;
                case PresetState.Right:
                    break;
            }
        }

        public void OnSetCameraStateEvent(SetCameraStateEvent e) => LoadCameraState(e.cameraState);

        public void LoadCameraState(CameraState cameraState)
        {
            TransformTo(
                cameraState.GetPosition(),
                cameraState.GetRotation(),
                cameraState.transitionDuration
            );
            Debug.Log($"Restoring zoom: {cameraState.zoom} | {cameraState.transitionDuration}s");
            Zoom.ZoomTo(cameraState.zoom, cameraState.transitionDuration);
        }

        public void LoadCameraState(string fileName)
        {
            var cameraState = ApplicationData.LoadJSON<CameraState>(
                "presets/camera",
                fileName + ".json"
            );
            LoadCameraState(cameraState);
        }

        public void SaveCameraState(string fileName)
        {
            var json = JsonConvert.SerializeObject(new CameraState(this));
            ApplicationData.SaveJSON(json, "presets/camera", fileName + ".json");
        }
    }
}
