using System;
using System.Collections.Generic;
using UFB.Network.RoomMessageTypes;

[Serializable]
public class UserData
{
    public string sessionId;
    public string id;
    public string email;
    public string displayName;
    public int gold;
    
    public UfbRoomJoinOptions  joinOptions;
    public UfbRoomCreateOptions createOptions;
    
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

[Serializable]
public class UIRoomData
{
    public string id;
    public string name;
    public List<RoomUserData> members = new ();
    public string ownerId;
    public bool isPrivate;
    public string inviteToken;
}

[Serializable]
public struct RoomUserData
{
    public string id;
    public string name;
    public string characterClass;
}