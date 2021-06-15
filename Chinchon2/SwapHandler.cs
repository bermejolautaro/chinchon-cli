using System.Linq;

namespace Chinchon2
{
    public class SwapHandler : IGameHandler
    {
        public void Handle(string[] args, GameState gameState)
        {
            var firstIndex = int.Parse(args[1]) - 1;
            var secondIndex = int.Parse(args[2]) - 1;

            var cards = GameService.GetCurrentPlayerCards(gameState).ToList();

            var temp = cards[firstIndex];
            cards[firstIndex] = cards[secondIndex];
            cards[secondIndex] = temp;

            var newGameState = GameService.SetCurrentPlayerCards(gameState, cards);

            IOGameService.SaveGameState(IOGameService.path, newGameState);
        }
    }
}
