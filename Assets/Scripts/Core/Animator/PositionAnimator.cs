using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

namespace UFB.Core
{
    // public interface IAnimator
    // {
    //     void Stop();
    //     void Resume();
    // }
    
    public class PositionAnimator : MonoBehaviour, IObjectAnimator<Vector3>
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
                        () => transform.position,
                        curve == null ? new AnimationCurve() : curve 
                    );
                }
                return _executor;
            }
        }

        public Action<Vector3> OnAnimationUpdate;
        public AnimationCurve curve;

        private void AnimationCallback(Vector3 newPos)         
        {
            transform.position = newPos;
            if (OnAnimationUpdate != null)
            {
                OnAnimationUpdate(newPos);
            }
        }

        public void AnimateTo(Vector3 position, float duration) => Executor.LerpTo(position, duration);
        public void AnimateToSnapshot(string key, float duration) => Executor.LerpToSnapshot(key, duration);


        public void SetSnapshot(string key) => Executor.SetSnapshot(transform.position, key);
        public void SetSnapshot(Vector3 scale, string key) => Executor.SetSnapshot(scale, key);

        public void SetSnapshot(Vector3 position, string key, float duration)
        {
            Executor.SetSnapshot(position, key);
            Executor.LerpToSnapshot(key, duration);
        }

        public Vector3 GetSnapshot(string key) => Executor.GetSnapshot(key);
 
        public void Stop() => Executor.Stop();
        public void Resume() => Executor.Resume();
        public void AddUpdateListener(Action<Vector3> listener) => OnAnimationUpdate += listener;
        public void RemoveUpdateListener(Action<Vector3> listener) => OnAnimationUpdate -= listener;
    }
}