using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Chinchon.Domain
{
    public class PlayerOptions
    {
        public int? Id { get; set; }
        public int? Points { get; set; }
        public IEnumerable<Card>? Cards { get; set; }
    }

    public class Player : IEquatable<Player>
    {
        public int Id { get; }
        public int Points { get; }
        public IEnumerable<Card> Cards { get; } = Enumerable.Empty<Card>();

        public Player(int id)
        {
            Id = id;
        }

        public Player(Player player, PlayerOptions options)
        {
            Id = options.Id ?? player.Id;
            Points = options.Points ?? player.Points;
            Cards = options.Cards ?? player.Cards;
        }

        public Player(PlayerOptions options)
        {
            Id = options.Id ?? 0;
            Points = options.Points ?? 0;
            Cards = options.Cards ?? Enumerable.Empty<Card>();
        }

        public override bool Equals(object? obj)
        {
            return obj is null || !(obj is Player player) ? false : Equals(player);
        }

        public bool Equals(Player other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public override string? ToString()
        {
            return 
                $"Id={Id}|" +
                $"Points={Points}|" +
                $"Cards={Cards.Serialize()}";
        }
    }

    public static class PlayerExtensions
    {
        public static Player With(this Player player, Action<PlayerOptions> builder)
        {
            var options = new PlayerOptions();
            builder(options);

            return new Player(player, options);
        }
    }
}
