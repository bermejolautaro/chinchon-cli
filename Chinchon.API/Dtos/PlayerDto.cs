using Chinchon.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Chinchon.API.Dtos
{
    public class PlayerDto
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public ICollection<CardDto> Cards { get; set; } = new Collection<CardDto>();
    }

    public static class PlayerDtoExtensions
    {
        public static Player ToPlayer(this PlayerDto p)
        {
            return new Player(new PlayerOptions()
            {
                Id = p.Id,
                Points = p.Points,
                Cards = p.Cards.Select(x => new Card(x.Suit, x.Rank))
            });
        }
    }
}
