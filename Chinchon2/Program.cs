using System;
using System.Collections.Generic;
using System.IO;

namespace Chinchon2
{
    class Program
    {
        static void Main(string[] args)
        {
            var startHandler = new StartHandler();
            var seeHandler = new SeeHandler();
            var pickCardFromDeckHandler = new PickCardFromDeckHandler();
            var pickCardFromPileHandler = new PickCardFromPileHandler();
            var discardCardHandler = new DiscardCardHandler();
            var cutHandler = new CutHandler();
            var swapHandler = new SwapHandler();

            var handlersByMenuCommand = new Dictionary<string, IMenuHandler>()
            {
                ["start"] = startHandler,
            };

            var handlersByGameCommand = new Dictionary<string, IGameHandler>()
            {
                ["see"] = seeHandler,
                ["pickCardFromDeck"] = pickCardFromDeckHandler,
                ["pd"] = pickCardFromDeckHandler,
                ["pickCardFromPile"] = pickCardFromPileHandler,
                ["pp"] = pickCardFromPileHandler,
                ["discardCard"] = discardCardHandler,
                ["dc"] = discardCardHandler,
                ["cut"] = cutHandler,
                ["swap"] = swapHandler
            };

            if (handlersByMenuCommand.ContainsKey(args[0]))
            {
                handlersByMenuCommand[args[0]].Handle(args);
                return;
            }
            else if (handlersByGameCommand.ContainsKey(args[0]))
            {
                using StreamReader sr = File.OpenText(IOGameService.path);
                GameState gameState = GameService.DeserializeGameState(sr);

                if (!string.IsNullOrEmpty(gameState.Winner))
                {
                    Console.WriteLine($"This game has already ended. The winner is: {gameState.Winner}");
                }
                else
                {
                    handlersByGameCommand[args[0]].Handle(args, gameState);
                }
            }
            else
            {
                Console.WriteLine("Invalid command");
            }
        }
    }
}
