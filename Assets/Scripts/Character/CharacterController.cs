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
using UFB.Network.RoomMessageTypes;
using UFB.Effects;

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
            EventBus.Publish(new CameraOrbitAroundEvent(_model.transform, 0.3f));
            await AnimationDispatcher.PlayAnimationAsync("Entrance", "CharacterIdle");
            new RippleTilesEffect(CurrentTile, 20, 1f).Execute();
        }

        public async Task MoveAlongPath(IEnumerable<Tile> path, float speed = 0.1f)
        {
            IsMoving = true;
            // detach from current tile
            transform.parent = null;
            // wait for it to hop up
            await AnimationDispatcher.PlayAnimationAsync("HopStart", "Moving", 2f);

            foreach (Tile tile in path.Skip(0))
            {
                var thisTile = tile;
                var destination = thisTile.Position;
                thisTile.Stretch(1.2f, speed * 0.5f);
                _positionAnimator.AnimateTo(destination, speed);
                await Task.Delay((int)(speed * 1000));
                thisTile.ResetStretch(2f);
            }
            await AnimationDispatcher.PlayAnimationAsync("HopEnd", "CharacterIdle", 2f);
            path.Last().AttachGameObject(gameObject, true);
            IsMoving = false;
            new RippleTilesEffect(CurrentTile, 20, 1f).Execute();
        }

        public void ForceMoveToTile(
            Tile destination,
            float duration = 0.5f,
            Action onComplete = null
        )
        {
            EventBus.Publish(new CameraOrbitAroundEvent(_model.transform, 0.3f));

            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    "forceMove",
                    new RequestMoveMessage
                    {
                        tileId = destination.Id,
                        destination = destination.Coordinates
                    }
                )
            );
        }
    }
}
