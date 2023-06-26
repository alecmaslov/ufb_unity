using UnityEngine;
using System.Collections;
using UFB.Map;

namespace UFB.Entities {

    public class TileEntity : MonoBehaviour
    {
        public GameTile GameTile { get; private set; }
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private bool _isVisible = true;
        private Color _color;
        private TransformCoroutineManager _coroutineManager;
        private void OnEnable()
        {
            _coroutineManager = new TransformCoroutineManager(this);
            _coroutineManager.AddReferenceSnapshot("initial");
        }

        public void Initialize(GameTile tile, Texture texture, Color color)
        {
            GameTile = tile;
            this.name = tile.Id;
            transform.position = new Vector3(tile.Coordinates.X, 0, tile.Coordinates.Y);
            _spriteRenderer.color = color;
            _color = color;
            if (texture != null)
                _spriteRenderer.sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            
            SetVisibility(false, 0, 0);
            SetVisibility(true, 5f, Random.Range(0f, 6f));
        }

        public void SetVisibility(bool isVisible, float duration = 0.5f, float delay = 0)
        {
            if (duration == 0)
            {
                _isVisible = isVisible;
                _spriteRenderer.color = new Color(_color.r, _color.g, _color.b, isVisible ? 1 : 0);
                return;
            }

            _coroutineManager.TimedAction(
                id: "fade",
                onUpdate: (float value) =>
                {
                    float alpha = isVisible ? value : 1 - value;
                    _spriteRenderer.color = new Color(_color.r, _color.g, _color.b, alpha);
                },
                onComplete: () =>
                {
                    _isVisible = isVisible;
                    _spriteRenderer.color = new Color(_color.r, _color.g, _color.b, isVisible ? 1 : 0);
                },
                duration: duration,
                delay: delay);
        }

        public void Stretch(float heightScalar, float duration = 0.5f)
        {
            var initScale = _coroutineManager.GetReferenceScale("initial");
            initScale.y = initScale.y * (1 + heightScalar);
            _coroutineManager.ScaleTo(initScale, duration);
        }

        public void OnTraverse()
        {
            // when the player traverses over the tile
        }

        public void OnLand()
        {
            // when the player lands on the tile
        }

        
    }

}