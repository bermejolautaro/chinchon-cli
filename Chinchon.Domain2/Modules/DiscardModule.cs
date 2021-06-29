using System.Linq;

namespace Chinchon.Domain.Modules
{
    static class DiscardModule
    {
        internal static IResult DiscardCard(GameState gameState, Card cardToDiscard)
        {
            var playerCards = gameState.GetCurrentPlayer().Cards;

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
            gameState = gameState.With(options => options.Pile = gameState.Pile.Prepend(selectedCard));
            gameState = gameState.With(options => options.PlayerTurn = gameState.PlayerTurn % gameState.GetPlayers().Count() + 1);
            gameState = gameState.With(options => options.Turn = gameState.Turn + 1);

            return new SuccessResult(gameState);
        }
    }
}
