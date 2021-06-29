using Chinchon.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.GameHandlers
{
    public class SeeHandler : IGameHandler
    {
        private static readonly Dictionary<string, Func<GameState, ApplicationState, string>> _handlersByCommand = new Dictionary<string, Func<GameState, ApplicationState, string>>()
        {
            ["current"] = SeeCurrentPlayer,
            ["c"] = SeeCurrentPlayer
        };

        public HandlerResponse Handle(string[] args, GameState gameState, ApplicationState appState)
        {
            if (args.Length == 1)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(appState.Player1Name);
                sb.AppendLine($"Points: {gameState.Player1.Points}");

                foreach (var card in gameState.Player1.Cards)
                {
                    sb.AppendLine(card.ToString());
                }

                sb.AppendLine();
                sb.AppendLine();

                sb.AppendLine(appState.Player2Name);
                sb.AppendLine($"Points: {gameState.Player2.Points}");

                foreach (var card in gameState.Player2.Cards)
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
                var output = _handlersByCommand[args[1]].Invoke(gameState, appState);

                return new HandlerResponse()
                {
                    Action = new WriteAction(output)
                };
            }
        }

        private static string SeeCurrentPlayer(GameState gameState, ApplicationState appState)
        {
            var sb = new StringBuilder();

            sb.AppendLine(appState.GetPlayerNameById(gameState.GetCurrentPlayer().Id));
            sb.AppendLine($"Points: {gameState.GetCurrentPlayer().Points}");

            sb.AppendLine($"Pile: {gameState.Pile.FirstOrDefault()}");

            var cards = gameState.GetCurrentPlayer().Cards.ToList();

            for (int i = 0; i < cards.Count(); i++)
            {
                var card = cards[i];
                sb.AppendLine($"{i + 1}. {card}");
            }

            return sb.ToString();
        }
    }
}
