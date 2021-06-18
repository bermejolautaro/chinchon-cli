using Chinchon.Domain;
using Chinchon.GameHandlers;
using Chinchon.MenuHandlers;
using ExhaustiveMatching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chinchon
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var path = $"./saves/chinchon-{Guid.NewGuid()}.txt";
            var random = new Random();
            var startHandler = new StartHandler(random);
            var seeHandler = new SeeHandler();
            var pickCardFromDeckHandler = new PickCardFromDeckHandler();
            var pickCardFromPileHandler = new PickCardFromPileHandler();
            var discardCardHandler = new DiscardCardHandler();
            var cutHandler = new CutHandler(random);
            var swapHandler = new SwapHandler();
            var moveHandler = new MoveHandler();

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
                ["swap"] = swapHandler,
                ["move"] = moveHandler,
                ["mv"] = moveHandler
            };

            if (handlersByMenuCommand.ContainsKey(args[0]))
            {
                var response = handlersByMenuCommand[args[0]].Handle(args);

                switch (response)
                {
                    case SuccessResult action:
                        {
                            SaveGameState(path, action.GameState);
                            break;
                        }
                    case ErrorResult action:
                        {
                            Console.Write(action.ErrorMessage);
                            break;
                        }
                    default: throw ExhaustiveMatch.Failed(response);
                }

                return;
            }
            else if (handlersByGameCommand.ContainsKey(args[1]))
            {
                using var sr = File.OpenText($"saves\\{args[0]}.txt");
                GameState gameState = GameService.DeserializeGameState(ReadGameState(sr));

                var response = handlersByGameCommand[args[1]].Handle(args, gameState);

                switch (response.Action)
                {
                    case SaveAction action:
                        {
                            SaveGameState(path, action.GameState);
                            break;
                        }
                    case WriteAction action:
                        {
                            Console.Write(action.Output);
                            break;
                        }
                    case NothingAction _:
                        { break; }
                    default: throw ExhaustiveMatch.Failed(response.Action);
                }
            }
            else
            {
                Console.WriteLine("Invalid command");
            }
        }

        private static string ReadGameState(StreamReader sr)
        {
            var serializedGameStateBuilder = new StringBuilder();

            while (true)
            {
                string? line = sr.ReadLine();

                if (line == null)
                {
                    break;
                }

                serializedGameStateBuilder.AppendLine(line);
            }

            return serializedGameStateBuilder.ToString();
        }

        private static void SaveGameState(string path, GameState gameState)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using StreamWriter fs = File.CreateText(path);
                fs.Write(gameState.SerializeGameState());
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
