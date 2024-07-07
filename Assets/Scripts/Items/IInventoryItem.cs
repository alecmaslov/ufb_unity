using System;
using UnityEngine;

namespace UFB.Items {
    [Serializable]
    public enum ItemType {
        MagicToken,
        EnergyShard,
        HeartPiece,
        EnergyCrystal,
        HeartCrystal,
        Chest,
        Gold
    }

    [Serializable]
    public enum ITEM
    {
        HeartPiece = 0,
        EnergyShard,
        Potion,
        Feather,
        Arrow,
        Bomb,

        HeartCrystal,
        EnergyCrystal,
        Melee,
        Mana,
        WarpCrystal,
        Elixir,
        Quiver,
        BombBag,
        BombArrow,
        FireArrow,
        IceArrow,
        VoidArrow,
        FireBomb,
        IceBomb,
        VoidBomb,
        caltropBomb,
        IceTea,
        FlameChili
    }

    public enum POWER
    {
        Sword1 = 0,
        Axe1,
        Spear1,
        Shield1,
        Bow1,
        Crossbow1,
        Cannon1,
        Armor1,
        Fire1,
        Ice1,
        Holy1,
        Void1,

        Sword2,
        Axe2,
        Spear2,
        Shield2,
        Bow2,
        Crossbow2,
        Cannon2,
        Armor2,
        Fire2,
        Ice2,
        Holy2,
        Void2,

        Sword3,
        Axe3,
        Spear3,
        Shield3,
        Bow3,
        Crossbow3,
        Cannon3,
        Armor3,
        Fire3,
        Ice3,
        Holy3,
        Void3,
    }

    public enum Stack
    {
        Cure = 0,
        Block,
        Steady,
        Reflect,
        Charge,
        Revenge,
        Revive,
        Slow,
        Dodge,
        Freeze,
        Burn,
        Void,
    }

    public enum Perk
    {
        PUSH,
        PULL,
        VAMPIRE,
        X_RAY,
        AREA_OF_EFFECT
    }

    public enum QUEST 
    {
        SLAYER,
        GLITTER,
        KILL,
        CRAFTS,
        LUCK,
        ENERGY,
        STRENGTH,
        LIFE
    }

    public enum PowerType {

    }

    public interface IPower {
        void OnEquip();
        void OnUnequip();
    }

    public interface IBonus {
        
    }

    public interface IInventoryItem {
        string DisplayName { get; }
        ItemType Type { get; }
        // Sprite Image { get; }
        void OnPickup();
        void OnUse();
    }
}