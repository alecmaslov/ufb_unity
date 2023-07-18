using UnityEngine;
using UFB.Entities;
using System.Collections.Generic;
using UFB.Player;

namespace UFB.Effects
{
    [System.Serializable]
    public class PlayerPillarsEffect : IEffect
    {
        private readonly PlayerManager _playerManager;
        private bool _isRising = false;

        public float Height = 50f;
        public float DurationRise = 300f;
        public float RandomDelayRange = 0.5f;

        public PlayerPillarsEffect(
            PlayerManager playerManager,
            float height = 50f,
            float durationRise = 300f,
            float randomDelayRange = 0.5f)
        {
            _playerManager = playerManager;
            DurationRise = durationRise;
            Height = height;
            RandomDelayRange = randomDelayRange;
        }

        public void Execute()
        {
            float delay = Random.Range(0f, RandomDelayRange);
            if (_isRising)
            {
                _playerManager.IteratePlayers(player =>
                {
                    CoroutineHelpers.DelayedAction(() =>
                    {
                        player.CurrentTile?.SlamDown();
                    }, delay, _playerManager);
                });
            }
            else
            {
                _playerManager.IteratePlayers(player =>
                {
                    CoroutineHelpers.DelayedAction(() =>
                    {
                        player.CurrentTile?.Stretch(Height, DurationRise);
                    }, delay, _playerManager);
                });
            }
            _isRising = !_isRising;
        }
    }

}