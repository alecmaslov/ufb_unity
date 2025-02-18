using Colyseus.Schema;
using System.Collections.Generic;
using UFB.Map;
using UFB.StateSchema;

namespace UFB.Network.RoomMessageTypes
{
    public interface ISendMessage { }

    public class UfbRoomRules : ISendMessage
    {
        public int maxPlayers = 4;
        public int initHealth = 100;
        public int initEnergy = 100;
        public float turnTime = 60f * 3f;
    }

    public class RequestCharacterId : ISendMessage 
    { 
        public string characterId;
    }

    public class UfbRoomCreateOptions : ISendMessage
    {
        public string mapName = "kaiju";
        public UfbRoomRules rules = new UfbRoomRules();
    }

    public class UfbRoomJoinOptions : ISendMessage
    {
        public string token;
        public string playerId;
        public string displayName;
        public string characterId;
        public string characterClass;
    }

    public class RequestMoveMessage : ISendMessage
    {
        public Coordinates destination;
        public bool isPath = false;
        public string tileId;
        public string characterId;
        public float originEnergy;
        public string left;
        public string right;
        public string top;
        public string down;
        public bool isFeather;
    }

    public class RequestCancelMoveMessage : ISendMessage
    {
        public Coordinates destination;
        public string tileId;
        public string characterId;
        public float originEnergy;
        public string left;
        public string right;
        public string top;
        public string down;
        public List<Item> items;
    }

    public class RequestEndTurnMessage : RequestCharacterId
    {
        
    }

    public class RequestTestMessage : RequestCharacterId 
    { 
        
    }

    public class RequestSpawnMessage : ISendMessage
    {
        public Coordinates destination;
        public string tileId;
        public string playerId;
        public bool isItemBag;
    }

    public class RequestGetPowerMoveList : RequestCharacterId
    {
        public int powerId;    
    }

    public class RequestGetHighlightRect : RequestCharacterId 
    {
        public int powerMoveId;
    }

    public class RequestGetMerchantList: ISendMessage
    {

    }

    public class RequestTile: RequestCharacterId
    {
        public string tileId;
    }

    public class RequestActiveQuest: RequestCharacterId
    {
        public Quest quest;
    }

    public class RequestAddCraftItem: RequestCharacterId
    {
        public int idx1;
        public int idx2;
        public int idx3;
        public int coin;
        public string type;
    }

    public class RequestBuyItem: RequestCharacterId
    {
        public string type;
        public int id;
    }

    public class RequestSellItem : RequestCharacterId 
    {
        public string type;
        public int id;
    }

    public class RequestSetPowerMoveItem : RequestCharacterId
    {
        public string enemyId;
        public int powerMoveId;
        public int diceCount;
        public int vampireCount;
        public int extraItemId = -1;
    }

    public class RequestStackOnStartMessage : RequestCharacterId {
        public int stackId;
        public DiceData[] diceData;
    }

    public class RequestEndPowerMoveItem : RequestSetPowerMoveItem 
    {
        public int enemyDiceCount;
        public int stackId;
    }

    public class RequestSetDiceRoll : RequestCharacterId
    {
        public int powerMoveId;
        public int diceTimes = 0;
        public int extraItemId = -1;
    }

    public class RequestSetDiceStackTurnRoll : RequestCharacterId {
        public int diceType;
        public int stackId;
    }

    public class RequestMoveItem : RequestCharacterId 
    {
        public int itemId;
        public string tileId;
        public bool isFeather;
    }

    public class RequestGetSpawnMessage : RequestCharacterId
    {
        public int itemId;
        public string playerId;
        public int powerId;
        public int coinCount;
        public string spawnId;
    }

    public class RequestGetResourceMessage : ISendMessage
    {
        public string playerId;
    }
}
