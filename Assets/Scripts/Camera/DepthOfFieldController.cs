using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UFB.Camera
{
    public class DepthOfFieldController : MonoBehaviour
    {
        public float FocalLength
        {
            get => _dof.focalLength.value;
            set { _focalLengthInterpolator.LerpTo(value, defaultDuration); }
        }

        public float Aperture
        {
            get => _dof.aperture.value;
            set { _apertureInterpolator.LerpTo(value, defaultDuration); }
        }

        public float FocusDistance
        {
            get => _dof.focusDistance.value;
            set { _focusDistanceInterpolator.LerpTo(value, defaultDuration); }
        }

        public AnimationCurve curve = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.5f, 1)
        );

        // [SerializeField]
        // private float _focalLength = 150f;

        // [SerializeField]
        // private float _aperture = 1f;

        // [SerializeField]
        // private float _focusDistance = 10f;

        public float defaultDuration = 0.5f;

        [SerializeField]
        private float _defaultFocalLength = 150f;

        [SerializeField]
        private Volume _volume;

        [SerializeField]
        private float _dofOffset = -0.5f;

        [SerializeField]
        private float _aperturePow = 0.5f;

        private Interpolator<float> _focalLengthInterpolator;
        private Interpolator<float> _apertureInterpolator;
        private Interpolator<float> _focusDistanceInterpolator;

        private DepthOfField _dof;

        private void OnEnable()
        {
            if (_volume.profile.TryGet(out DepthOfField dof))
            {
                _dof = dof;
            }
            else
            {
                throw new System.Exception("No depth of field found in volume");
            }

            _focalLengthInterpolator = new Interpolator<float>(
                this,
                Mathf.Lerp,
                (newValue) => _dof.focalLength.value = newValue,
                () => _dof.focalLength.value,
                curve
            );

            _focusDistanceInterpolator = new Interpolator<float>(
                this,
                Mathf.Lerp,
                (newValue) => _dof.focusDistance.value = newValue,
                () => _dof.focusDistance.value,
                curve
            );

            _apertureInterpolator = new Interpolator<float>(
                this,
                Mathf.Lerp,
                (newValue) => _dof.aperture.value = newValue,
                () => _dof.aperture.value,
                curve
            );

            _focalLengthInterpolator.LerpTo(_defaultFocalLength, 0.5f);

            if (_volume == null)
            {
                _volume = FindObjectOfType<Volume>();
                if (_volume == null)
                {
                    throw new System.Exception("No volume found in scene");
                }
            }
        }

        public void AutoFocus(float distance, float maxDist, float? duration)
        {
            _focusDistanceInterpolator.LerpTo(distance + _dofOffset, duration ?? defaultDuration);
            _apertureInterpolator.LerpTo(
                32 - (Mathf.Pow(distance / maxDist, _aperturePow) * 31) + 1,
                duration ?? defaultDuration
            );

            if (Mathf.Abs(_dof.focalLength.value - _defaultFocalLength) > 0.1f)
            {
                _focalLengthInterpolator.LerpTo(_defaultFocalLength, duration ?? defaultDuration);
            }
        }

        public void ManualFocus(float distance, float aperture, float? focalLength, float? duration)
        {
            _focusDistanceInterpolator.LerpTo(distance + _dofOffset, duration ?? defaultDuration);
            _apertureInterpolator.LerpTo(aperture, duration ?? defaultDuration);
        }

        public void PrintDebug()
        {
            Debug.Log(
                $"[OrbitCamera] focus distance: {_dof.focusDistance.value} | aperature: {_dof.aperture.value}"
            );
        }
    }
}
