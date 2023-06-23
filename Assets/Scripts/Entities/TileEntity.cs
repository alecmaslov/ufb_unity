using UnityEngine;
using System.Collections;
using UFB.Map;

namespace UFB.Entities {


    public class TileEntity : MonoBehaviour
    {
        public GameTile GameTile { get; private set; }

        [SerializeField] private SpriteRenderer _spriteRenderer;

        private bool _isVisible = true;

        private Vector3 _originalScale;
        private Vector3 _originalPosition;

        private Color _color;

        private void Start()
        {
            _spriteRenderer = this.GetComponent<SpriteRenderer>();
            SetVisibility(false);

            _originalScale = this.transform.localScale;
            _originalPosition = this.transform.position;
        }


        // private void 

        public void Initialize(GameTile tile, Texture texture, Color color)
        {
            GameTile = tile;
            this.name = tile.Id;
            this.transform.position = new Vector3(tile.Coordinates.X, 0, tile.Coordinates.Y);
            // _spriteRenderer.color = color;
            _spriteRenderer.color = new Color(1,1,1);
            _color = color;

            transform.rotation = Quaternion.Euler(0, 270, 0);

            if (texture != null)
                _spriteRenderer.sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        private void SetVisibility(bool isVisible)
        {
            gameObject.GetComponent<Renderer>().enabled = isVisible;
            // _spriteRenderer.enabled = isVisible;
            _isVisible = isVisible;
        }

        public void TransitionIn(float delay, float duration)
        {
            StartCoroutine(TransitionInCoroutine(delay, duration));
        }

        private IEnumerator TransitionInCoroutine(float delay, float duration)
        {
            yield return new WaitForSeconds(delay);
            float time = 0;
            SetVisibility(true);
            Color currentColor = _spriteRenderer.color;
            // _spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                this.transform.localScale = Vector3.Lerp(Vector3.zero, _originalScale, t);
                // _spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, t);
                yield return null;
            }
        }

        
    }

}