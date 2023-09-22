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
using System.Threading;

namespace UFB.Character
{
    [RequireComponent(typeof(TileAttachable))]
    [RequireComponent(typeof(PositionAnimator))]
    public class CharacterController : MonoBehaviour
    {
        public UfbCharacter Character { get; private set; }
        public PlayerState State { get; private set; }
        public CharacterMovement Movement { get; private set; }

        private GameObject _model;
        private AnimationDispatcher _animationDispatcher;

        public async void Spawn(UfbCharacter character, PlayerState playerState)
        {
            Character = character;
            State = playerState;
            // spawn the prefab
            var task = Addressables.InstantiateAsync(character.modelPrefab, transform);
            await task.Task;
            _model = task.Result;

            _animationDispatcher = new AnimationDispatcher(_model.GetComponent<Animator>());
            Movement = new CharacterMovement(this, _animationDispatcher);
        }
    }
}