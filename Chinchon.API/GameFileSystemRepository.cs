using Chinchon.API.Dtos;
using Chinchon.API.Responses;
using Chinchon.Domain;
using Chinchon.Domain.Modules;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chinchon.API
{
    public class GameFileSystemRepository : IGameRepository
    {
        private readonly Random _random;
        private readonly ILogger<GameFileSystemRepository> _logger;
        private readonly string _path;

        // TODO: Hacer que no dependa de toda la configuracion, solo el valor que necesita
        public GameFileSystemRepository(Random random, IConfiguration config, IWebHostEnvironment env, ILogger<GameFileSystemRepository> logger)
        {
            _random = random;
            _logger = logger;
            _path = $"{env.ContentRootPath}/{config.GetSection("Paths").GetSection("Saves").Value}";
        }

        public async Task<CreateGameResponse> CreateGame(Player player1, Player player2, Player? player3, Player? player4)
        {
            var result = StartModule.Start(_random, player1, player2);

            var gameGuid = Guid.NewGuid();
            var player1Guid = Guid.NewGuid();
            var player2Guid = Guid.NewGuid();

            switch (result)
            {
                case SuccessResult success:
                    {
                        try
                        {
                            Directory.CreateDirectory(_path);

                            var gameStatePath = GetGameStatePath(_path, gameGuid);
                            await SaveGameState(_path, gameGuid, success.GameState);

                            var extendedStatePath = GetExtendedGameStatePath(_path, gameGuid);
                            await SaveExtendedGameState(_path, gameGuid, new ExtendedGameState()
                            {
                                Player1Guid = player1Guid,
                                Player2Guid = player2Guid,
                                PlayerKeyToBeRetrieved = 2
                            });

                            return new CreateGameResponse()
                            {
                                GameGuid = gameGuid,
                                Player1Guid = player1Guid
                            };
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }
                case ErrorResult error:
                    {
                        throw new Exception(error.ErrorMessage);
                    }
                default:
                    throw new InvalidOperationException("Impossible state");
            }
        }

        public async Task<GameState?> GetGame(Guid gameGuid)
        {
            return await GetGame(_path, gameGuid);
        }

        public async Task<PlayerStateDto?> GetPlayer(Guid gameGuid, Guid playerGuid)
        {
            var gameState = await GetGame(_path, gameGuid);
            var extendedState = await GetExtendedGameState(_path, gameGuid);

            Player player;

            if (extendedState.Player1Guid == playerGuid)
            {
                player = gameState!.Player1;
            }
            else if(extendedState.Player2Guid == playerGuid)
            {
                player = gameState!.Player2;
            }
            else if(extendedState.Player3Guid == playerGuid)
            {
                player = gameState!.Player3!;
            }
            else if(extendedState.Player4Guid == playerGuid)
            {
                player = gameState!.Player4!;
            }
            else
            {
                return null;
            }

            return new PlayerStateDto()
            {
                Id = player.Id,
                Points = player.Points,
                Cards = player.Cards,
                IsPlayerTurn = player.Id == gameState.PlayerTurn,
                TopCardInPile = gameState.Pile.First()
            };
        }

        public async Task<Guid?> GetPlayerKey(Guid gameGuid)
        {
            var extendedState = await GetExtendedGameState(_path, gameGuid);

            var playerKey = extendedState.PlayerKeyToBeRetrieved switch
            {
                1 => extendedState.Player1Guid,
                2 => extendedState.Player2Guid,
                3 => extendedState.Player3Guid,
                4 => extendedState.Player4Guid,
                _ => null
            };

            if(playerKey is null)
            {
                return null;
            }

            extendedState.PlayerKeyToBeRetrieved += 1;

            await SaveExtendedGameState(_path, gameGuid, extendedState);

            return playerKey;
        }

        public async Task<PlayerStateDto?> PickCardFromDeck(Guid gameGuid, Guid playerGuid)
        {
            var gameState = await GetGame(_path, gameGuid);
            var extendedState = await GetExtendedGameState(_path, gameGuid);

            var playerId = extendedState.GetPlayerId(playerGuid);

            if(playerId != gameState?.PlayerTurn)
            {
                return null;
            }

            var result = Mediator.Send(new PickCardFromDeckRequest(gameState));

            switch(result)
            {
                case SuccessResult success:
                    {
                        var player = success.GameState.GetCurrentPlayer();
                        await SaveGameState(_path, gameGuid, success.GameState);

                        return new PlayerStateDto()
                        {
                            Id = player.Id,
                            Points = player.Points,
                            Cards = player.Cards,
                            IsPlayerTurn = player.Id == success.GameState.PlayerTurn,
                            TopCardInPile = success.GameState.Pile.First()
                        };
                    }
                case ErrorResult error:
                    {
                        _logger.LogInformation(error.ErrorMessage);
                        return null;
                    }
            }

            throw new InvalidOperationException("Impossible state");

        }

        public async Task<PlayerStateDto?> PickCardFromPile(Guid gameGuid, Guid playerGuid)
        {
            var gameState = await GetGame(_path, gameGuid);
            var extendedState = await GetExtendedGameState(_path, gameGuid);

            var playerId = extendedState.GetPlayerId(playerGuid);

            if (playerId != gameState?.PlayerTurn)
            {
                return null;
            }

            var result = Mediator.Send(new PickCardFromPileRequest(gameState));

            switch (result)
            {
                case SuccessResult success:
                    {
                        var player = success.GameState.GetCurrentPlayer();
                        await SaveGameState(_path, gameGuid, success.GameState);

                        return new PlayerStateDto()
                        {
                            Id = player.Id,
                            Points = player.Points,
                            Cards = player.Cards,
                            IsPlayerTurn = player.Id == success.GameState.PlayerTurn,
                            TopCardInPile = success.GameState.Pile.First()
                        };
                    }
                case ErrorResult error:
                    {
                        _logger.LogInformation(error.ErrorMessage);
                        return null;
                    }
            }

            throw new InvalidOperationException("Impossible state");
        }

        public async Task<PlayerStateDto?> DiscardCard(Guid gameGuid, Guid playerGuid, Card card)
        {
            var gameState = await GetGame(_path, gameGuid);
            var extendedState = await GetExtendedGameState(_path, gameGuid);

            var playerId = extendedState.GetPlayerId(playerGuid);

            if (playerId != gameState?.PlayerTurn)
            {
                return null;
            }

            var result = Mediator.Send(new DiscardCardRequest(gameState, card));

            switch (result)
            {
                case SuccessResult success:
                    {
                        var player = success.GameState.GetPlayerById(playerId);
                        await SaveGameState(_path, gameGuid, success.GameState);

                        return new PlayerStateDto()
                        {
                            Id = player.Id,
                            Points = player.Points,
                            Cards = player.Cards,
                            IsPlayerTurn = player.Id == success.GameState.PlayerTurn,
                            TopCardInPile = success.GameState.Pile.First()
                        };
                    }
                case ErrorResult _:
                    {
                        return null;
                    }
            }

            throw new InvalidOperationException("Impossible state");
        }

        public async Task<PlayerStateDto?> Cut(Guid gameGuid, Guid playerGuid, Group? firstGroup, Group? secondGroup, Card? cardToCutWith)
        {
            var gameState = await GetGame(_path, gameGuid);
            var extendedState = await GetExtendedGameState(_path, gameGuid);

            var playerId = extendedState.GetPlayerId(playerGuid);

            if (playerId != gameState?.PlayerTurn)
            {
                return null;
            }

            var groups = new[] { firstGroup, secondGroup }.Where(x => x != null).Cast<Group>();

            var result = Mediator.Send(new CutRequest(gameState, _random, groups, cardToCutWith));

            switch (result)
            {
                case SuccessResult success:
                    {
                        var player = success.GameState.GetPlayerById(playerId);
                        await SaveGameState(_path, gameGuid, success.GameState);

                        return new PlayerStateDto()
                        {
                            Id = player.Id,
                            Points = player.Points,
                            Cards = player.Cards,
                            IsPlayerTurn = player.Id == gameState.PlayerTurn,
                            TopCardInPile = gameState.Pile.First()
                        };
                    }
                case ErrorResult error:
                    {
                        _logger.LogInformation(error.ErrorMessage);
                        return null;
                    }
            }

            throw new InvalidOperationException("Impossible state");
        }

        private static async Task<ExtendedGameState> GetExtendedGameState(string path, Guid gameGuid)
        {
            using var streamReader = File.OpenText(GetExtendedGameStatePath(path, gameGuid));
            var serializedExtendedGameStateBuilder = new StringBuilder();

            while (true)
            {
                string? line = await streamReader.ReadLineAsync();

                if (line == null)
                {
                    break;
                }

                serializedExtendedGameStateBuilder.AppendLine(line);
            }

            var extendedGameState = System.Text.Json.JsonSerializer.Deserialize<ExtendedGameState>(serializedExtendedGameStateBuilder.ToString());

            return extendedGameState;
        }

        private static async Task SaveExtendedGameState(string path, Guid gameGuid, ExtendedGameState extendedGameState)
        {
            using StreamWriter fsExtendedState = File.CreateText(GetExtendedGameStatePath(path, gameGuid));
            var extendedStateJson = System.Text.Json.JsonSerializer.Serialize(extendedGameState);
            await fsExtendedState.WriteAsync(extendedStateJson);
            fsExtendedState.Close();
        }

        private static async Task<GameState?> GetGame(string path, Guid gameGuid)
        {
            using var streamReader = File.OpenText(GetGameStatePath(path, gameGuid));
            var serializedGameStateBuilder = new StringBuilder();

            while (true)
            {
                string? line = await streamReader.ReadLineAsync();

                if (line == null)
                {
                    break;
                }

                serializedGameStateBuilder.AppendLine(line);
            }

            var gameStateDto = JsonConvert.DeserializeObject<GameStateDto>(serializedGameStateBuilder.ToString(), new StringEnumConverter());

            return gameStateDto.ToGameState();
        }

        private static async Task SaveGameState(string path, Guid gameGuid, GameState gameState)
        {
            using StreamWriter fs = File.CreateText(GetGameStatePath(path, gameGuid));
            //var gameStateJson = JsonSerializer.Serialize(gameState, options: new JsonSerializerOptions{ WriteIndented = true,  });
            var gameStateJson = JsonConvert.SerializeObject(gameState, Formatting.Indented, new StringEnumConverter());
            await fs.WriteAsync(gameStateJson);
            fs.Close();
        }

        private static string GetGameStatePath(string path, Guid gameGuid)
        {
            return $"{path}/{gameGuid}.game.json";
        }


        private static string GetExtendedGameStatePath(string path, Guid gameGuid)
        {
            return $"{path}/{gameGuid}.extended.json";
        }
    }
}
