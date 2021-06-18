using ExhaustiveMatching;
using System;

namespace Chinchon.Domain
{
    public class Card
    {
        public Card(SuitsEnum suit, RanksEnum rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public SuitsEnum Suit { get; }
        public RanksEnum Rank { get; }
        public int RankValue => Rank switch
        {
            RanksEnum.One => 1,
            RanksEnum.Two => 2,
            RanksEnum.Three => 3,
            RanksEnum.Four => 4,
            RanksEnum.Five => 5,
            RanksEnum.Six => 6,
            RanksEnum.Seven => 7,
            RanksEnum.Eight => 8,
            RanksEnum.Nine => 9,
            RanksEnum.Jack => 10,
            RanksEnum.Knight => 11,
            RanksEnum.King => 12,
            _ => throw ExhaustiveMatch.Failed(RankValue)
        };

        public override bool Equals(object? obj)
        {
            return obj is Card card &&
                   Suit == card.Suit &&
                   Rank == card.Rank;
        }

        public static bool operator ==(Card lhs, Card rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                return false;
            }

            return lhs.Equals(rhs);
        }

        public static bool operator !=(Card lhs, Card rhs) => !(lhs == rhs);

        public override int GetHashCode()
        {
            return HashCode.Combine(Suit, Rank);
        }

        public override string ToString()
        {
            return $"({Suit}, {Rank})";
        }
    }

    public enum SuitsEnum
    {
        Swords,
        Clubs,
        Golds,
        Cups
    }

    public enum RanksEnum
    {
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Jack,
        Knight,
        King
    }
}
