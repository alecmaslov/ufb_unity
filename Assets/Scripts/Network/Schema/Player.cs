using Colyseus;
using Colyseus.Schema;
using UFB.Map;
using UFB.Entities;
using UFB.Items;
using UFB.Player;

namespace UFB.Network
{
    public class PlayerStats : Schema
    {
        [Type(0, "number")]
        public float health;
        [Type(1, "number")]
        public float energy;
    }

    public class _PlayerState : Schema
    {
        // public readonly string Name;
        [Type(0, "string")]
        public string id;

        [Type(1, "number")]
        public float x;

        [Type(2, "number")]
        public float y;

        [Type(3, "ref", typeof(PlayerStats))]
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
    }

    public class PlayerJoined : Schema
    {
        [Type(0, "string")]
        public string clientId;

        [Type(1, "boolean")]
        public bool isMe = false;

        [Type(2, "number")]
        public float x;

        [Type(3, "number")]
        public float y;

        public Coordinates Coordinates => new Coordinates((int)x, (int)y);
    }



    public class PlayerMoved : Schema
    {
        [Type(0, "string")]
        public string playerId;

        [Type(1, "number")]
        public float x;

        [Type(2, "number")]
        public float y;

        public Coordinates NewCoords => new Coordinates((int)x, (int)y);
    }

}