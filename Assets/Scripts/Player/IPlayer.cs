

namespace UFB.Player
{
    // IPlayer could be a real player or an NPC
    // so this uses an interface, which has properties
    // and methods that all players will have
    public interface IPlayer {

        public PlayerInventory Inventory { get; set; }

        // public PlayerStatus Status { get; set; } <- instead of this, we can simplify and have it be
        // an array of IStatusValue
        public IStatusValue[] Status { get; }


    }


    public class HumanPlayer : IPlayer {

        public PlayerInventory Inventory { get; set; }
        public IStatusValue[] Status { get; }

    }
    
}