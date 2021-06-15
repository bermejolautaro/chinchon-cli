using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.Domain2
{
    public class Round
    {
        private static readonly IEnumerable<string> _suits = new[] { "Clubs", "Golds", "Cups", "Swords" };
        private static readonly IEnumerable<string> _ranks = Enumerable.Range(1, 12).Select(x => x.ToString());

        private readonly IList<Card> _deck = new List<Card>();
        private readonly IList<Card> _pile = new List<Card>();

        public Round()
        {
            _deck = ShuffleCards(GetCards()).ToList();
        }

        public Card PickCardFromDeck()
        {
            var card = _deck.Last();
            _deck.Remove(card);
            return card;
        }

        public Card PickCardFromPile()
        {
            var card = _pile.Last();
            _pile.Remove(card);
            return card;
        }

        public void PutCardOnPile(Card card)
        {
            _pile.Add(card);
        }

        public bool IsPileEmpty()
        {
            return _pile.Count == 0;
        }

        public Card PeekCardFromPile()
        {
            return _pile[_pile.Count - 1];
        }

        private static IEnumerable<Card> GetCards()
        {
            return _ranks.SelectMany(rank => _suits.Select(suit => new Card(suit, rank)));
        }

        private static IEnumerable<Card> ShuffleCards(IEnumerable<Card> cards)
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
    }
}
