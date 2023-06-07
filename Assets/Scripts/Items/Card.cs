

namespace UFB.Items {

    // consider using the prototype pattern for cards so we can easily add new cards in the future
    // cards could be loaded from JSON contianing their name & characteristics

    public interface ICardReward {
        
    }

    // a card can give a reward for any ICardReward
    public class Card {

        private readonly ICardReward _reward;

        public Card()
        {
            // randomly assign ICardReward
        }

        public ICardReward Draw()
        {
            return null;
        }

    }

}