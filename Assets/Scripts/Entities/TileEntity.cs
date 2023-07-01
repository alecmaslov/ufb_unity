using UnityEngine;
using System.Collections;
using UFB.Map;
using System.Collections.Generic;
using System.Linq;
namespace UFB.Entities {

    public class TileEntity : MonoBehaviour
    {
        public GameTile GameTile { get; private set; }
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private bool _isVisible = true;
        private Color _color;
        private TransformCoroutineManager _tileTransformManager;
        public Coordinates Coordinates => GameTile.Coordinates;
        private Wall[] _walls;

        [SerializeField] private Transform _tileTransform;
        [SerializeField] private Transform _tileMesh;

        private MeshRenderer _meshRenderer;


        private void OnEnable()
        {
            _tileTransformManager = new TransformCoroutineManager(this, useTransform: _tileTransform);
            _tileTransformManager.AddReferenceSnapshot("initial");
            _meshRenderer = _tileMesh.GetComponent<MeshRenderer>();
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
            
            _spriteRenderer.enabled = false;
            _meshRenderer.material.SetTexture("_BaseMap", texture);


            SetVisibility(false, 0, 0);
            SetVisibility(true, 2f, Random.Range(0f, 1f));
        }

        public void SetVisibility(bool isVisible, float duration = 0.5f, float delay = 0)
        {
            if (duration == 0)
            {
                _isVisible = isVisible;
                _spriteRenderer.color = new Color(_color.r, _color.g, _color.b, isVisible ? 1 : 0);
                return;
            }

            _tileTransformManager.TimedAction(
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

        public void ResetAppearance()
        {
            _spriteRenderer.color = _color;
            _tileTransformManager.TransformToReferenceSnapshot("initial", 0.5f);
        }

        public void Stretch(float heightScalar, float duration = 0.5f)
        {
            var initScale = _tileTransformManager.GetReferenceScale("initial");
            initScale.y = initScale.y * (1 + heightScalar);
            _tileTransformManager.ScaleTo(initScale, duration);
        }

        public void OnTraverse()
        {
            // when the player traverses over the tile
        }

        public void OnLand()
        {
            // when the player lands on the tile
        }

        public float DistanceTo(TileEntity other) 
        {
            return this.Coordinates.DistanceTo(other.Coordinates);
            // return Vector2.Distance(this.Coordinates.Vector, other.Coordinates.Vector);
        }

        public bool HasWall(TileSide side)
        {
            return GameTile.Edges.Any(edge => edge.Side == side && edge.EdgeProperty == EdgeProperty.Wall);
        }

        public bool HasBridge(TileSide side)
        {
            return GameTile.Edges.Any(edge => edge.Side == side && edge.EdgeProperty == EdgeProperty.Bridge);
        }

        /// <summary>
        /// Is the target adjacent tile blocked by a wall?
        /// </summary>
        public bool BlockedByWall(TileEntity target)
        {
            if (!target.Coordinates.IsAdjacent(this.Coordinates)) {
                throw new System.Exception("Trying to check a non-adjacent tile for a wall.");
            }

            if (target.Coordinates.X > this.Coordinates.X && target.Coordinates.Y > this.Coordinates.Y) {
                return this.HasWall(TileSide.Top) || this.HasWall(TileSide.Right) ||
                    target.HasWall(TileSide.Bottom) || target.HasWall(TileSide.Left);
            } else if (target.Coordinates.X < this.Coordinates.X && target.Coordinates.Y < this.Coordinates.Y) {
                return this.HasWall(TileSide.Bottom) || this.HasWall(TileSide.Left) ||
                    target.HasWall(TileSide.Top) || target.HasWall(TileSide.Right);
            } else if (target.Coordinates.X > this.Coordinates.X && target.Coordinates.Y < this.Coordinates.Y) {
                return this.HasWall(TileSide.Bottom) || this.HasWall(TileSide.Right) ||
                    target.HasWall(TileSide.Top) || target.HasWall(TileSide.Left);
            } else if (target.Coordinates.X < this.Coordinates.X && target.Coordinates.Y > this.Coordinates.Y) {
                return this.HasWall(TileSide.Top) || this.HasWall(TileSide.Left) ||
                    target.HasWall(TileSide.Bottom) || target.HasWall(TileSide.Right);
            } else if (target.Coordinates.X > this.Coordinates.X) {
                return this.HasWall(TileSide.Right) || target.HasWall(TileSide.Left);
            } else if (target.Coordinates.X < this.Coordinates.X) {
                return this.HasWall(TileSide.Left) || target.HasWall(TileSide.Right);
            } else if (target.Coordinates.Y > this.Coordinates.Y) {
                return this.HasWall(TileSide.Top) || target.HasWall(TileSide.Bottom);
            } else if (target.Coordinates.Y < this.Coordinates.Y) {
                return this.HasWall(TileSide.Bottom) || target.HasWall(TileSide.Top);
            }

            return false;
        }

    }

}



// if (target.Coordinates.X > this.Coordinates.X) {
//     return this.HasWall(TileSide.Right) || target.HasWall(TileSide.Left);
// } else if (target.Coordinates.X < this.Coordinates.X) {
//     return this.HasWall(TileSide.Left) || target.HasWall(TileSide.Right);
// } else if (target.Coordinates.Y > this.Coordinates.Y) {
//     return this.HasWall(TileSide.Top) || target.HasWall(TileSide.Bottom);
// } else if (target.Coordinates.Y < this.Coordinates.Y) {
//     return this.HasWall(TileSide.Bottom) || target.HasWall(TileSide.Top);
// } else {
//     return false;
// }