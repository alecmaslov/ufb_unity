using UFB.Map;
using UFB.Entities;
using UFB.Items;

namespace UFB.Player
{

    public class Quest {

        public enum Difficulty {
            Normal,
            Hard
        }

        public bool IsActive { get; set; }

    }

    // IPlayer could be a real player or an NPC
    // so this uses an interface, which has properties
    // and methods that all players will have
    public class Player {
        
        public readonly string Name;
        public readonly int Id;

        public Health Health { get; }
        public Energy Energy { get; }

        public PlayerEntity Entity { get; set; }

        public Coordinate Position { get; set; }

        // public IInventoryItem[] Items { get; set; }
        public PlayerInventory Inventory { get; set; }

        // public PlayerStatus Status { get; set; } <- instead of this, we can simplify and have it be
        // an array of IStatusValue | or we could have a GetStatus() function that returns the things that
        // are status values
        public IStatusValue[] Status { get; }

        public Quest[] Quests { get; set; }

        public Player(string name, int id) {
            Name = name;
            Id = id;
        }
    }


    // public class HumanPlayer : Player {

    // }
    
}