using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UFB.Core;
using SchemaTest.InstanceSharingTypes;
using UFB.Events;
using Newtonsoft.Json;

namespace UFB.Events
{
    public class CameraOrbitAroundEvent
    {
        // this should also contain a CameraState 
        // so the camera can move to that location
        public Transform target;
        public float duration;

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
}

[RequireComponent(typeof(OrbitCamera))]
[RequireComponent(typeof(ZoomController))]
[RequireComponent(typeof(PositionAnimator))]
[RequireComponent(typeof(RotationAnimator))]
public class CameraController : MonoBehaviour
{
    public ZoomController Zoom { get; private set; }
    public OrbitCamera Orbit { get; private set; }
    public CameraState topDownState;

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
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<CameraOrbitAroundEvent>(OnOrbitAroundEvent);
        EventBus.Unsubscribe<SetCameraStateEvent>(OnSetCameraStateEvent);
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
