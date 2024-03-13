using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UFB.Map
{
    public class TileWalls : MonoBehaviour
    {
        public readonly string wallPrefabAddress = "Prefabs/Wall";

        public Dictionary<TileSide, GameObject> Walls { get; private set; } =
            new Dictionary<TileSide, GameObject>();

        private readonly Dictionary<int, TileSide> _tileSideMapping = new Dictionary<int, TileSide>
        {
            { 0, TileSide.Top },
            { 1, TileSide.Right },
            { 2, TileSide.Bottom },
            { 3, TileSide.Left }
        };

        public async void SpawnWalls(bool[] walls)
        {
            for (int i = 0; i < walls.Length; i++)
            {
                if (walls[i])
                {
                    var wallObject = await Addressables.InstantiateAsync(wallPrefabAddress, transform).Task;
                    wallObject.transform.localPosition = Vector3.zero;
                    wallObject.name = $"Wall_{i}";
                    ApplyTransforms(wallObject, _tileSideMapping[i]);
                    Walls.Add(_tileSideMapping[i], wallObject);
                }
            }
            // foreach (var w in walls.items)
            // {
            //     int index = w.Key;
            //     if (w.Value == 1)
            //     {
            //         var wallObject = await Addressables.InstantiateAsync(wallPrefabAddress, transform).Task;
            //         // var wallMaterial = await Addressables.LoadAssetAsync<Material>("Materials/Wall").Task;

            //         // wallObject.GetComponent<MeshRenderer>().material = wallMaterial;
            //         wallObject.transform.localPosition = Vector3.zero;
            //         wallObject.name = $"Wall_{index}";
            //         ApplyTransforms(wallObject, _tileSideMapping[index]);
            //         Walls.Add(_tileSideMapping[index], wallObject);
            //     }
            // }
        }

        private void ApplyTransforms(GameObject wall, TileSide side)
        {
            switch (side)
            {
                case TileSide.Top:
                    wall.transform.localPosition += new Vector3(0, 0, 0.5f);
                    wall.transform.Rotate(0, 90, 0);
                    break;
                case TileSide.Right:
                    wall.transform.localPosition += new Vector3(0.5f, 0, 0);
                    wall.transform.Rotate(0, 0, 0);
                    break;
                case TileSide.Bottom:
                    wall.transform.localPosition += new Vector3(0, 0, -0.5f);
                    wall.transform.Rotate(0, 270, 0);
                    break;
                case TileSide.Left:
                    wall.transform.localPosition += new Vector3(-0.5f, 0, 0);
                    wall.transform.Rotate(0, 180, 0);
                    break;
            }
        }
    }
}

// 1, 3 correct
// switch (side)
// {
// case TileSide.Top:
//     wall.transform.localPosition += new Vector3(0, 0, 0.5f);
//     wall.transform.Rotate(0, 90, 0);
//     break;
// case TileSide.Right:
//     wall.transform.localPosition += new Vector3(0.5f, 0, 0);
//     wall.transform.Rotate(0, 0, 0);
//     break;
// case TileSide.Bottom:
//     wall.transform.localPosition += new Vector3(0, 0, -0.5f);
//     wall.transform.Rotate(0, 270, 0);
//     break;
// case TileSide.Left:
//     wall.transform.localPosition += new Vector3(-0.5f, 0, 0);
//     wall.transform.Rotate(0, 180, 0);
//     break;
// }