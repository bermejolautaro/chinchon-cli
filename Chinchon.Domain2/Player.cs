using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.Domain2
{
    public class Player
    {
        private readonly IList<Card> _cards;
        private IEnumerable<Group> _groups = Enumerable.Empty<Group>();

        public Player()
        {
            _cards = new List<Card>();
            Points = 0;
        }

        public IEnumerable<Card> Cards => _cards;
        public IEnumerable<Group> Groups => _groups;

        public int Points { get; private set; }

        public void SetCardAtPosition(int cardInHandPosition, Card card)
        {
            _cards[cardInHandPosition] = card;
        }

        public Card GetCardAtPosition(int position)
        {
            return _cards[position];
        }

        public void AddPoints(int amount)
        {
            Points = amount;
        }

        public int CalculatePoints()
        {
            return Points;
        }

        internal void RemoveCard(Card card)
        {
            _cards.Remove(card);
        }

        public void AddCard(Card card)
        {
            _cards.Add(card);
        }

        public void SetGroups(IEnumerable<Group> otherGroups)
        {
            _groups = otherGroups;
        }
    }
}
