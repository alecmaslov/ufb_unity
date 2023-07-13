using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace UFB.Core
{
    public class RotationAnimator : MonoBehaviour, IObjectAnimator<Quaternion>
    {
        private Interpolator<Quaternion> _executor;
        public Interpolator<Quaternion> Executor
        {
            get
            {
                if (_executor == null)
                {
                    _executor = new Interpolator<Quaternion>(
                        this, 
                        Quaternion.Slerp,
                        AnimationCallback,
                        () => transform.rotation,
                        curve == null ? new AnimationCurve() : curve 
                    );
                }
                return _executor;
            }
        }

        public Action<Quaternion> OnAnimationUpdate;
        
        public AnimationCurve curve;

        private void AnimationCallback(Quaternion newRotation)
        {
            transform.rotation = newRotation;
            if (OnAnimationUpdate != null)
            {
                OnAnimationUpdate(newRotation);
            }
        }

        public void AnimateTo(Quaternion rotation, float duration) => Executor.LerpTo(rotation, duration);
        public void AnimateToSnapshot(string key, float duration) => Executor.LerpToSnapshot(key, duration);


        public void SetSnapshot(string key) => Executor.SetSnapshot(transform.rotation, key);
        public void SetSnapshot(Quaternion rotation, string key) => Executor.SetSnapshot(rotation, key);

        public void SetSnapshot(Quaternion rotation, string key, float duration)
        {
            Executor.SetSnapshot(rotation, key);
            Executor.LerpToSnapshot(key, duration);
        }

        public Quaternion GetSnapshot(string key) => Executor.GetSnapshot(key);

        public void Stop() => Executor.Stop();
        public void Resume() => Executor.Resume();
        public void AddUpdateListener(Action<Quaternion> listener) => OnAnimationUpdate += listener;
        public void RemoveUpdateListener(Action<Quaternion> listener) => OnAnimationUpdate -= listener;
    }
}