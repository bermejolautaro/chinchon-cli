using Chinchon.Domain;
using Chinchon.GameHandlers;
using Chinchon.MenuHandlers;
using ExhaustiveMatching;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace Chinchon
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var random = new Random();
            var startHandler = new StartHandler(random);
            var loadHandler = new LoadHandler();
            var seeHandler = new SeeHandler();
            var pickCardFromDeckHandler = new PickCardFromDeckHandler();
            var pickCardFromPileHandler = new PickCardFromPileHandler();
            var discardCardHandler = new DiscardCardHandler();
            var cutHandler = new CutHandler(random);
            var moveHandler = new MoveHandler();

            var handlersByMenuCommand = new Dictionary<string, IMenuHandler>()
            {
                ["start"] = startHandler,
                ["load"] = loadHandler
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
                ["move"] = moveHandler,
                ["mv"] = moveHandler
            };

            if (handlersByMenuCommand.ContainsKey(args[0]))
            {
                var response = handlersByMenuCommand[args[0]].Handle(args);

                switch (response.Action)
                {
                    case SaveStateAction action:
                        {
                            var guid = Guid.NewGuid();
                            SaveConfiguration(guid);
                            SaveGameState(GetGameStatePath(guid), action.GameState);
                            SaveAppState(GetAppStatePath(guid), action.AppState);
                            break;
                        }
                    case SaveConfigurationAction action:
                        {
                            SaveConfiguration(action.GameGuid);
                            break;
                        }
                    case WriteAction action:
                        {
                            Console.Write(action.Output);
                            break;
                        }
                    case NothingAction _:
                        { break; }

                    default: throw ExhaustiveMatch.Failed(response);
                }

                return;
            }
            else if (handlersByGameCommand.ContainsKey(args[0]))
            {
                var lastLoadedGameGuid = ReadConfiguration();
                ApplicationState? appState = null;
                GameState? gameState = null;

                try
                {
                    using var srAppState = File.OpenText(GetAppStatePath(lastLoadedGameGuid));
                    using var srGameState = File.OpenText(GetGameStatePath(lastLoadedGameGuid));
                    appState = ReadAppState(srAppState);
                    gameState = GameService.DeserializeGameState(ReadGameState(srGameState));

                    if(gameState is null || appState is null)
                    {
                        throw new Exception("Error trying to load state");
                    }
                }
                catch(FileNotFoundException)
                {
                    if (lastLoadedGameGuid == Guid.Empty)
                    {
                        Console.WriteLine($"Empty game id. Load an existing game");
                    }
                    else
                    {
                        Console.WriteLine($"Game with id: {lastLoadedGameGuid} does not exist");
                    }

                    return;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }

                var response = handlersByGameCommand[args[0]].Handle(args, gameState, appState);

                switch (response.Action)
                {
                    case SaveStateAction action:
                        {
                            SaveGameState(GetGameStatePath(lastLoadedGameGuid), action.GameState);
                            break;
                        }
                    case SaveConfigurationAction _:
                        { break; }
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

        private static Guid ReadConfiguration()
        {
            var lastGameLoadedGuid = new Guid(ConfigurationManager.AppSettings["LastLoadedGameGuid"]);

            return lastGameLoadedGuid;
        }

        private static ApplicationState ReadAppState(StreamReader sr)
        {
            var serializedAppStateBuilder = new StringBuilder();

            while (true)
            {
                string? line = sr.ReadLine();

                if (line == null)
                {
                    break;
                }

                serializedAppStateBuilder.AppendLine(line);
            }

            return JsonConvert.DeserializeObject<ApplicationState>(serializedAppStateBuilder.ToString())!;
        }

        private static void SaveAppState(string path, ApplicationState appState)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                using StreamWriter fs = File.CreateText(path);
                fs.Write(appState.ToString());
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void SaveConfiguration(Guid guid)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;

            settings["LastLoadedGameGuid"].Value = guid.ToString();

            configFile.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
        }

        private static string GetGameStatePath(Guid gameGuid)
        {
            return $"./saves/{gameGuid}.game.txt";
        }

        private static string GetAppStatePath(Guid gameGuid)
        {
            return $"./saves/{gameGuid}.app.txt";
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
