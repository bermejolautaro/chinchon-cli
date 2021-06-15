using Chinchon.Domain2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon2
{
    public static class CardsService
    {
        public static Card DeserializeCard(string card)
        {
            var removedParentheses = string.Join("", card.Where(x => x != '(' && x != ')'));
            var splitted = removedParentheses.Split(", ");
            var suit = splitted[0];
            var rank = splitted[1];

            return new Card(suit, rank);
        }

        public static IEnumerable<Card> GetCards()
        {
            return Constants.Ranks.SelectMany(rank => Constants.Suits.Select(suit => new Card(suit, rank)));
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

        public static RemoveCardResponse RemoveTopCard(IEnumerable<Card> cards)
        {
            var topCard = cards.First();
            return new RemoveCardResponse
            {
                Cards = cards.Where(c => c != topCard),
                RemovedCard = topCard
            };
        }
    }

    public class RemoveCardResponse
    {
        public IEnumerable<Card> Cards { get; set; }
        public Card RemovedCard { get; set; }
    }
}
