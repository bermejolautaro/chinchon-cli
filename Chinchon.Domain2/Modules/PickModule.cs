using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.Domain.Modules
{
    static class PickModule
    {
        internal static IResult PickCardFromDeck(GameState gameState)
        {
            static GameState SetCards(IEnumerable<Card> cards, GameState gameState)
            {
                var newGameState = gameState.Clone();
                newGameState.Deck = cards;
                return newGameState;
            }

            return PickCard(gameState, gameState.Deck, "Deck is empty", SetCards);
        }

        internal static IResult PickCardFromPile(GameState gameState)
        {
            static GameState SetCards(IEnumerable<Card> cards, GameState gameState)
            {
                var newGameState = gameState.Clone();
                newGameState.Pile = cards;
                return newGameState;
            }

            return PickCard(gameState, gameState.Pile, "Pile is empty", SetCards);
        }

        private static IResult PickCard(
            GameState gameState,
            IEnumerable<Card> cardsFrom,
            string errorMessage,
            Func<IEnumerable<Card>, GameState, GameState> setCards)
        {
            var cards = gameState.GetCurrentPlayerCards();

            if (gameState.WasCut)
            {
                return new ErrorResult("Can't pick cards after someone cut");
            }

            if (cards.Count() != 7)
            {
                return new ErrorResult("Only can pick cards when having 7 cards");
            }

            var response = CardsService.RemoveTopCard(cardsFrom);

            if (response.RemovedCard is null)
            {
                return new ErrorResult(errorMessage);
            }

            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                return new ErrorResult(errorMessage);
            }

            gameState = gameState.SetCurrentPlayerCards(cards.Append(response.RemovedCard));

            return new SuccessResult(setCards(response.Cards, gameState));
        }
    }
}
