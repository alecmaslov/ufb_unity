using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Collections;
using UFB.StateSchema;
using UFB.Entities;
using UnityEngine;
using UFB.Player;
using UFB.Core;
using System.Threading.Tasks;
using System;
using UFB.Map;

namespace UFB.Character
{
    [RequireComponent(typeof(TileAttachable))]
    [RequireComponent(typeof(PositionAnimator))]
    public class CharacterController : MonoBehaviour
    {
        public string Id => State.id;
        public UfbCharacter Character { get; private set; }
        public CharacterState State { get; private set; }
        public bool IsMoving { get; private set; }

        public AnimationDispatcher _animationDispatcher;
        private GameObject _model;

        public async Task Spawn(UfbCharacter character, CharacterState characterState)
        {
            Character = character;
            State = characterState;
            name = character.name + "_" + characterState.id;
            // spawn the prefab
            var task = Addressables.InstantiateAsync(character.modelPrefab, transform);
            await task.Task;
            _model = task.Result;

            _animationDispatcher = new AnimationDispatcher(_model.GetComponent<Animator>());
            // Movement = new CharacterMovement(this, AnimationDispatcher);
        }

        public async Task MoveAlongPath(IEnumerable<Tile> path)
        {
            IsMoving = true;
            // wait for it to hop up
            await _animationDispatcher.PlayAnimationAsync("HopStart", "Moving");
            // detach from the tile
            // _tileAttachable.DetachFromTile();

            foreach (Tile tile in path)
            {
                // set completion source, then resolve it when the animation completes (for each step)
                var tcs = new TaskCompletionSource<bool>();
                GetComponent<PositionAnimator>().AnimateTo(
                    tile.Position,
                    0.1f,
                    () =>
                    {
                        Debug.Log("Hop To Tile Complete");
                        tile.Stretch(2.0f, 0.1f, () => tile.ResetStretch(0.1f));
                        tcs.SetResult(true);
                    }
                );

                await tcs.Task;
            }

            await _animationDispatcher.PlayAnimationAsync("HopEnd", "Idle");
            // _tileAttachable.AttachToTile(path.Last());
            IsMoving = false;
        }

        public void ForceMoveToTile(
            Tile destination,
            float duration = 0.5f,
            Action onComplete = null
        )
        {
            // _tileAttachable.DetachFromTile();
            GetComponent<PositionAnimator>().AnimateTo(
                destination.Position,
                duration,
                () =>
                {
                    // _tileAttachable.AttachToTile(destination);
                    onComplete?.Invoke();
                }
            );
        }
    }
}
