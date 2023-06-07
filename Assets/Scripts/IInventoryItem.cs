using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum ItemType {
    EnergyShard,
    HeartPiece,
    EnergyCrystal,
    HeartCrystal,
    Chest,
    Gold
}

public interface IInventoryItem {
    string Name { get; }
    ItemType Type { get; }
    Sprite Image { get; }
    int Count { get; set; }
    void OnPickup();
    void OnUse();
    void OnEquip();
}