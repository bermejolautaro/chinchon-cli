using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.Domain2
{
    public class Group
    {

        public Group(IEnumerable<Card> cards)
        {
            Cards = cards;
        }

        public IEnumerable<Card> Cards { get; }

        public bool IsSubsetOf(IEnumerable<Card> cards)
        {
            return new HashSet<Card>(Cards).IsSubsetOf(new HashSet<Card>(cards));
        }

        public bool Contains(Card card)
        {
            return Cards.Contains(card);
        }

        public static bool IsValidGroup(IEnumerable<Card> cards)
        {
            if (cards.Count() < 3)
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

            var sortedCards = cardsCopy.OrderBy(c => int.Parse(c.Rank));
            var lastCard = sortedCards.First();

            foreach (var card in sortedCards.Skip(1))
            {
                if (int.Parse(lastCard.Rank) != int.Parse(card.Rank) - 1)
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

        public override bool Equals(object obj)
        {
            if (!(obj is Group group))
                return false;

            return new HashSet<Card>(Cards).SetEquals(new HashSet<Card>(group.Cards));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Cards);
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", Cards)}]";
        }
    }
}
