using UnityEngine;
using System.Collections;
using UFB.Core;
using UnityEngine.AddressableAssets;

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
        cameraTargetPrefab.InstantiateAsync(transform.position, transform.rotation, transform.parent).Completed += (obj) =>
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
        _target.GetComponent<MeshRenderer>().enabled = !_target.GetComponent<MeshRenderer>().enabled;
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

        // lerp between current position and target position
        // t.position = Vector3.Lerp(t.position, _target.position, Time.deltaTime * 5);
        // t.rotation = Quaternion.Slerp(t.rotation, _target.rotation, Time.deltaTime * 5);