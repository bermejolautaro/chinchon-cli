using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.Domain
{
    public static class CardsService
    {
        public static Card DeserializeCard(string card)
        {
            var removedParentheses = string.Join("", card.Where(x => x != '(' && x != ')'));
            var splitted = removedParentheses.Split(", ");
            var suit = splitted[0];
            var rank = splitted[1];

            return new Card((SuitsEnum)Enum.Parse(typeof(SuitsEnum), suit), (RanksEnum)Enum.Parse(typeof(RanksEnum), rank));
        }

        public static IEnumerable<Card> GetCards()
        {
            return Enum.GetValues(typeof(RanksEnum))
                .Cast<RanksEnum>()
                .SelectMany(rank => Enum.GetValues(typeof(SuitsEnum)).Cast<SuitsEnum>().Select(suit => new Card(suit, rank)));
        }

        public static IEnumerable<Card> ShuffleCards(IEnumerable<Card> cards)
        {
            var cardsCopy = cards.ToList();
            var random = new Random();

            for (int i = cardsCopy.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                var temp = cardsCopy[i];
                cardsCopy[i] = cardsCopy[j];
                cardsCopy[j] = temp;
            }

            return cardsCopy;
        }

        public static IEnumerable<Card> ShuffleCards(Random random, IEnumerable<Card> cards)
        {
            var cardsCopy = cards.ToList();

            for (int i = cardsCopy.Count - 1; i > 0; i--)
            {
                var j = random.Next(i + 1);
                var temp = cardsCopy[i];
                cardsCopy[i] = cardsCopy[j];
                cardsCopy[j] = temp;
            }

            return cardsCopy;
        }

        public static RemoveCardResponse RemoveTopCard(IEnumerable<Card> cards)
        {
            var topCard = cards.FirstOrDefault();
            if (topCard is null)
            {
                return new RemoveCardResponse()
                {
                    Cards = Enumerable.Empty<Card>(),
                    RemovedCard = null,
                    ErrorMessage = "Cards is empty"
                };
            }

            return new RemoveCardResponse
            {
                Cards = cards.Where(c => c != topCard),
                RemovedCard = topCard
            };
        }
    }

    public class RemoveCardResponse
    {
        public IEnumerable<Card> Cards { get; set; } = Enumerable.Empty<Card>();
        public Card? RemovedCard { get; set; } = null;
        public string ErrorMessage { get; set; } = "";
    }
}
