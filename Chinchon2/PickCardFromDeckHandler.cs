using Chinchon.Domain2;
using System.Collections.Generic;

namespace Chinchon2
{
    public class PickCardFromDeckHandler : PickCardHandler
    {
        protected override IEnumerable<Card> GetCards(GameState gameState)
        {
            return gameState.Deck;
        }

        protected override void SetCards(GameState gameState, IEnumerable<Card> cards)
        {
            gameState.Deck = cards;
        }
    }
}
