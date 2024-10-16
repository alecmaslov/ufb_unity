using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UFB.Network;
using UFB.Events;
using UFB.UI;

namespace UFB.Events
{
    public class ToastMessageEvent
    {
        public string Message { get; private set; }
        public ToastColorScheme ColorScheme { get; private set; }
        public float Duration { get; private set; }
        public float AnimationDuration { get; private set; }

        public ToastMessageEvent(
            string message,
            ToastColorScheme? colorScheme = null,
            float duration = 5f,
            float animationDuration = 0.5f)
        {
            Message = message;
            ColorScheme = colorScheme ?? new ToastColorScheme(Color.black, Color.white); // Default scheme
            Duration = duration;
            AnimationDuration = animationDuration;
        }

        public ToastMessageEvent(
            string message,
            UIToast.ToastType type,
            float duration = 5f,
            float animationDuration = 0.5f)
        {
            Message = message;
            ColorScheme = UIToast.GetColorScheme(type);
            Duration = duration;
            AnimationDuration = animationDuration;
        }
    }
}

namespace UFB.UI
{
    [System.Serializable]
    public struct ToastColorScheme
    {
        public Color BackgroundColor;
        public Color TextColor;

        public ToastColorScheme(Color backgroundColor, Color textColor)
        {
            this.BackgroundColor = backgroundColor;
            this.TextColor = textColor;
        }
    }

    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    [RequireComponent(typeof(Image))]
    public class UIToast : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _toastText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private float _destinationY;
        [SerializeField] private CanvasGroup _canvasGroup;

        public enum ToastType { Message, Error, Warning }

        public static ToastColorScheme MessageScheme = new ToastColorScheme(Color.black, Color.white);
        public static ToastColorScheme ErrorScheme = new ToastColorScheme(Color.red, Color.white);
        public static ToastColorScheme WarningScheme = new ToastColorScheme(Color.yellow, Color.black);

        public void Initialize(
            string message,
            ToastType type = ToastType.Message,
            float duration = 5f,
            float animationDuration = 0.5f)
        {
            _toastText.text = message;
            ToastColorScheme colorScheme = GetColorScheme(type);
            SetColorScheme(colorScheme);
            StartCoroutine(ShowToast(duration, animationDuration));
        }

        public void Initialize(ToastMessageEvent messageEvent)
        {
            _toastText.text = messageEvent.Message;
            SetColorScheme(messageEvent.ColorScheme);
            StartCoroutine(ShowToast(messageEvent.Duration, messageEvent.AnimationDuration));
        }


        public static ToastColorScheme GetColorScheme(ToastType type)
        {
            switch (type)
            {
                case ToastType.Error:
                    return ErrorScheme;
                case ToastType.Warning:
                    return WarningScheme;
                default:
                    return MessageScheme;
            }
        }

        private void SetColorScheme(ToastColorScheme colorScheme)
        {
            _backgroundImage.color = colorScheme.BackgroundColor;
            _toastText.color = colorScheme.TextColor;
        }

        private IEnumerator ShowToast(float duration, float animationDuration)
        {
            var rectTransform = GetComponent<RectTransform>();
            var originalPosition = rectTransform.anchoredPosition;
            var destinationPosition = new Vector2(originalPosition.x, _destinationY);

            // start with opacity at 0
            _canvasGroup.alpha = 0f;

            float t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / animationDuration;

                rectTransform.anchoredPosition = Vector2.Lerp(originalPosition, destinationPosition, t);
                _canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }

            yield return new WaitForSeconds(duration);

            var extendedPosition = rectTransform.anchoredPosition;

            t = 0f;
            while (t < 1)
            {
                t += Time.deltaTime / animationDuration;
                rectTransform.anchoredPosition = Vector2.Lerp(extendedPosition, originalPosition, t);
                _canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
            Destroy(gameObject);
        }
    }

}
