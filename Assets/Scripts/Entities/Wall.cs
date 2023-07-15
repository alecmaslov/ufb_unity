using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFB.Map;
using UFB.Core;

namespace UFB.Entities {
    
    public class Wall : MonoBehaviour
    {
        [SerializeField] TileSide _side;
        public TileSide Side { get => _side; }
        // private TransformCoroutineManager _transformManager;
        private ScaleAnimator _scaleAnimator;
        private float _transitionDuration = 1.5f;
        private float _wallHeight = 0.5f;

        private bool _isActivated = false;

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            Vector3 deactivatedScale;
            Vector3 activatedScale;

            activatedScale = transform.localScale;
            activatedScale.y = _wallHeight;

            deactivatedScale = transform.localScale;
            deactivatedScale.y = 0;
            transform.localScale = deactivatedScale;

            _scaleAnimator = GetComponent<ScaleAnimator>();
            _scaleAnimator.SetSnapshot(deactivatedScale, "deactivated");
            _scaleAnimator.SetSnapshot(activatedScale, "activated");

        }

        public void Activate()
        {

            _scaleAnimator.AnimateToSnapshot("activated", _transitionDuration);
            _isActivated = true;
        }

        public void Deactivate()
        {
            _scaleAnimator.AnimateToSnapshot("deactivated", _transitionDuration);
            _isActivated = false;
        }

        public void SetHeight(float height)
        {
            Vector3 activatedHeight = _scaleAnimator.GetSnapshot("activated");
            activatedHeight.y = height;
            _scaleAnimator.SetSnapshot(activatedHeight, "activated");
            if (_isActivated) {
                _scaleAnimator.AnimateToSnapshot("activated", _transitionDuration);
            }
        }


}
}
