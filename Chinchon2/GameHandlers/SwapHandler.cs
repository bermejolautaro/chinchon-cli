using Chinchon.Domain;
using System.Linq;

namespace Chinchon.GameHandlers
{
    public class SwapHandler : IGameHandler
    {

        public HandlerResponse Handle(string[] args, GameState gameState)
        {
            var firstIndex = int.Parse(args[1]) - 1;
            var secondIndex = int.Parse(args[2]) - 1;

            var cards = gameState.GetCurrentPlayerCards().ToList();

            var temp = cards[firstIndex];
            cards[firstIndex] = cards[secondIndex];
            cards[secondIndex] = temp;

            //var newGameState = gameState.SetCurrentPlayerCards(cards);

            return new HandlerResponse()
            {
                Action = new SaveAction(gameState)
            };
        }
    }
}
