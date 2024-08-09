using Colyseus.Schema;
using UFB.StateSchema;

namespace UFB.Network.RoomMessageTypes
{
    public interface IReceiveMessage { }

    public class PathStep : IReceiveMessage
    {
        public string tileId;
        public CoordinatesState coord;
    }

    public class ResultItem : IReceiveMessage
    {
        public int id;
        public int count;
    }

    public class PowerMoveResult : IReceiveMessage 
    {
        public ResultItem[] items;
        public ResultItem[] stacks;
        public int ultimate;
        public int energy;
        public int health;
        public int coin;
    }

    public class PowerMove : IReceiveMessage
    {
        public int id;
        public string name;
        public int powerImageId;
        public int[] powerIds;
        public Item[] costList;
        public PowerMoveResult result;
        public int light;
        public int coin;
        public int range;
    }

    public class CharacterMovedMessage : IReceiveMessage
    {
        public string characterId;
        public PathStep[] path;
        public int left;
        public int right;
        public int top;
        public int down;
    }

    public class PowerMoveListMessage : IReceiveMessage
    {
        public PowerMove[] powermoves;
    }

    public class MoveItemMessage : IReceiveMessage
    {
        public int left;
        public int right;
        public int top;
        public int down;
        public int itemId;
    }

    public class SetMoveItemMessage : IReceiveMessage 
    { 
        public int itemId;
        public string tileId;
    }

    public class ItemResult : IReceiveMessage 
    {
        public int heart;
        public int energy;
        public int ultimate;
        public int stackId = -1;
        public int powerId = -1;
        public int perkId;
    }

    public class QuestItem: IReceiveMessage
    {
        public int questId;
        public string questTitle;
        public string questDescription;
        public int cost;
    }

    public class GetBombMessage : IReceiveMessage
    {
        public string playerId;
        public ItemResult itemResult;
    }

    public class TurnMessage : IReceiveMessage
    {
        public string characterId;
        public float curTime;
    }

    public class TurnChangeMessage : IReceiveMessage
    {
        public int turn;
        public string characterId;
        public float curTime;
    }

    public class SpawnInitMessage : IReceiveMessage
    {
        public string characterId;
        public string spawnId;
        public int coin;
        public int item;
        public int power;
        public string tileId;
    }

    public class AddExtraScoreMessage: IReceiveMessage
    {
        public string characterId;
        public string type;
        public int score;
        public int stackId = -1;
    }

    public class GetResourceDataMessage : IReceiveMessage
    {
        public CharacterState characterState;
    }

    public class GetMerchantDataMessage: IReceiveMessage
    {
        public Item[] items;
        public Item[] stacks;
        public Item[] powers;
        public Quest[] quests;
        public string tileId;
    }

    public class GetReSpawnMerchantMessage : IReceiveMessage
    {
        public string tileId;
        public string oldTileId;
    }

    public class GetQuestDataMessage: IReceiveMessage
    {
        public QuestItem[] questItems;
    }

    public class NotificationMessage : IReceiveMessage
    {
        public string type;
        public string message;
    }

    public class BecomeZombieMessage : IReceiveMessage
    {
        public string playerId; // the playerId the zombie will takeover
    }
}