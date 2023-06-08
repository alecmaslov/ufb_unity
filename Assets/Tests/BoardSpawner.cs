using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UFB.Entities;

namespace UFB.Development {
    
    public class BoardSpawner : MonoBehaviour
    {

        [Header("Entities")]
        [SerializeField] private GameObject _tilePrefab;

        [Header("Dimensions")]

        [SerializeField] private int _tilesX = 26;
        [SerializeField] private int _tilesY = 26;

        private List<TileEntity> _tiles;

        // Start is called before the first frame update
        void Start()
        {
            Texture[] loadedSprites = Resources.LoadAll<Texture>("Tiles/");
            
            foreach (Texture sprite in loadedSprites)
            {
                Debug.Log(sprite.name);
            }

            for (int x = 0; x < _tilesX; x++)
            {
                for (int y = 0; y < _tilesY; y++)
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

        // Update is called once per frame
        void Update()
        {
            
        }
    }

}

