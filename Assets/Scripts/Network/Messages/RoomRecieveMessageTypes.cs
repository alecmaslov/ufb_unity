using UFB.StateSchema;

namespace UFB.Network.RoomMessageTypes
{
    public interface IReceiveMessage { }

    public class PathStep : IReceiveMessage
    {
        public string tileId;
        public CoordinatesState coord;
    }

    public class PowerMove : IReceiveMessage
    {
        public int id;
        public string name;
        public int powerImageId;
        public int[] powerIds;
        public Item[] costList;
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

    public class GetBombMessage : IReceiveMessage
    {
        public string playerId;
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
    }

    public class GetResourceDataMessage : IReceiveMessage
    {
        public CharacterState characterState;
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