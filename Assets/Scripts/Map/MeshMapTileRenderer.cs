using UnityEngine;

namespace UFB.Map
{
    public class MeshMapTileRenderer : ITileRenderer
    {
        public Coordinates Coordinates { get; private set; }
        public GameObject GameObject { get; private set; }
        public float Height { get; set; }
        private int _meshIndex { get; set; }
        private float _initialHeight;

        private MeshTileMapGenerator _meshMap;

        public MeshMapTileRenderer(
            int x,
            int y,
            float height,
            GameObject gameObject,
            MeshTileMapGenerator meshMap,
            int meshIndex
        )
        {
            Coordinates = new Coordinates(x, y);
            Height = height;
            GameObject = gameObject;
            _meshIndex = meshIndex;
            _meshMap = meshMap;
            _initialHeight = height;
        }

        public void SetHeight(float newHeight)
        {
            _meshMap.SetTileHeight(_meshIndex, newHeight, null);
            Height = newHeight;
        }

        public void SetTint(Color color)
        {
            /// TODO: implement
        }

        public void Reset()
        {
            SetHeight(_initialHeight);
            SetTint(Color.white);
        }
    }
}
