using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.Domain
{
    public class Group : IEnumerable<Card>
    {

        private Group(IEnumerable<Card> cards)
        {
            Cards = cards;
        }

        public IEnumerable<Card> Cards { get; }

        public bool IsChinchon()
        {
            return IsChinchon(Cards);
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return Cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Cards.GetEnumerator();
        }

        public static Group CreateGroup(IEnumerable<Card> cards)
        {
            if(!IsValidGroup(cards))
            {
                throw new ArgumentException(nameof(cards));
            }

            return new Group(cards);
        }

        public static bool TryCreateGroup(IEnumerable<Card> cards, out Group group)
        {
            group = new Group(cards);
            return IsValidGroup(cards);
        }

        public static bool IsValidGroup(IEnumerable<Card> cards)
        {
            if (cards.Count() < 3 || cards.Count() > 7)
                return false;

            if (cards.Count() != cards.Distinct().Count())
                return false;

            return IsValidGroupOfThree(cards) || IsValidGroupOfFour(cards) || IsValidStraight(cards);
        }

        public static bool IsValidGroupOfThree(IEnumerable<Card> cards)
        {
            var cardsCopy = cards.ToList();

            if (cardsCopy.Count != 3)
                return false;

            return cardsCopy[0].Rank == cardsCopy[1].Rank &&
                   cardsCopy[1].Rank == cardsCopy[2].Rank;
        }

        public static bool IsValidGroupOfFour(IEnumerable<Card> cards)
        {
            var cardsCopy = cards.ToList();

            if (cardsCopy.Count != 4)
                return false;

            return cardsCopy[0].Rank == cardsCopy[1].Rank &&
                   cardsCopy[1].Rank == cardsCopy[2].Rank &&
                   cardsCopy[2].Rank == cardsCopy[3].Rank;
        }

        public static bool IsValidStraight(IEnumerable<Card> cards)
        {
            var cardsCopy = cards.ToList();
            if (cardsCopy.Any(c => c.Suit != cardsCopy[0].Suit))
                return false;

            var sortedCards = cardsCopy.OrderBy(c => c.RankValue);
            var lastCard = sortedCards.First();

            foreach (var card in sortedCards.Skip(1))
            {
                if (lastCard.RankValue != card.RankValue - 1)
                {
                    return false;
                }

                lastCard = card;
            }

            return true;
        }

        public static bool IsChinchon(IEnumerable<Card> cards)
        {
            return IsValidStraight(cards) && cards.Count() == 7;
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is Group group))
                return false;

            return new HashSet<Card>(Cards).SetEquals(new HashSet<Card>(group));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Cards);
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", Cards.Select(x => x.ToString()))}]";
        }
    }
}
