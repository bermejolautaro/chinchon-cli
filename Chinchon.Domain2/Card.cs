using System;

namespace Chinchon.Domain2
{
    public class Card
    {
        public Card(string suit, string rank)
        {
            Suit = suit;
            Rank = rank;
        }

        public string Suit { get; }
        public string Rank { get; }

        public override bool Equals(object obj)
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
}
