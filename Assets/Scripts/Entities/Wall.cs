using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFB.Map;
using UFB.Core;

namespace UFB.Entities {
    
    public class Wall : MonoBehaviour
    {
        public TileSide Side { get; private set; }
        // private TransformCoroutineManager _transformManager;
        private ScaleAnimator _scaleAnimator;
        private float _transitionDuration = 1.5f;
        private readonly float _wallHeight = 0.5f;

        void Awake()
        {
            Side = ParseSide();

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
        }

        public void Deactivate()
        {
            _scaleAnimator.AnimateToSnapshot("deactivated", _transitionDuration);
        }

        private TileSide ParseSide() {
            var tileSide = gameObject.name.Split("__");
            if (tileSide.Length < 2) throw new System.Exception("Wall name must be in the format of TileName__Side");
            var side = tileSide[1];

            switch(side)
            {
                case "Left":
                    Side = TileSide.Left;
                    break;
                case "Right":
                    Side = TileSide.Right;
                    break;
                case "Top":
                    Side = TileSide.Top;
                    break;
                case "Bottom":
                    Side = TileSide.Bottom;
                    break;
            }
            return Side;
        }

}
}
