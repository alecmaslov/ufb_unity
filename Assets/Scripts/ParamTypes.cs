using System;

[Serializable]
public class UserData
{
    public string id;
    public string email;
    public string displayName;
    public int gold;
    
    public int collectedGold;
    public int losses;
    public int wins;
    public int kills;
    public int battles;
    public int damageTaken;
    public int damageHeal;
    public int itemBags;
    public int usedEnergies;
    public int damageDeal;
    public int usedStacks;
    public int collectGolds;
    public int traveledTiles;
    public int chests;
}

[Serializable]
public struct HeroData
{
    public string error;
    public int level;
    public int collect_golds;
    public int losses;
    public int wins;
    public int kills;
    public int battles;
    public int damage_taken;
    public int damage_heal;
    public int item_bags;
    public int used_energies;
    public int damage_deal;
    public int used_stacks;
    public int traveled_tiles;
    public int chests;
}