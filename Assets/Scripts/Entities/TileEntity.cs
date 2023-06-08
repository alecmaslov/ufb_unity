using UnityEngine;
using System.Collections;
using UFB.Map;

namespace UFB.Entities {


    public class TileEntity : MonoBehaviour
    {
        public Tile GameTile { get; private set; }

        [SerializeField] private SpriteRenderer _spriteRenderer;

        private bool _isVisible = true;

        private void Start()
        {
            _spriteRenderer = this.GetComponent<SpriteRenderer>();
            SetVisibility(false);
        }

        public void Initialize(Tile tile, Texture texture)
        {
            GameTile = tile;
            this.name = tile.Id;
            this.transform.position = new Vector3(tile.Coordinate.x, 0, tile.Coordinate.y);
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
            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;
                this.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return null;
            }
        }

        
    }

}