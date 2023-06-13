
namespace UFB.Player {
    public struct StatusValue {
        public int value;
        public int maxValue;

        public StatusValue(int value, int maxValue) {
            this.value = value;
            this.maxValue = maxValue;
        }

        public float Percent {
            get {
                return (float)value / (float)maxValue;
            }
        }

        public void Add(int amount) {
            value += amount;
            if (value > maxValue) {
                value = maxValue;
            }
        }

        public void Subtract(int amount) {
            value -= amount;
            if (value < 0) {
                value = 0;
            }
        }
    }


    // even though things like health, energy, magictokens, shards, are
    // not inherently status values, since they do other things, they
    // should implement an interface so that various game APIs can
    // access a value and display it as a status value
    public interface IStatusValue {
        int Value { get; }
    }

    public class Energy : IStatusValue {
        public int Value { get; private set; }
        public int Income { get; private set; } // amount of energy recieved each turn
    }

    public class Health : IStatusValue {
        public int Value { get; private set; }
        public int MaxValue { get; private set; }
    }
}