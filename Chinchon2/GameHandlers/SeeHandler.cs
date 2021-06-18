using Chinchon.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.GameHandlers
{
    public class SeeHandler : IGameHandler
    {
        private static readonly Dictionary<string, Func<GameState, string>> _handlersByCommand = new Dictionary<string, Func<GameState, string>>()
        {
            ["current"] = SeeCurrentPlayer,
            ["c"] = SeeCurrentPlayer
        };

        public HandlerResponse Handle(string[] args, GameState gameState)
        {
            if (args.Length == 1)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Player 1");
                sb.AppendLine($"Points: {gameState.Player1Points}");

                foreach (var card in gameState.Player1Cards)
                {
                    sb.AppendLine(card.ToString());
                }

                sb.AppendLine();
                sb.AppendLine();

                sb.AppendLine("Player 2");
                sb.AppendLine($"Points: {gameState.Player2Points}");

                foreach (var card in gameState.Player2Cards)
                {
                    sb.AppendLine(card.ToString());
                }

                return new HandlerResponse()
                {
                    Action = new WriteAction(sb.ToString())
                };
            }
            else
            {
                var output = _handlersByCommand[args[2]].Invoke(gameState);

                return new HandlerResponse()
                {
                    Action = new WriteAction(output)
                };
            }
        }

        private static string SeeCurrentPlayer(GameState gameState)
        {
            var sb = new StringBuilder();
            if (gameState.PlayerTurn == 1)
            {
                sb.AppendLine("Player 1");
                sb.AppendLine($"Points: {gameState.Player1Points}");
            }
            else
            {
                sb.AppendLine("Player 2");
                sb.AppendLine($"Points: {gameState.Player2Points}");
            }

            sb.AppendLine($"Pile: {gameState.Pile.FirstOrDefault()}");

            var cards = gameState.GetCurrentPlayerCards().ToList();

            for (int i = 0; i < cards.Count(); i++)
            {
                var card = cards[i];
                sb.AppendLine($"{i + 1}. {card}");
            }

            return sb.ToString();
        }
    }
}
