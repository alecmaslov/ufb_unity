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
        public Transform target;
        public float duration;
        public CameraOrbitAroundEvent(Transform target, float duration)
        {
            this.target = target;
            this.duration = duration;
        }
    }
}

[System.Serializable]
public struct CameraState
{
    // Individual float components for position and rotation
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ, rotW;
    public float zoom;
    public float transitionDuration;

    // Constructor using Vector3 and Quaternion
    public CameraState(Vector3 position, Quaternion rotation, float zoom, float transitionDuration = 1f)
    {
        this.posX = position.x;
        this.posY = position.y;
        this.posZ = position.z;
        this.rotX = rotation.x;
        this.rotY = rotation.y;
        this.rotZ = rotation.z;
        this.rotW = rotation.w;
        this.zoom = zoom;
        this.transitionDuration = transitionDuration;
    }

    // Constructor using CameraController
    public CameraState(CameraController cameraController, float transitionDuration = 1f)
    {
        Vector3 position = cameraController.transform.position;
        Quaternion rotation = cameraController.transform.rotation;

        this.posX = position.x;
        this.posY = position.y;
        this.posZ = position.z;
        this.rotX = rotation.x;
        this.rotY = rotation.y;
        this.rotZ = rotation.z;
        this.rotW = rotation.w;
        this.zoom = cameraController.Zoom.CurrentZoom;
        this.transitionDuration = transitionDuration;
    }

    // Convenience methods for getting the Vector3 position and Quaternion rotation
    public Vector3 GetPosition()
    {
        return new Vector3(posX, posY, posZ);
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(rotX, rotY, rotZ, rotW);
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

    private PositionAnimator _positionAnimator;
    private RotationAnimator _rotationAnimator;

    public bool UseOrbit
    {
        get
        {
            return _useOrbit;
        }
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

    private bool _useOrbit = true;
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
        EventBus.Subscribe<CameraOrbitAroundEvent>(OrbitAround);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<CameraOrbitAroundEvent>(OrbitAround);
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

    public void OrbitAround(CameraOrbitAroundEvent orbitAroundEvent)
    {
        OrbitAround(orbitAroundEvent.target);
    }

    public void SaveCameraState(string fileName)
    {
        var json = JsonConvert.SerializeObject(new CameraState(this));
        ApplicationData.SaveJSON(json, "presets/camera", fileName + ".json");
    }

    public void LoadCameraState(CameraState cameraState)
    {
        TransformTo(cameraState.GetPosition(), cameraState.GetRotation(), cameraState.transitionDuration);
        Debug.Log($"Restoring zoom: {cameraState.zoom} | {cameraState.transitionDuration}s");
        Zoom.ZoomTo(cameraState.zoom, cameraState.transitionDuration);
    }

    public void LoadCameraState(string fileName)
    {
        var cameraState = ApplicationData.LoadJSON<CameraState>("presets/camera", fileName + ".json");
        LoadCameraState(cameraState);
    }
}