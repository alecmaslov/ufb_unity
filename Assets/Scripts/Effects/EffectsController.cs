using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UFB.Effects
{
    public class EffectsController : MonoBehaviour
    {
        private Dictionary<string, IEffect> _effects;

        private void Awake()
        {
            _effects = new Dictionary<string, IEffect>();
        }

        public void RegisterEffect(string effectName, IEffect effect)
        {
            _effects[effectName] = effect;
        }

        public void RunEffect(string effectName)
        {
            if (_effects.TryGetValue(effectName, out var effect))
            {
                effect.Execute();
            }
            else
            {
                Debug.Log($"Effect {effectName} not found");
            }
        }
    }

}

