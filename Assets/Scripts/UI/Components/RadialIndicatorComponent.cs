using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UFB.Core;
using System;

namespace UFB.UI
{
    public class RadialIndicatorComponent : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private Image _radialDisplay;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _valueText;
        [SerializeField] private TextMeshProUGUI _maxValueText;
        [SerializeField] private RectTransform _valueIndicatorContainer;

        public RangedValue Value { get; private set; }

        public void Initialize(RangedValue value)
        {
            Value = value;
            Value.OnValueChanged += OnValueChanged;
            OnValueChanged(Value.value);
        }

        public void ToggleValueIndicator(bool show)
        {
            _valueIndicatorContainer.gameObject.SetActive(show);
        }

        private void OnValueChanged(float newValue)
        {
            _radialDisplay.fillAmount = newValue / Value.maxValue;
            _valueText.text = newValue.ToString();
            _maxValueText.text = Value.maxValue.ToString();
        }

    }

}
