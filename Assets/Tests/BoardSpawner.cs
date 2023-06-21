using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFB.Entities;
using System.Linq;

namespace UFB.Development {
    
    public class BoardSpawner : MonoBehaviour
    {

        [Header("Entities")]
        [SerializeField] private GameObject _tilePrefab;

        [Header("Dimensions")]
        [SerializeField] private int _tilesX = 26;
        [SerializeField] private int _tilesY = 26;

        [SerializeField] private string _mapName = "Kraken";

        private List<TileEntity> _tiles;

        private readonly string _tileColumns = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        void Start()
        {
            Texture[] loadedSprites = Resources.LoadAll<Texture>($"Maps/{_mapName}");

            string[] tileColumnNames = _tileColumns.Select(c => c.ToString()).ToArray();
            
            foreach (Texture sprite in loadedSprites)
            {
                Debug.Log(sprite.name);
            }

            for (int y = 0; y < _tilesY; y++)
            {
                for (int x = 0; x < _tilesX; x++)
                {
                    GameObject tile = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                    tile.transform.parent = this.transform;
                    // tile.name = $"Tile ({x}, {y})";

                    TileEntity tileEntity = tile.GetComponent<TileEntity>();

                    tileEntity.Initialize(new Map.Tile() {
                        Coordinate = new Map.Coordinate() {
                            x = x,
                            y = y
                        }
                    }, loadedSprites[Random.Range(0, loadedSprites.Length)]);

                    tileEntity.TransitionIn(x*y*0.01f, 0.5f);
                }
            }
        }
    }

}

