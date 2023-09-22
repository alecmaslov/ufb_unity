using UnityEngine;

namespace UFB.Core
{
    [System.Serializable]
    public class RangedValue
    {
        public float value = 0;
        public float minValue = 0;
        public float maxValue = 1;

        public delegate void OnValueChangedDelegate(float newValue);
        public event OnValueChangedDelegate OnValueChanged;

        public RangedValue() {}

        public RangedValue(float value, float minValue = 0, float maxValue = 1)
        {
            this.value = value;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public float Percent => value / maxValue;

        public void Set(float value)
        {
            this.value = Mathf.Clamp(value, minValue, maxValue);
        }

        public void Add(float amount)
        {
            value += amount;
            if (value > maxValue) value = maxValue;
        }

        public void Subtract(float amount)
        {
            value -= amount;
            if (value < 0) value = 0;
        }
    }

}