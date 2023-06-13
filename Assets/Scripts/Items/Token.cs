using UFB.Player;
using UnityEngine;
using UFB.Items;

namespace UFB.Items {

    // here we implement an IStatusValue and IInventoryItem, since they can be viewed as both
    public class MagicToken : IInventoryItem {
        
        public string DisplayName => "Magic Token";

        public ItemType Type => ItemType.MagicToken;

        // public Sprite Image { get; }

        public void OnPickup() {}
        
        public void OnUse() {}

        public void OnEquip() {}
    }

}