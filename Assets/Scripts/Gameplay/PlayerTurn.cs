namespace UFB.Gameplay {

    public class PlayerTurn {
        public readonly int PlayerId;
        public readonly int TurnNumber;

        public PlayerTurn(int playerId, int turnNumber) {
            PlayerId = playerId;
            TurnNumber = turnNumber;
        }
    }

}