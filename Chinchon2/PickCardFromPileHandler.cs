using Chinchon.Domain2;
using System.Collections.Generic;

namespace Chinchon2
{
    public class PickCardFromPileHandler : PickCardHandler
    {
        protected override IEnumerable<Card> GetCards(GameState gameState)
        {
            return gameState.Pile;
        }

        protected override void SetCards(GameState gameState, IEnumerable<Card> cards)
        {
            gameState.Pile = cards;
        }
    }
}
