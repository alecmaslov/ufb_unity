using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UFB.Entities;
using UFB.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace UFB.Character
{
    // [RequireComponent(typeof(TileAttachable))]
    // [RequireComponent(typeof(PositionAnimator))]
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement
    {
        public bool IsMoving { get; private set; }

        private TileAttachable _tileAttachable;
        private PositionAnimator _positionAnimator;
        private AnimationDispatcher _animationDispatcher;

        public CharacterMovement(CharacterController characterController, AnimationDispatcher animationDispatcher)
        {
            _tileAttachable = characterController.GetComponent<TileAttachable>();
            _positionAnimator = characterController.GetComponent<PositionAnimator>();
        }

        public async void MoveAlongPath(IEnumerable<TileEntity> path)
        {
            IsMoving = true;
            // wait for it to hop up
            await _animationDispatcher.PlayAnimationAsync("HopStart", "Moving");
            // detach from the tile
            _tileAttachable.DetachFromTile();

            foreach (TileEntity tile in path)
            {
                // set completion source, then resolve it when the animation completes (for each step)
                var tcs = new TaskCompletionSource<bool>();
                _positionAnimator.AnimateTo(tile.AttachedPosition, 0.1f, () =>
                {
                    Debug.Log("Hop To Tile Complete");
                    tile.Stretch(2.0f, 0.1f, () => tile.ResetStretch(0.1f));
                    tcs.SetResult(true);
                });

                await tcs.Task;
            }

            await _animationDispatcher.PlayAnimationAsync("HopEnd", "Idle");
            _tileAttachable.AttachToTile(path.Last());
            IsMoving = false;
        }

    }
}