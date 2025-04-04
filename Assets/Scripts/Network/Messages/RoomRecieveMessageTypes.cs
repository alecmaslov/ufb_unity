using Colyseus.Schema;
using UFB.Items;
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

    public class SlotItemData
    {
        public Item power;
        public PowerMove[] powermoves;
    }
    
    public class DiceData
    {
        public DICE_TYPE type;
        public int diceCount;
    }

    public class PowerMoveResult : IReceiveMessage 
    {
        public ResultItem[] items;
        public ResultItem[] stacks;
        public int dice;
        public int ultimate;
        public int energy;
        public int health;
        public int coin;
        public int perkId = -1;
        public int perkId1 = -1;
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

        public int extraItemId = -1;

        public int getDiceTime()
        {
            int count = 0;
            if (result.dice > 0) 
            {
                count++;
            }
            if(result.perkId == (int)PERK.VAMPIRE)
            {
                count++;
            }
            if(result.perkId1 == (int)PERK.VAMPIRE)
            {
                count++;
            }
            return count;
        }

        public bool IsDiceAttacked()
        {

            return range > 0;
        }
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

    public class SetCharacterPositionMessage : IReceiveMessage 
    { 
        public string characterId;
        public PathStep[] path;
    }

    public class SetMovePointMessage : IReceiveMessage 
    {
        public string characterId;
        public PathStep[] path;
        public int cost;
        public int featherCount;
        public string portalNextTileId;
    }

    public class GetEquipSlotMessage : IReceiveMessage
    {
        public SlotItemData[] data;
    }
    
    public class GetStackOnStartMessage : IReceiveMessage {
        public string characterId;
        public ResultItem[] stackList;
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

    public class ToastBanStackMessage : IReceiveMessage
    {
        public string characterId;
        public int stack1;
        public int stack2;
        public int count1;
        public int count2;
    }

    public class DefenceAttackMessage : IReceiveMessage 
    {
        public string originId;
        public string targetId;
        public string type;
        public PowerMove pm;
    }

    public class EndAttackMessage : IReceiveMessage 
    {
        public string characterId;
    }
    public class DeadMonsterMessage : IReceiveMessage 
    {
        public string characterId;
    }

    public class GameEndMessage : IReceiveMessage 
    {
        public string characterId;
        public int endType;
    }

    public class ToastStackPerkMessage : IReceiveMessage
    {
        public string characterId;
        public int stackId;
        public int perkId;
        public int count;
    }

    public class ToastPerkMessage : IReceiveMessage
    {
        public string characterId;
        public int perkId;
        public string tileId;
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

    public class SetHighLightRectMessage : IReceiveMessage {
        public string[] tileIds;
    }

    public class SetDiceRollMessage : IReceiveMessage {
        public DiceData[] diceData;
    }

    public class EnemyDiceRollMessage : IReceiveMessage {
        public string enemyId;
        public string characterId;
        public int powerMoveId;
        public int stackId;
        public int diceCount;
        public int enemyDiceCount;
    }

    public class EquipBonus : IReceiveMessage {
        public int id;
        public ResultItem[] items;
        public ResultItem[] stacks;
        public ResultItem[] randomItems;
    }

    public class RewardBonusMessage : IReceiveMessage
    {
        public string characterId;
        public int coin = 0;

        public ResultItem[] items;
        public ResultItem[] stacks;
        public ResultItem[] powers;
    }

    public class GetTurnStartEquipBonusMessage : IReceiveMessage {
        public EquipBonus[] bonuses;
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

    public class UnEquipItemMessage : IReceiveMessage
    {
        public string playerId;
        public int powerId;
    }
}