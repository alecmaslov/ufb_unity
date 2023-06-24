using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ManagedCoroutine {
    public float Duration { get; private set; }
    private MonoBehaviour _owner;
    private Coroutine _coroutine;
    public ManagedCoroutine(MonoBehaviour owner, IEnumerator task, float duration, float delay = 0f) {
        this._owner = owner;
        this.Duration = duration;
        if (delay > 0) {
            _coroutine = owner.StartCoroutine(DelayedStart(() => { _coroutine = owner.StartCoroutine(task); }, delay));
        } else {
            _coroutine = owner.StartCoroutine(task);
        }
    }

    private IEnumerator DelayedStart(Action task, float delay) {
        yield return new WaitForSeconds(delay);
        task.Invoke();
    }

    public void Stop() {
        if (_coroutine != null)
            _owner.StopCoroutine(_coroutine);
    }
}

public class CoroutineManager {

    protected MonoBehaviour _owner;
    protected Dictionary<string, ManagedCoroutine> _activeCoroutines = new Dictionary<string, ManagedCoroutine>();
    
    public CoroutineManager(MonoBehaviour owner) {
        this._owner = owner;
    }

    public void StopAllCoroutines() {
        foreach (var coroutine in _activeCoroutines.Values)
            coroutine.Stop();
        _activeCoroutines.Clear();
    }

    private void AddCoroutine(string id, ManagedCoroutine coroutine) {
        if (!_activeCoroutines.ContainsKey(id))
            _activeCoroutines[id] = coroutine;
        else {
            _activeCoroutines[id].Stop();
            _activeCoroutines[id] = coroutine;
        }
    }

    private void RemoveCoroutine(string id) {
        if (_activeCoroutines.ContainsKey(id)) {
            _activeCoroutines[id].Stop();
            _activeCoroutines.Remove(id);
        }
    }

    /// <summary>
    /// Pass an arbitrary action for MonoBehavior owner to take on start, onupdate, and on complete, over specified duration
    /// </summary>
    public void TimedAction(string id, Action<float> onUpdate, float duration, Action onStart = null, Action onComplete = null, float delay = 0f) {
        if (id == null)
            id = "TimedAction";

        if (_activeCoroutines.ContainsKey(id))
            _activeCoroutines[id].Stop();

        _activeCoroutines[id] = new ManagedCoroutine(
            _owner, 
            TimedActionCoroutine(id, onStart, onUpdate, onComplete, duration),
            duration,
            delay
        );
    }

    private IEnumerator TimedActionCoroutine(
            string id,
            Action onStart, 
            Action<float> onUpdate, 
            Action onComplete, 
            float duration) {

        onStart?.Invoke();
        float time = 0f;
        while (time < duration) {
            onUpdate?.Invoke(time/duration);
            time += Time.deltaTime;
            yield return null;
        }
        onComplete?.Invoke();
        _activeCoroutines.Remove(id);
        // onCleanup?.Invoke();
    }
}
