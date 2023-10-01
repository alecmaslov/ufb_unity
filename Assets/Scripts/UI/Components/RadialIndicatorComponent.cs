using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UFB.Core;
using System;
using UFB.StateSchema;

namespace UFB.UI
{
    public class RadialIndicatorComponent : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _container;

        [SerializeField]
        private Image _radialDisplay;

        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TextMeshProUGUI _valueText;

        [SerializeField]
        private TextMeshProUGUI _maxValueText;

        [SerializeField]
        private RectTransform _valueIndicatorContainer;

        public void SetRangedValueState(RangedValueState state)
        {
            SetFromState(state);
            state.OnChange(() => SetFromState(state));
        }

        private void SetFromState(RangedValueState state)
        {
            _radialDisplay.fillAmount = state.Percent();
            _valueText.text = state.current.ToString();
            _maxValueText.text = state.max.ToString();
        }

        public void ToggleValueIndicator(bool show)
        {
            _valueIndicatorContainer.gameObject.SetActive(show);
        }
    }
}
