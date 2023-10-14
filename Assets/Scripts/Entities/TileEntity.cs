using UnityEngine;
using System.Collections;
using UFB.Map;
using System.Collections.Generic;
using System.Linq;
using UFB.Core;
using TMPro;
using System;
using UFB.Events;
using UFB.StateSchema;


namespace UFB.Entities
{

    interface ITileAttachable
    {
        void AttachToTile(TileEntity tile);
        void DetachFromDile();
        void UpdateAttachedPosition(Vector3 newPosition);
    }

    public class TileEntity : MonoBehaviour, IClickable
    {
        public TileState State { get; private set; }

        public GameTile GameTile { get; private set; }
        public bool IsStretched { get; private set; }
        [SerializeField] private SpriteRenderer _spriteRenderer;
        private bool _isVisible = true;
        private Color _color;
        public Coordinates Coordinates => State.coordinates.ToCoordinates();

        public Transform wallContainer;
        private Dictionary<TileSide, Wall> _walls;

        [SerializeField] private Transform _tileTransform;
        [SerializeField] private Transform _tileMesh;
        [SerializeField] private TextMeshProUGUI _coordinatesText;

        private MeshRenderer _meshRenderer;

        private List<TileAttachable> _attachables = new List<TileAttachable>();
        private ScaleAnimator _scaleAnimator;

        private CoroutineManager _coroutineManager;
        public CoroutineManager CoroutineManager
        {
            get
            {
                if (_coroutineManager == null)
                {
                    _coroutineManager = new CoroutineManager(this);
                }
                return _coroutineManager;
            }
        }

        // position on top of the tile
        public Vector3 AttachedPosition
        {
            get
            {
                var position = _tileTransform.position;
                position.y += _tileTransform.localScale.y;
                return position;
            }
        }

        private void Awake()
        {
            _scaleAnimator = _tileTransform.GetComponent<ScaleAnimator>();
            _scaleAnimator.AddUpdateListener(OnScaleChanged);
            _scaleAnimator.SetSnapshot("initial");
            _meshRenderer = _tileMesh.GetComponent<MeshRenderer>();

        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {
            _scaleAnimator.RemoveUpdateListener(OnScaleChanged);
        }

        public void Initialize(TileState state, Vector3 position, string assetPath)
        {
            State = state;
            name = state.id;
            _coordinatesText.text = $"{Coordinates.GameId.Replace("tile_", "").Replace("_", " ")}";
            transform.position = position;
        }

        public void Initialize(GameTile tile, GameBoard board)
        {
            // GameTile = tile;
            // this.name = tile.Coordinates.GameId;
            // _scaleAnimator = _tileTransform.GetComponent<ScaleAnimator>();
            // _scaleAnimator.AddUpdateListener(OnScaleChanged);
            // _scaleAnimator.SetSnapshot("initial");
            // _meshRenderer = _tileMesh.GetComponent<MeshRenderer>();


            // _coordinatesText.text = $"{tile.Coordinates.GameId.Replace("tile_", "").Replace("_", " ")}";

            // transform.position = new Vector3(board.Dimensions - tile.Coordinates.Y, 0f, tile.Coordinates.X);
            // transform.position = new Vector3(board.Dimensions - tile.Coordinates.X, 0f, tile.Coordinates.Y);

            var color = tile.GetColor();
            _spriteRenderer.color = color;
            _color = color;


            // var texture = tile.GetTexture(board.State.name);
            
            
            // if (texture != null)
            //     _spriteRenderer.sprite = Sprite.Create((Texture2D)texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // _spriteRenderer.enabled = false;
            // _meshRenderer.material.SetTexture("_BaseMap", texture);

            InitializeWalls();
        }

        private void InitializeWalls()
        {
            _walls = new Dictionary<TileSide, Wall>();
            var allWalls = GetComponentsInChildren<Wall>();
            foreach (var wall in GetComponentsInChildren<Wall>())
            {
                _walls.Add(wall.Side, wall);
                if (Application.isEditor) wall.Initialize();
            }
            GameTile.Edges.ForEach(edge =>
            {
                var wall = _walls[edge.Side];
                CoroutineHelpers.DelayedAction(() => wall.Activate(), UnityEngine.Random.Range(0.1f, 3f), this);
            });
        }


        private void OnScaleChanged(Vector3 newScale)
        {
            if (_attachables.Count == 0) return;
            bool needsCleanup = false;
            foreach (var attachable in _attachables)
            {
                if (attachable == null)
                {
                    needsCleanup = true; // bad - this means we forgot to DetachEntity()
                    Debug.LogError($"[TileEntity.OnScaleChanged] Attached entity is null");
                    continue;
                }
                attachable.OnTilePositionUpdated(AttachedPosition);
            }
            if (needsCleanup) _attachables.RemoveAll(e => e == null);
        }


        public void OnClick()
        {
            Debug.Log($"[TileEntity.OnClick] {name}");
            // EventBus.Publish(new RequestPlayerMoveEvent(Coordinates));
            // EventBus.Publish(new TileClickedEvent(this));
            ChangeColor(Color.red, 0.5f);
        }

        public void SetVisibility(bool isVisible, float duration = 0.5f, float delay = 0)
        {
            Debug.LogError($"[TileEntity.SetVisibility] Not implemented yet");
            if (duration == 0)
            {
                _isVisible = isVisible;
                _spriteRenderer.color = new Color(_color.r, _color.g, _color.b, isVisible ? 1 : 0);
                return;
            }

            // _tileTransformManager.TimedAction(
            //     id: "fade",
            //     onUpdate: (float value) =>
            //     {
            //         float alpha = isVisible ? value : 1 - value;
            //         _spriteRenderer.color = new Color(_color.r, _color.g, _color.b, alpha);
            //     },
            //     onComplete: () =>
            //     {
            //         _isVisible = isVisible;
            //         _spriteRenderer.color = new Color(_color.r, _color.g, _color.b, isVisible ? 1 : 0);
            //     },
            //     duration: duration,
            //     delay: delay);
        }

        public void ToggleCoordinateText()
        {
            _coordinatesText.gameObject.SetActive(!_coordinatesText.gameObject.activeSelf);
        }

        public void SetWallHeight(float height)
        {
            foreach (var wall in _walls.Values)
                wall.SetHeight(height);
        }


        public void ResetAppearance()
        {
            _spriteRenderer.color = _color;
        }

        public void Glow(float amount, float duration = 0.5f)
        {
            CoroutineManager.TimedAction(
                id: "glow",
                onUpdate: (float value) =>
                {
                    var currentColor = _meshRenderer.material.GetColor("_EmissionColor");
                    var newColor = Color.Lerp(currentColor, Color.white * amount, value);
                    _meshRenderer.material.SetColor("_EmissionColor", newColor);
                },
                onComplete: () =>
                {
                    _meshRenderer.material.SetColor("_EmissionColor", Color.white * amount);
                },
                duration: duration
            );
        }

        public void ChangeColor(Color color, float duration = 0.5f)
        {
            CoroutineManager.TimedAction(
                id: "color",
                onUpdate: (float value) =>
                {
                    var currentColor = _meshRenderer.material.GetColor("_BaseColor");
                    var newColor = Color.Lerp(currentColor, color, value);
                    _meshRenderer.material.SetColor("_BaseColor", newColor);
                },
                onComplete: () =>
                {
                    _meshRenderer.material.SetColor("_BaseColor", color);
                },
                duration: duration
            );
        }

        public void Stretch(float heightScalar, float duration = 0.5f, Action onComplete = null)
        {
            var newScale = Vector3.Scale(_scaleAnimator.GetSnapshot("initial"), new Vector3(1, 1 + heightScalar, 1));
            _scaleAnimator.AnimateTo(newScale, duration, onComplete);
            if (heightScalar > 0.01)
            {
                IsStretched = true;
            }
            else
            {
                IsStretched = false;
            }
        }

        public void ResetStretch(float duration = 0.5f, Action onComplete = null)
        {
            _scaleAnimator.AnimateTo(_scaleAnimator.GetSnapshot("initial"), duration, onComplete);
            IsStretched = false;
        }

        public void SlamDown()
        {
            _scaleAnimator.AnimateTo(_scaleAnimator.GetSnapshot("initial"), 0.2f);
            // GameManager.Instance.GameBoard.RunRippleEffect(this);
            IsStretched = false;
        }

        // public void RippleEffect() 
        // {
        //     GameController.Instance.GameBoard.RunRippleEffect(this);
        // }


        /// <summary>
        /// Attach a gameObject to this tile, and it will move with it
        /// CHANGE THIS to using ITilePlaceable
        /// </summary>
        public void AttachEntity(TileAttachable attachable)
        {
            attachable.transform.position = AttachedPosition;
            _attachables.Add(attachable);
        }

        public void DetachEntity(TileAttachable attachable) => _attachables.Remove(attachable);

        public void OnPassThrough(PlayerEntity player)
        {
            // when the player traverses over the tile
        }

        public void OnEnter(PlayerEntity player)
        {
            // when the player lands on the tile
        }

        public float DistanceTo(TileEntity other) => this.Coordinates.DistanceTo(other.Coordinates);

        public bool HasWall(TileSide side) => GameTile.Edges.Any(edge => edge.Side == side && edge.EdgeProperty == EdgeProperty.Wall);

        public bool HasBridge(TileSide side) => GameTile.Edges.Any(edge => edge.Side == side && edge.EdgeProperty == EdgeProperty.Bridge);

        /// <summary>
        /// Is the target adjacent tile blocked by a wall?
        /// </summary>
        public bool BlockedByWall(TileEntity target)
        {
            if (!target.Coordinates.IsAdjacent(this.Coordinates))
            {
                throw new System.Exception("Trying to check a non-adjacent tile for a wall.");
            }

            if (target.Coordinates.X > this.Coordinates.X)
            {
                return this.HasWall(TileSide.Right) || target.HasWall(TileSide.Left);
            }
            else if (target.Coordinates.X < this.Coordinates.X)
            {
                return this.HasWall(TileSide.Left) || target.HasWall(TileSide.Right);
            }
            else if (target.Coordinates.Y > this.Coordinates.Y)
            {
                return this.HasWall(TileSide.Top) || target.HasWall(TileSide.Bottom);
            }
            else if (target.Coordinates.Y < this.Coordinates.Y)
            {
                return this.HasWall(TileSide.Bottom) || target.HasWall(TileSide.Top);
            }
            return false;
        }

        public override string ToString() => $"TileEntity({this.name}) at {this.Coordinates}";
    }

}