using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.Domain.Modules
{
    static class MoveModule
    {
        internal static IResult Move(GameState gameState, Card card, int toPosition)
        {
            if (toPosition < 1 || toPosition > 8)
            {
                return new ErrorResult("Invalid index");
            }

            var cards = gameState.GetCurrentPlayer().Cards.ToList();

            cards.Remove(card);
            cards.Insert(toPosition - 1, card);

            var newGameState = gameState.SetCurrentPlayerCards(cards);

            return new SuccessResult(newGameState);
        }
    }
}
