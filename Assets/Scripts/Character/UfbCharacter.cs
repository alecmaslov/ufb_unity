using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace UFB.Player
{
    // eventually this will be loaded from a network request

    [CreateAssetMenu(fileName = "New Character", menuName = "UFB/Character")]
    public class UfbCharacter : ScriptableObject
    {
        public string id;
        public string characterName;
        public Sprite avatar;
        public Sprite card;
        public string description;
        public Mesh mesh;

        public AssetReference modelPrefab;
    }
}