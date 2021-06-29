using Chinchon.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chinchon.API.Dtos
{
    public class PlayerStateDto
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public IEnumerable<Card> Cards { get; set; } = Enumerable.Empty<Card>();
        public bool IsPlayerTurn { get; set; }
        public Card? TopCardInPile { get; set; }
    }
}
