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
        FlameChili,

        FlameChili2,
        FlameChili3,
        IceTea2,
        IceTea3,
        HeartPiece2,
        Potion2,
        Potion3,
        Feather2,
        Feather3,
        Arrow2,
        Arrow3,
        Bomb2,
        Bomb3,

        Melee2,
        Mana2,
        Quiver2,
        BombBag2,
        WarpCrystal2,
        Elixir2,
        BombArrow2,
        FireArrow2,
        IceArrow2,
        VoidArrow2,
        FireBomb2,
        IceBomb2,
        VoidBomb2,
        caltropBomb2,
        
        //Random Item
        RandomArrow,
        RandomBomb
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

    public enum STACK
    {
        Cure = 0,
        Block,
        Steady,
        Barrier,
        Charge,
        Revenge,
        Revive,
        Slow,
        Dodge,
        Freeze,
        Burn,
        Void,
        PUMP,

        Dodge2,
        Cure2,
        Charge2,
        Barrier2,
        Steady2,
        Revenge2,
        Block2,
        
    }

    public enum PERK
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

    public enum USER_TYPE 
    { 
        USER = 1,
        MONSTER
    }

    public enum DICE_TYPE
    {
        DICE_6 = 1,
        DICE_4,
        DICE_6_6,
        DICE_6_4
    }

    public enum END_TYPE 
    {
        VICTORY = 1,
        DEFEAT
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