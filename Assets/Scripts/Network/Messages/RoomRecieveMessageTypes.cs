using UFB.StateSchema;

namespace UFB.Network.RoomMessageTypes
{
    public interface IReceiveMessage { }

    public class PathStep : IReceiveMessage
    {
        public string tileId;
        public CoordinatesState coord;
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