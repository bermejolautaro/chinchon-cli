using System;
using System.Collections.Generic;
using System.Linq;

namespace Chinchon2
{
    public class SeeHandler : IGameHandler
    {
        private static Dictionary<string, Action<GameState>> _handlersByCommand = new Dictionary<string, Action<GameState>>()
        {
            ["current"] = SeeCurrentPlayer,
            ["c"] = SeeCurrentPlayer
        };

        public void Handle(string[] args, GameState gameState)
        {
            if (args.Length == 1)
            {
                Console.WriteLine("Player 1");
                Console.WriteLine($"Points: {gameState.Player1Points}");

                foreach (var card in gameState.Player1Cards)
                {
                    Console.WriteLine(card.ToString());
                }

                Console.WriteLine();
                Console.WriteLine();

                Console.WriteLine("Player 2");
                Console.WriteLine($"Points: {gameState.Player2Points}");

                foreach (var card in gameState.Player2Cards)
                {
                    Console.WriteLine(card.ToString());
                }
            }
            else
            {
                _handlersByCommand[args[1]].Invoke(gameState);
            }
        }

        private static void SeeCurrentPlayer(GameState gameState)
        {
            if (gameState.PlayerTurn == 1)
            {
                Console.WriteLine("Player 1");
                Console.WriteLine($"Points: {gameState.Player1Points}");
            }
            else
            {
                Console.WriteLine("Player 2");
                Console.WriteLine($"Points: {gameState.Player2Points}");
            }

            var cards = GameService.GetCurrentPlayerCards(gameState).ToList();

            for (int i = 0; i < cards.Count(); i++)
            {
                var card = cards[i];
                Console.WriteLine($"{i + 1}. {card}");
            }
        }
    }
}
