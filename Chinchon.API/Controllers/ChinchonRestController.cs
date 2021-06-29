using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chinchon.API.Dtos;
using Chinchon.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Chinchon.API.Controllers
{
    [ApiController]
    [Route("api/rest/chinchon")]
    public class ChinchonRestController : ControllerBase
    {
        private readonly IGameRepository _gameRepository;

        public ChinchonRestController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        [HttpPost("game")]
        public async Task<ActionResult<GameState>> Post()
        {
            var response = await _gameRepository.CreateGame(new Player(1), new Player(2));

            return Ok(new { response.GameGuid, Player1 = response.Player1Guid });
        }

        [HttpGet("game/{gameGuid}/player-key")]
        public async Task<ActionResult<Guid?>> GetPlayerKey(Guid gameGuid)
        {
            return await _gameRepository.GetPlayerKey(gameGuid);
        }

        [HttpGet("game/{gameGuid}/players/{playerGuid}")]
        public async Task<PlayerStateDto?> GetPlayer(Guid gameGuid, Guid playerGuid)
        {
            return await _gameRepository.GetPlayer(gameGuid, playerGuid);
        }
    }
}
