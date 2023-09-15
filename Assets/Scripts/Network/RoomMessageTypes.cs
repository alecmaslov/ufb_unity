namespace UFB.Network
{

    public class PathStep {
        public string tileId;
        public ServerCoordinates coord;
    }
    
    public class PlayerMovedMessage
    {
        public string playerId;
        public PathStep[] path;
    }

    public class NotificationMessage
    {
        public string type;
        public string message;
    }
}