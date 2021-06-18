using System.Linq;

namespace Chinchon.Domain.Modules
{
    static class DiscardModule
    {
        internal static IResult DiscardCard(GameState gameState, Card cardToDiscard)
        {
            var playerCards = gameState.GetCurrentPlayerCards();

            if (playerCards.Count() != 8)
            {
                return new ErrorResult("Only can discard cards when you have eight cards in your hand");
            }

            var selectedCard = playerCards.FirstOrDefault(x => x == cardToDiscard);

            if(selectedCard is null)
            {
                return new ErrorResult("Can't discard a card that you don't have");
            }

            gameState = gameState.SetCurrentPlayerCards(playerCards.Where(c => c != selectedCard));
            gameState.Pile = gameState.Pile.Prepend(selectedCard);
            gameState.PlayerTurn = gameState.PlayerTurn % gameState.PlayerAmount + 1;
            gameState.Turn++;

            return new SuccessResult(gameState);
        }
    }
}
