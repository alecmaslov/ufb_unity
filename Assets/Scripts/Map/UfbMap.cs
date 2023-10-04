using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UFB.Map
{
    [CreateAssetMenu(fileName = "New Map", menuName = "UFB/Map")]
    public class UfbMap : ScriptableObject
    {
        public string id;
        public string mapName;
        public Sprite mapThumbnail;
        public AssetReference mapImage;
    }
}
