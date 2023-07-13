using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace UFB.Core
{
    
    public class ScaleAnimator : MonoBehaviour, IObjectAnimator<Vector3>
    {
        private Interpolator<Vector3> _executor;
        public Interpolator<Vector3> Executor
        {
            get
            {
                if (_executor == null)
                {
                    _executor = new Interpolator<Vector3>(
                        this, 
                        Vector3.Lerp,
                        AnimationCallback,
                        () => transform.localScale,
                        curve == null ? new AnimationCurve() : curve 
                    );
                }
                return _executor;
            }
        }

        public Action<Vector3> OnAnimationUpdate;

        public AnimationCurve curve;

        private void AnimationCallback(Vector3 newScale)
        {
            transform.localScale = newScale;
            if (OnAnimationUpdate != null)
            {
                OnAnimationUpdate(newScale);
            }
        }

        public void AnimateTo(Vector3 scale, float duration) => Executor.LerpTo(scale, duration);
        public void AnimateToSnapshot(string key, float duration) => Executor.LerpToSnapshot(key, duration);

        public void SetSnapshot(string key) => Executor.SetSnapshot(transform.localScale, key);
        public void SetSnapshot(Vector3 scale, string key) => Executor.SetSnapshot(scale, key);

        public void SetSnapshot(Vector3 scale, string key, float duration)
        {
            Executor.SetSnapshot(scale, key);
            Executor.LerpToSnapshot(key, duration);
        }

        public Vector3 GetSnapshot(string key) => Executor.GetSnapshot(key);

        public void Stop() => Executor.Stop();
        public void Resume() => Executor.Resume();
        public void AddUpdateListener(Action<Vector3> listener) => OnAnimationUpdate += listener;
        public void RemoveUpdateListener(Action<Vector3> listener) => OnAnimationUpdate -= listener;
    }
}