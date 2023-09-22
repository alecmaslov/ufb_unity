using UFB.Map;
using UFB.Entities;
using UFB.Items;
using Colyseus.Schema;

namespace UFB.Player
{

    public class Quest
    {

        public enum Difficulty
        {
            Normal,
            Hard
        }

        public bool IsActive { get; set; }

    }


    public class PlayerStats : Schema 
    {
        [Colyseus.Schema.Type(0, "number")]
        public float health;
        [Colyseus.Schema.Type(1, "number")]
        public float energy;
    }

    // IPlayer could be a real player or an NPC
    // so this uses an interface, which has properties
    // and methods that all players will have
    public class _PlayerState : Schema
    {
        // public readonly string Name;
        [Colyseus.Schema.Type(0, "string")]
        public string id;

        [Colyseus.Schema.Type(1, "number")]
        public float x;

        [Colyseus.Schema.Type(2, "number")]
        public float y;

        [Colyseus.Schema.Type(3, "ref", typeof(PlayerStats))]
        public PlayerStats Stats;



        public Coordinates Position => new Coordinates((int)x, (int)y);

        public PlayerEntity Entity { get; set; }


        // public Health Health { get; }
        // public Energy Energy { get; }


        // public IInventoryItem[] Items { get; set; }
        public PlayerInventory Inventory { get; set; }

        // public PlayerStatus Status { get; set; } <- instead of this, we can simplify and have it be
        // an array of IStatusValue | or we could have a GetStatus() function that returns the things that
        // are status values
        public IStatusValue[] Status { get; }

        public Quest[] Quests { get; set; }

        // public UFBPlayer(string name, string id)
        // {
        //     Name = name;
        //     this.id = id;
        // }
    }

}