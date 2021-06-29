using Chinchon.API.Dtos;
using Chinchon.API.Responses;
using Chinchon.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chinchon.API
{
    public interface IGameRepository
    {
        Task<CreateGameResponse> CreateGame(Player player1, Player player2, Player? player3 = null, Player? player4 = null);
        Task<GameState?> GetGame(Guid gameGuid);
        Task<PlayerStateDto?> GetPlayer(Guid gameGuid, Guid playerGuid);
        Task<Guid?> GetPlayerKey(Guid gameGuid);
        Task<PlayerStateDto?> PickCardFromDeck(Guid gameGuid, Guid playerGuid);
        Task<PlayerStateDto?> PickCardFromPile(Guid gameGuid, Guid playerGuid);
        Task<PlayerStateDto?> DiscardCard(Guid gameGuid, Guid playerGuid, Card card);
        Task<PlayerStateDto?> Cut(Guid gameGuid, Guid playerGuid, Group? firstGroup, Group? secondGroup, Card? cardToCutWith);
    }
}
