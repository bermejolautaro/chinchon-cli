using Chinchon.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chinchon.API.Dtos
{
    public class CardDto
    {
        public SuitsEnum Suit { get; set; }
        public RanksEnum Rank { get; set; }
    }

    public static class CardDtoExtensions
    {
        public static Card ToCard(this CardDto c)
        {
            return new Card(c.Suit, c.Rank);
        }
    }

    public static class IEnumerableCardDtoExtensions
    {
        public static IEnumerable<Card> ToCards(this IEnumerable<CardDto> cs)
        {
            return cs.Select(x => x.ToCard());
        }
    }
}
