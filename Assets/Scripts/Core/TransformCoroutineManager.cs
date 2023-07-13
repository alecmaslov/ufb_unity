using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TransformType
{
    Position,
    Rotation,
    Scale
}


/// eventually use this instead of a dictionary in TransformCoroutineManager
public class CoroutineDuration
{
    public float Duration { get; set; }
    public TransformType TransformType { get; set; }
}


public class TransformCoroutineManager : CoroutineManager
{
    public TransformSnapshot TargetSnapshot => _targetSnapshot;

    public TransformSnapshot CurrentSnapshot => new TransformSnapshot(_useTransform);

    public Dictionary<string, TransformSnapshot> ReferenceSnapshots
    {
        get
        {
            if (_referenceSnapshots == null)
            {
                _referenceSnapshots = new Dictionary<string, TransformSnapshot>();
            }
            return _referenceSnapshots;
        }
    }

    private Dictionary<string, TransformSnapshot> _referenceSnapshots; // stored for later use
    private TransformSnapshot _targetSnapshot;
    private bool _useLocal;

    private Dictionary<TransformType, float> _cachedDurations = new Dictionary<TransformType, float>();
    private Action _onTransformStart;
    private Action _onTransformComplete;

    private bool _shouldBreakCoroutine = false;

    private Transform _useTransform;

    private Dictionary<TransformType, Action<float>> _updateActions = new Dictionary<TransformType, Action<float>>();


    public TransformCoroutineManager(
        MonoBehaviour owner, 
        Action onTransformStart = null, 
        Action onTransformComplete = null, 
        bool useLocal = false, Transform useTransform = null) : base(owner)
    {
        if (useTransform == null) useTransform = owner.transform;
        
        this._useTransform = useTransform;
        Vector3 position = useLocal ? useTransform.localPosition : useTransform.position;
        Quaternion rotation = useLocal ? useTransform.localRotation : useTransform.rotation;
        this._targetSnapshot = new TransformSnapshot(position, rotation, useTransform.localScale);
        this._onTransformStart = onTransformStart;
        this._onTransformComplete = onTransformComplete;
    }

    /// <summary>
    /// Attach a listener to a given TransformType, such as postion, scale, or rotation
    /// </summary> 
    public void AddUpdateListener(TransformType type, Action<float> action)
    {
        if (!_updateActions.ContainsKey(type)) _updateActions[type] = null;
        _updateActions[type] += action;
    }

    public void RemoveUpdateListener(TransformType type, Action<float> action)
    {
        _updateActions[type] -= action;
    }

    /// <summary>
    /// Add a reference snapshot at a given TransformSnapshot, referenced by key
    /// </summary>
    public void AddReferenceSnapshot(string key, TransformSnapshot snapshot)
    {
        ReferenceSnapshots[key] = snapshot;
    }

    /// <summary>
    /// Add a reference snapshot at the current transform, referenced by key
    /// </summary>
    public void AddReferenceSnapshot(string key)
    {
        ReferenceSnapshots[key] = CurrentSnapshot;
    }

    public void TransformToReferenceSnapshot(string key, float duration = 1f)
    {
        if (!ReferenceSnapshots.ContainsKey(key))
        {
            Debug.LogError($"No reference snapshot found for key {key}");
            return;
        }
        TransformTo(ReferenceSnapshots[key], duration);
    }

    public Vector3 GetReferencePosition(string key)
    {
        if (!ReferenceSnapshots.ContainsKey(key))
        {
            Debug.LogError($"No reference snapshot found for key {key}");
            return Vector3.zero;
        }
        return ReferenceSnapshots[key].Position;
    }

    public Quaternion GetReferenceRotation(string key)
    {
        if (!ReferenceSnapshots.ContainsKey(key))
        {
            Debug.LogError($"No reference snapshot found for key {key}");
            return Quaternion.identity;
        }
        return ReferenceSnapshots[key].Rotation;
    }

    public Vector3 GetReferenceScale(string key)
    {
        if (!ReferenceSnapshots.ContainsKey(key))
        {
            Debug.LogError($"No reference snapshot found for key {key}");
            return Vector3.zero;
        }
        return ReferenceSnapshots[key].Scale;
    }

    /// <summary>
    /// Efficiently update position of owner MonoBehavior transform 
    /// </summary>
    public void MoveTo(Vector3 position, float duration = 1f)
    {
        if (Vector3.Equals(_targetSnapshot.Position, position))
            return;
        _targetSnapshot.Position = position;
        // check if the dict contains the timed position, also redundantly check if the coroutine is null (probably don't need this)
        if (_activeCoroutines.ContainsKey("TimedPosition") && _activeCoroutines["TimedPosition"] != null)
        {
            // Debug.Log($"Coroutine exists, extending duration by {duration}");
            // just extend the duration out, and update the target position
            // look into more ways to extend this functionality, so that the extension is logarithmic
            // or changes in a a way that is more expected
            _cachedDurations[TransformType.Position] += duration;
            return;
        }
        _cachedDurations[TransformType.Position] = duration;
        // if the coroutine is null, or the coroutine is not in the dict, then start a new coroutine
        _activeCoroutines["TimedPosition"] = new ManagedCoroutine(
            _owner,
            TimedCachedDurationCoroutine(
                "TimedPosition",
                (float t) =>
                {
                    if (_useLocal)
                    {
                        _useTransform.localPosition = Vector3.Lerp(_useTransform.localPosition, _targetSnapshot.Position, t);
                    }
                    else
                    {
                        _useTransform.position = Vector3.Lerp(_useTransform.position, _targetSnapshot.Position, t);
                    }
                },
                duration,
                TransformType.Position
            ),
            duration
        );
    }

    /// <summary>
    /// Efficiently update rotation of owner MonoBehavior transform 
    /// </summary>
    public void RotateTo(Quaternion rotation, float duration = 1f)
    {
        if (Quaternion.Equals(_targetSnapshot.Rotation, rotation))
            return;
        _targetSnapshot.Rotation = rotation;
        if (_activeCoroutines.ContainsKey("TimedRotation") && _activeCoroutines["TimedRotation"] != null)
        {
            // Debug.Log($"Coroutine exists, extending duration by {duration}");
            _cachedDurations[TransformType.Rotation] += duration;
            return;
        }
        _cachedDurations[TransformType.Rotation] = duration;
        _activeCoroutines["TimedRotation"] = new ManagedCoroutine(
            _owner,
            TimedCachedDurationCoroutine(
                "TimedRotation",
                (float t) =>
                {
                    if (_useLocal)
                    {
                        _useTransform.localRotation = Quaternion.Lerp(_useTransform.localRotation, _targetSnapshot.Rotation, t);
                    }
                    else
                    {
                        _useTransform.rotation = Quaternion.Lerp(_useTransform.rotation, _targetSnapshot.Rotation, t);
                    }
                },
                duration,
                TransformType.Rotation
            ),
            duration
        );
    }
    /// <summary>
    /// Efficiently update scale of owner MonoBehavior transform 
    /// </summary>
    public void ScaleTo(Vector3 scale, float duration = 1f)
    {
        if (Vector3.Equals(_targetSnapshot.Scale, scale))
            return;
        _targetSnapshot.Scale = scale;
        if (_activeCoroutines.ContainsKey("TimedScale") && _activeCoroutines["TimedScale"] != null)
        {
            // Debug.Log($"Coroutine exists, extending duration by {duration}");
            _cachedDurations[TransformType.Scale] += duration;
            return;
        }
        _cachedDurations[TransformType.Scale] = duration;
        _activeCoroutines["TimedScale"] = new ManagedCoroutine(
            _owner,
            TimedCachedDurationCoroutine(
                "TimedScale",
                (float t) => { _useTransform.localScale = Vector3.Lerp(_useTransform.localScale, _targetSnapshot.Scale, t); },
                duration,
                TransformType.Scale
            ),
            duration
        );
    }

    public void ResetDurations(float duration = 1f)
    {
        _cachedDurations[TransformType.Position] = duration;
        _cachedDurations[TransformType.Rotation] = duration;
        _cachedDurations[TransformType.Scale] = duration;
    }

    public void TransformTo(TransformSnapshot snapshot, float duration = 1f)
    {
        MoveTo(snapshot.Position, duration);
        RotateTo(snapshot.Rotation, duration);
        ScaleTo(snapshot.Scale, duration);
    }

    public void SetTargetTransform(TransformSnapshot snapshot)
    {
        _targetSnapshot.Position = snapshot.Position;
        _targetSnapshot.Rotation = snapshot.Rotation;
        _targetSnapshot.Scale = snapshot.Scale;
    }

    public void Freeze(TransformType? transformType = null)
    {
        if (transformType == null)
        {
            if (_activeCoroutines.ContainsKey("TimedPosition") && _activeCoroutines["TimedPosition"] != null)
            {
                _activeCoroutines["TimedPosition"].Stop();
                _activeCoroutines["TimedPosition"] = null;
            }
            if (_activeCoroutines.ContainsKey("TimedRotation") && _activeCoroutines["TimedRotation"] != null)
            {
                _activeCoroutines["TimedRotation"].Stop();
                _activeCoroutines["TimedRotation"] = null;
            }
            if (_activeCoroutines.ContainsKey("TimedScale") && _activeCoroutines["TimedScale"] != null)
            {
                _activeCoroutines["TimedScale"].Stop();
                _activeCoroutines["TimedScale"] = null;
            }
            if (!_useLocal)
            {
                _targetSnapshot.Position = _useTransform.position;
                _targetSnapshot.Rotation = _useTransform.rotation;
            }
            else
            {
                _targetSnapshot.Position = _useTransform.localPosition;
                _targetSnapshot.Rotation = _useTransform.localRotation;
            }
            _targetSnapshot.Scale = _useTransform.localScale;
            _onTransformComplete?.Invoke();
            return;
        }
        switch (transformType)
        {
            case TransformType.Position:
                _activeCoroutines["TimedPosition"].Stop();
                _activeCoroutines["TimedPosition"] = null;
                if (!_useLocal)
                {
                    _targetSnapshot.Position = _useTransform.position;
                }
                else
                {
                    _targetSnapshot.Position = _useTransform.localPosition;
                }
                break;
            case TransformType.Rotation:
                _activeCoroutines["TimedRotation"].Stop();
                _activeCoroutines["TimedRotation"] = null;
                if (!_useLocal)
                {
                    _targetSnapshot.Rotation = _useTransform.rotation;
                }
                else
                {
                    _targetSnapshot.Rotation = _useTransform.localRotation;
                }
                break;
            case TransformType.Scale:
                _activeCoroutines["TimedScale"].Stop();
                _activeCoroutines["TimedScale"] = null;
                _targetSnapshot.Scale = _useTransform.localScale;
                break;
        }
        if (!HasActiveCoroutines())
            _onTransformComplete?.Invoke();
    }

    private bool HasActiveCoroutines()
    {
        if (_activeCoroutines.ContainsKey("TimedPosition") && _activeCoroutines["TimedPosition"] != null)
        {
            return true;
        }
        if (_activeCoroutines.ContainsKey("TimedRotation") && _activeCoroutines["TimedRotation"] != null)
        {
            return true;
        }
        if (_activeCoroutines.ContainsKey("TimedScale") && _activeCoroutines["TimedScale"] != null)
        {
            return true;
        }
        return false;
    }

    private IEnumerator TimedCachedDurationCoroutine(
            string id,
            Action<float> onUpdate,
            float duration,
            TransformType transformType)
    {
        _onTransformStart?.Invoke();
        float time = 0f;
        while (time < _cachedDurations[transformType])
        {
            if (_shouldBreakCoroutine)
            {
                _shouldBreakCoroutine = false;
                break;
            }
            onUpdate?.Invoke(time / _cachedDurations[transformType]);
            if (_updateActions.ContainsKey(transformType)) {
                _updateActions[transformType]?.Invoke(time / _cachedDurations[transformType]);
            }
            time += Time.deltaTime;
            yield return null;
        }
        _activeCoroutines.Remove(id);
        if (!HasActiveCoroutines())
            _onTransformComplete?.Invoke();
    }

}