using System;
using System.Linq;

namespace Chinchon2
{
    public class DiscardCardHandler : IGameHandler
    {
        public void Handle(string[] args, GameState gameState)
        {
            if (gameState.TurnState != TurnStateEnum.Picked)
            {
                Console.WriteLine("Only can discard cards on Picked state");
                return;
            }

            var isInteger = int.TryParse(args[1], out int cardIndex);

            if (!isInteger)
            {
                Console.WriteLine("Invalid Input");
                return;
            }

            if (cardIndex < 1 || cardIndex > 8)
            {
                Console.WriteLine("Invalid Card Index");
                return;
            }

            var playerCards = GameService.GetCurrentPlayerCards(gameState);
            var selectedCard = playerCards.ElementAt(cardIndex - 1);

            gameState = GameService.SetCurrentPlayerCards(gameState, playerCards.Where(c => c != selectedCard));
            gameState.Pile = gameState.Pile.Concat(new[] { selectedCard });
            gameState.Turn++;
            gameState.PlayerTurn = ((gameState.Turn - 1) % gameState.PlayerAmount) + 1;
            gameState.TurnState = TurnStateEnum.Initial;

            IOGameService.SaveGameState(IOGameService.path, gameState);
        }
    }
}
