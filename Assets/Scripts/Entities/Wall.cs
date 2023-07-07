using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFB.Map;


namespace UFB.Entities {
    
    public class Wall : MonoBehaviour
    {
        public TileSide Side { get; private set; }
        private TransformCoroutineManager _transformManager;
        private float _transitionDuration = 1.5f;

        private Vector3 _deactivatedScale;
        private Vector3 _activatedScale;

        void Awake()
        {
            Side = ParseSide();
            // _transformManager = new TransformCoroutineManager(this, useTransform: transform);
            // _transformManager.AddReferenceSnapshot("activated");

            _activatedScale = transform.localScale;
            _deactivatedScale = transform.localScale;
            _deactivatedScale.y = 0;

            transform.localScale = _deactivatedScale;
            
            // var newSnapshot = new TransformSnapshot(transform.position, transform.rotation, newScale);
            // _transformManager.AddReferenceSnapshot("deactivated", newSnapshot);
            // _transformManager.ScaleTo(newScale, 0);
            // _transformManager.TransformToReferenceSnapshot("deactivated", 0);
        }

        private void OnEnable()
        {
            _transformManager = new TransformCoroutineManager(this);
        }

        public void Activate()
        {
            // transform.localScale = _activatedScale;
            _transformManager.ScaleTo(_activatedScale, _transitionDuration);
            // _transformManager.ScaleTo(new Vector3(1, 1, 1), _transitionDuration);
            // _transformManager.TransformToReferenceSnapshot("activated", _transitionDuration);
        }

        public void Deactivate()
        {
            _transformManager.ScaleTo(_deactivatedScale, _transitionDuration);
            // transform.localScale = _deactivatedScale;
            // _transformManager.ScaleTo(new Vector3(1, 0, 1), _transitionDuration);
            // _transformManager.TransformToReferenceSnapshot("deactivated", _transitionDuration);
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

        // Update is called once per frame
        void Update()
        {
            
        }

}
}
