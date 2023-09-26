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
using System.Linq;
using UFB.Events;

namespace UFB.Character
{
    [RequireComponent(typeof(TileAttachable))]
    [RequireComponent(typeof(PositionAnimator))]
    public class CharacterController : MonoBehaviour
    {
        public string Id => State.id;
        public UfbCharacter Character { get; private set; }
        public CharacterState State { get; private set; }
        public AnimationDispatcher AnimationDispatcher { get; private set; }
        public bool IsMoving { get; private set; }
        public Tile CurrentTile { get; private set; }

        // TODO: Implement ID based tile lookup instead!
        // public Tile CurrentTile =>
        //     ServiceLocator.Current.Get<GameBoard>().Tiles[State.currentTileId];


        // .GetTileByCoordinates(State.coordinates.ToCoordinates());

        private GameObject _model;
        private PositionAnimator _positionAnimator;

        private void Awake()
        {
            _positionAnimator = GetComponent<PositionAnimator>();
        }

        public async Task Initialize(
            UfbCharacter character,
            CharacterState characterState,
            bool playEntrance = true
        )
        {
            Character = character;
            State = characterState;
            name = character.name + "_" + characterState.id;

            // spawn the prefab
            _model = await Addressables.InstantiateAsync(character.modelPrefab, transform).Task;
            AnimationDispatcher = new AnimationDispatcher(_model.GetComponent<Animator>());

            CurrentTile = ServiceLocator.Current.Get<GameBoard>().Tiles[State.currentTileId];
            CurrentTile.AttachGameObject(gameObject, true);

            State.OnCurrentTileIdChange(
                (newTileId, oldTileId) =>
                {
                    Debug.Log($"Character {Id} moved to tile {newTileId} from {oldTileId}");
                    CurrentTile = ServiceLocator.Current.Get<GameBoard>().Tiles[newTileId];
                }
            );

            if (playEntrance)
                PlayEntranceAnimation();
        }

        public async void PlayEntranceAnimation()
        {
            try
            {
                Debug.Log("Playing entrance animation!");
                // set to high above the map for dramatic entrance
                // transform.position += new Vector3(0, 10f);

                // EventBus.Publish
                EventBus.Publish(new CameraOrbitAroundEvent(_model.transform, 0.3f));

                // await AnimationDispatcher.PlayAnimationAsync("HopStart", "Moving");
                // await AnimationDispatcher.PlayAnimationAsync("Entrance", "CharacterIdle");

                Debug.Log("Entrance animation complete!");

                // GetComponent<PositionAnimator>()
                //     .AnimateTo(
                //         CurrentTile.Position,
                //         0.75f,
                //         () =>
                //         {
                //             transform.position = CurrentTile.Position;
                //         }
                //     );
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public async Task MoveAlongPath(IEnumerable<Tile> path, float speed = 0.1f)
        {
            IsMoving = true;

            // detach from current tile
            transform.parent = null;

            // wait for it to hop up
            await AnimationDispatcher.PlayAnimationAsync("HopStart", "Moving");

            foreach (Tile tile in path.Skip(0))
            {
                var thisTile = tile;
                var destination = thisTile.Position;
                thisTile.Stretch(1.5f, speed * 0.5f);
                _positionAnimator.AnimateTo(destination, speed);
                await Task.Delay((int)(speed * 1000));
                thisTile.ResetStretch(3f);
            }
            await AnimationDispatcher.PlayAnimationAsync("HopEnd", "CharacterIdle");
            path.Last().AttachGameObject(gameObject, true);
            IsMoving = false;
        }

        private async Task MoveToTileAsync(Tile tile)
        {
            var tcs = new TaskCompletionSource<bool>();
            Debug.Log($"Moving to tile {tile.Id}");
            _positionAnimator.AnimateTo(
                tile.Position,
                0.5f,
                () =>
                {
                    Debug.Log("Hop To Tile Complete");
                    tcs.SetResult(true);
                }
            );

            await tcs.Task;
        }

        public void ForceMoveToTile(
            Tile destination,
            float duration = 0.5f,
            Action onComplete = null
        )
        {
            // _tileAttachable.DetachFromTile();
            _positionAnimator.AnimateTo(
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
