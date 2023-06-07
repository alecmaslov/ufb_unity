using UnityEngine;

namespace UFB.Items {
    public enum ItemType {
        MagicToken,
        EnergyShard,
        HeartPiece,
        EnergyCrystal,
        HeartCrystal,
        Chest,
        Gold
    }

    public interface IInventoryItem {
        string DisplayName { get; }
        ItemType Type { get; }
        Sprite Image { get; }
        void OnPickup();
        void OnUse();
        void OnEquip();
    }
}