using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using System.Collections;
using UFB.StateSchema;
using UFB.Entities;
using UnityEngine;
using UFB.Core;
using System.Threading.Tasks;
using System;
using UFB.Map;
using System.Linq;
using UFB.Events;
using UFB.Network.RoomMessageTypes;
using UFB.Effects;
using UFB.Camera;
using Colyseus.Schema;

namespace UFB.Character
{
    [RequireComponent(typeof(TileAttachable))]
    [RequireComponent(typeof(PositionAnimator))]
    public class CharacterController : MonoBehaviour, ICameraFocusable, IClickable
    {
        public string Id => State.id;
        public UfbCharacter Character { get; private set; }
        public CharacterState State { get; private set; }
        public AnimationDispatcher AnimationDispatcher { get; private set; }
        public bool IsMoving { get; private set; }
        public Tile CurrentTile { get; private set; }

        private GameObject _model;
        private PositionAnimator _positionAnimator;
        private Coroutine _moveAlongPathCoroutine;

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

            // spawn the character model prefab
            var task = Addressables.InstantiateAsync(character.modelPrefab, transform);
            EventBus.Publish(new DownloadProgressEvent(task, $"Character Model"));
            _model = await task.Task;
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
            //await AnimationDispatcher.PlayAnimationAsync("HopStart", "Moving", 1f);

            if (_moveAlongPathCoroutine != null)
                StopCoroutine(_moveAlongPathCoroutine);

            CameraManager.instance.setCameraTarget(transform);

            var tcs = new TaskCompletionSource<bool>();

            _moveAlongPathCoroutine = StartCoroutine(
                MoveAlongPathCoroutine(path, speed, () => tcs.SetResult(true))
            );

            await tcs.Task;
            await AnimationDispatcher.PlayAnimationAsync("HopEnd", "CharacterIdle", 1f);
            path.Last().AttachGameObject(gameObject, true);
            IsMoving = false;
            
            CameraManager.instance.setCameraTarget(null);

        }

        private IEnumerator MoveAlongPathCoroutine(
            IEnumerable<Tile> path,
            float speed = 0.1f,
            Action onComplete = null
        )
        {
            foreach (Tile tile in path.Skip(0))
            {
                var thisTile = tile;
                var destination = thisTile.Position;
                if (tile != path.Last())
                {
                    thisTile.Stretch(0.5f, speed * 0.5f);
                }
                bool isFinished = false;
                _positionAnimator.AnimateTo(destination, speed, () => isFinished = true);
                while (!isFinished)
                {
                    yield return null;
                }
                if (tile != path.Last())
                {
                    thisTile.ResetStretch(2f);
                }
            }

            onComplete?.Invoke();
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
                        characterId = Id,
                        tileId = destination.Id,
                        destination = destination.Coordinates
                    }
                )
            );
        }

        public void CancelMoveToTile(
            List<Item> items,
            Tile destination,
            float originEnergy,
            float duration = 0.5f,
            Action onComplete = null
        )
        {
            //EventBus.Publish(new CameraOrbitAroundEvent(_model.transform, 0.3f));

            EventBus.Publish(
                RoomSendMessageEvent.Create(
                    "cancelMove",
                    new RequestCancelMoveMessage
                    {
                        characterId = Id,
                        tileId = destination.Id,
                        destination = destination.Coordinates,
                        originEnergy = originEnergy,
                        items = items
                    }
                )
            );
        }

        public void MoveToTile(Tile tile, bool isPath = false, bool isFeather = false)
        {

            EventBus.Publish(
                new RoomSendMessageEvent(
                    "move",
                    new RequestMoveMessage
                    {
                        characterId = Id,
                        tileId = tile.Id,
                        destination = tile.Coordinates,
                        isPath = isPath,
                        isFeather = isFeather
                    }
                )
            );
            EventBus.Publish(new CancelPopupMenuEvent());
        }

        public void InitMovePos(Tile tile)
        {
            Vector3 position = tile.Position;
            _positionAnimator.AnimateTo(position, 0.5f);
            tile.AttachGameObject(gameObject);
        }

        public void OnFocus()
        {
            // EventBus.Publish(new CameraOrbitAroundEvent(_model.transform, 0.3f));
            EventBus.Publish(new SelectedCharacterEvent { controller = this });
        }

        public void OnUnfocus()
        {
            return;
        }

        public void OnClick()
        {
            
        }
    }
}
