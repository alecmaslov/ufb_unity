using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace UFB.Core
{
    public class AnimationDispatcher
    {
        public readonly Animator animator;
        private CancellationTokenSource _cts;
        private bool _isAnimating = false;
        private string _currentTriggerName;

        public AnimationDispatcher(Animator animator)
        {
            this.animator = animator ?? throw new ArgumentNullException(nameof(animator));
        }

        public async Task PlayAnimationAsync(
            string triggerName,
            string targetStateName,
            float speed = 1.0f, // Default to normal speed
            Action onComplete = null
        )
        {
            if (_isAnimating)
            {
                throw new InvalidOperationException("Another animation is already in progress.");
            }

            _isAnimating = true;
            _currentTriggerName = triggerName;
            _cts = new CancellationTokenSource();

            try
            {
                animator.speed = speed; // Set the animation speed
                animator.ResetTrigger(triggerName); // Reset any existing trigger
                animator.SetTrigger(triggerName);
                await MonitorAnimationAsync(targetStateName, _cts.Token);
                onComplete?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Animation was cancelled.");
            }
            finally
            {
                animator.speed = 1.0f; // Reset the animation speed to default
                _isAnimating = false;
                _currentTriggerName = null;
            }
        }

        private async Task MonitorAnimationAsync(
            string targetStateName,
            CancellationToken cancellationToken
        )
        {
            while (
                animator != null
                && (
                    !animator.GetCurrentAnimatorStateInfo(0).IsName(targetStateName)
                    || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f
                )
            )
            {
                // Debug.Log(
                //     $"Animation is still playing: {animator.GetCurrentAnimatorStateInfo(0).normalizedTime}"
                // );
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }
        }

        public void CancelAnimation(float? finishDuration, Action onComplete = null)
        {
            if (!_isAnimating || animator == null)
                return;

            animator.ResetTrigger(_currentTriggerName);

            if (finishDuration.HasValue)
            {
                var currentInfo = animator.GetCurrentAnimatorStateInfo(0);
                var remainingPercentage = 1.0f - currentInfo.normalizedTime;

                if (remainingPercentage > 0 && finishDuration.Value > 0)
                {
                    float speed = remainingPercentage / finishDuration.Value;
                    animator.speed = speed;
                }

                _cts.Token.Register(() =>
                {
                    animator.speed = 1.0f;
                    onComplete?.Invoke();
                });
                _cts.CancelAfter(TimeSpan.FromSeconds(finishDuration.Value));
            }
            else
            {
                animator.speed = 1.0f;
                _cts?.Cancel();
                onComplete?.Invoke();
            }
        }
    }
}
