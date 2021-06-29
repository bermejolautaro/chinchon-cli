using System;
using System.Collections.Generic;
using System.Linq;

namespace Chinchon.Domain
{
    public class GameStateOptions
    {
        public bool? GameOver { get; set; }
        public bool? WasCut { get; set; }
        public int? RemainingPlayersToCut { get; set; }
        public int? Hand { get; set; }
        public int? Turn { get; set; }
        public int? PlayerTurn { get; set; }
        public IEnumerable<Card>? Deck { get; set; }
        public IEnumerable<Card>? Pile { get; set; }
        public Player? Player1 { get; set; } 
        public Player? Player2 { get; set; }
        public Player? Player3 { get; set; }
        public Player? Player4 { get; set; }
    }

    public class GameState : IEquatable<GameState>
    {
        public bool GameOver { get; }
        public bool WasCut { get; }
        public int RemainingPlayersToCut { get; } = -1;
        public int Hand { get; } = 1;
        public int Turn { get; } = 1;
        public int PlayerTurn { get; } = 1;
        public IEnumerable<Card> Deck { get; } = Enumerable.Empty<Card>();
        public IEnumerable<Card> Pile { get; } = Enumerable.Empty<Card>();
        public Player Player1 { get; }
        public Player Player2 { get; }
        public Player? Player3 { get; }
        public Player? Player4 { get; }


        public GameState(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
        }

        public GameState(Player player1, Player player2, Player player3) : this(player1, player2)
        {
            Player1 = player1;
            Player2 = player2;
            Player3 = player3;
        }

        public GameState(Player player1, Player player2, Player player3, Player player4) : this(player1, player2, player3)
        {
            Player1 = player1;
            Player2 = player2;
            Player3 = player3;
            Player4 = player4;
        }

        public GameState(GameStateOptions options)
        {
            GameOver = options.GameOver ?? false;
            WasCut = options.WasCut ?? false;
            RemainingPlayersToCut = options.RemainingPlayersToCut ?? -1;
            Hand = options.Hand ?? 1;
            Turn = options.Turn ?? 1;
            PlayerTurn = options.PlayerTurn ?? 1;
            Deck = options.Deck ?? Enumerable.Empty<Card>();
            Pile = options.Pile ?? Enumerable.Empty<Card>();
            Player1 = options.Player1 ?? new Player(1);
            Player2 = options.Player2 ?? new Player(2);
            Player3 = options.Player3;
            Player4 = options.Player4;
        }

        internal GameState(GameState state, GameStateOptions options)
        {
            GameOver = options.GameOver ?? state.GameOver;
            WasCut = options.WasCut ?? state.WasCut;
            RemainingPlayersToCut = options.RemainingPlayersToCut ?? state.RemainingPlayersToCut;
            Hand = options.Hand ?? state.Hand;
            Turn = options.Turn ?? state.Turn;
            PlayerTurn = options.PlayerTurn ?? state.PlayerTurn;
            Deck = options.Deck ?? state.Deck;
            Pile = options.Pile ?? state.Pile;
            Player1 = options.Player1 ?? state.Player1;
            Player2 = options.Player2 ?? state.Player2;
            Player3 = options.Player3 ?? state.Player3;
            Player4 = options.Player4 ?? state.Player4;
        }

        public override string? ToString()
        {
            return this.SerializeGameState();
        }

        public override bool Equals(object? obj)
        {
            return obj is null || !(obj is GameState gameState) ? false : Equals(gameState);
        }

        public bool Equals(GameState other)
        {
            return
                other != null &&
                Hand == other.Hand &&
                Turn == other.Turn &&
                PlayerTurn == other.PlayerTurn &&
                Player1.Equals(other.Player1) &&
                Player2.Equals(other.Player2) &&
                Deck.SequenceEqual(other.Deck) &&
                Pile.SequenceEqual(other.Pile);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Hand);
            hashCode.Add(Turn);
            hashCode.Add(PlayerTurn);
            hashCode.Add(Player1);
            hashCode.Add(Player2);
            hashCode.Add(Player3);
            hashCode.Add(Player4);
            hashCode.Add(Deck);
            hashCode.Add(Pile);

            return hashCode.ToHashCode();
        }

    }

    public static class GameStateExtensions
    {
        public static GameState With(this GameState gameState, Action<GameStateOptions> builder)
        {
            var options = new GameStateOptions();
            builder(options);

            return new GameState(gameState, options);
        }

        public static IEnumerable<Player> GetPlayers(this GameState state)
        {
            return new[] { state.Player1, state.Player2, state.Player3!, state.Player4! }.Where(x => !(x is null));
        }

        public static Player GetCurrentPlayer(this GameState state)
        {
            return state.PlayerTurn switch
            {
                1 => state.Player1!,
                2 => state.Player2!,
                3 => state.Player3!,
                4 => state.Player4!
            };
        }

        public static Player GetPlayerById(this GameState state, int playerId)
        {
            return playerId switch
            {
                1 => state.Player1!,
                2 => state.Player2!,
                3 => state.Player3!,
                4 => state.Player4!
            };
        }

        internal static GameState SetCurrentPlayerCards(this GameState state, IEnumerable<Card> cards)
        {
            return state.PlayerTurn switch
            {
                1 => state.With(stateOptions =>
                {
                    stateOptions.Player1 = state.Player1.With(options => options.Cards = cards);
                }),
                2 => state.With(stateOptions =>
                {
                    stateOptions.Player2 = state.Player2.With(options => options.Cards = cards);
                }),
                3 => state.With(stateOptions =>
                {
                    stateOptions.Player3 = state.Player3!.With(options => options.Cards = cards);
                }),
                4 => state.With(stateOptions =>
                {
                    stateOptions.Player4 = state.Player4!.With(options => options.Cards = cards);
                }),
            };
        }

        internal static GameState SetCurrentPlayerPoints(this GameState state, int points)
        {
            return state.PlayerTurn switch
            {
                1 => state.With(stateOptions =>
                {
                    stateOptions.Player1 = state.Player1.With(options => options.Points = points);
                }),
                2 => state.With(stateOptions =>
                {
                    stateOptions.Player2 = state.Player2.With(options => options.Points = points);
                }),
                3 => state.With(stateOptions =>
                {
                    stateOptions.Player3 = state.Player3!.With(options => options.Points = points);
                }),
                4 => state.With(stateOptions =>
                {
                    stateOptions.Player4 = state.Player4!.With(options => options.Points = points);
                }),
            };
        }


        public static GameState ShuffleAndDealCards(this GameState state, Random random)
        {
            var deck = CardsService.ShuffleCards(random, CardsService.GetCards());

            var players = new[] { state.Player1, state.Player2, state.Player3, state.Player4 }.Where(x => !(x is null));
            var playersCards = new List<List<Card>>()
            {
                new List<Card>(),
                new List<Card>(),
                new List<Card>(),
                new List<Card>()
            };

            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < players.Count(); j++)
                {
                    var response = CardsService.RemoveTopCard(deck);
                    deck = response.Cards;

                    if (response.RemovedCard is null)
                    {
                        return state;
                    }

                    playersCards[j].Add(response.RemovedCard);
                }
            }

            var (newDeck, removedCard, _) = CardsService.RemoveTopCard(deck);

            if (removedCard is null)
            {
                return state;
            }

            return state.With(stateOptions =>
            {
                stateOptions.Player1 = state.Player1.With(options => options.Cards = playersCards[0]);
                stateOptions.Player2 = state.Player2.With(options => options.Cards = playersCards[1]);
                stateOptions.Player3 = state.Player3?.With(options => options.Cards = playersCards[2]);
                stateOptions.Player4 = state.Player4?.With(options => options.Cards = playersCards[3]);
                stateOptions.Deck = newDeck;
                stateOptions.Pile = new[] { removedCard };
            });
        }

        public static string SerializeGameState(this GameState state)
        {
            var properties = typeof(GameState).GetProperties();

            var propertiesToString = properties.Select(property =>
            {
                if (property.PropertyType.IsAssignableFrom(typeof(IEnumerable<Card>)))
                {
                    var cards = (IEnumerable<Card>?)property.GetValue(state);

                    return $"{property.Name}={cards!.Serialize()}";
                }
                else
                {
                    return $"{property.Name}={property.GetValue(state)}";
                }
            });

            return $"{string.Join(Environment.NewLine, propertiesToString)}";
        }
    }
}
