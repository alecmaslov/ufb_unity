
namespace UFB.Player {
    public struct StatusValue<T> {
        public T value;
        public T maxValue;
    }


    // consider making a StatusItem that has
    /// - StatusValue value
    /// - Sprite icon

    /// or an IStatus

    public struct PlayerStatus {
        public StatusValue<int> Health;
        public Energy Energy;

        // I don't think the following things are really status
        // objects, but rather are inventory items. 
        // when we want to render the player status, it could be a combination
        // of items in the inventory that have an IStatusValue


        // public StatusValue<int> MeleeTokens;
        // public StatusValue<int> MagicTokens;
        // public StatusValue<int> EnergyShards;
        // public StatusValue<int> EnergyCrystals;
        // public StatusValue<int> HeartCrystals;
    }


    // even though things like health, energy, magictokens, shards, are
    // not inherently status values, since they do other things, they
    // should implement an interface so that various game APIs can
    // access a value and display it as a status value
    public interface IStatusValue {

    }

    public class Energy : IStatusValue {
        public int Value { get; private set; }
        public int Income { get; private set; } // amount of energy recieved each turn
    }
}