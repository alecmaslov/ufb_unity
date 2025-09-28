using UFB.StateSchema;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UFB.UI
{
    public class LinearIndicatorBar : MonoBehaviour
    {
        [SerializeField]
        private Slider _slider;

        [SerializeField]
        private Text _valueText;

        [SerializeField]
        private Text _maxValueText;

        [SerializeField]
        private RectTransform _valueIndicatorContainer;

        public string selectedId = "";

        public void SetRangedValueState(RangedValueState state, CharacterState e)
        {
            selectedId = e.characterId;
            SetFromState(state);
            state.OnChange(() =>
            {
                if (selectedId == e.characterId) 
                {
                    SetFromState(state);
                }
            });
        }

        private void SetFromState(RangedValueState state)
        {
            _slider.value = state.Percent();
            _valueText.text = state.current.ToString();
            _maxValueText.text = state.max.ToString();
        }

        public void ToggleValueIndicator(bool show)
        {
            _valueIndicatorContainer.gameObject.SetActive(show);
        }
    }
}
