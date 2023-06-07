


public class Player {

    IInventoryItem[] inventory { get; set; }

}




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
    public StatusValue<int> Energy;
    public StatusValue<int> MagicTokens;
    public StatusValue<int> EnergyShards;
    public StatusValue<int> EnergyCrystals;
    public StatusValue<int> HeartCrystals;
}