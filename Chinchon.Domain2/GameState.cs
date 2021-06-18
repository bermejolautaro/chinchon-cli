using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Chinchon.Domain
{
    public class GameState : IEquatable<GameState>
    {
        public bool GameOver { get; private set; }
        public bool WasCut { get; private set; }
        public int RemainingPlayersToCut { get; private set; } = -1;
        public int Hand { get; set; } = 1;
        public int Turn { get; set; } = 1;
        public int PlayerAmount { get; set; } = 2;
        public int PlayerTurn { get; set; } = 1;
        public IEnumerable<Card> Player1Cards { get; set; } = Enumerable.Empty<Card>();
        public int Player1Points { get; set; } = 0;
        public IEnumerable<Card> Player2Cards { get; set; } = Enumerable.Empty<Card>();
        public int Player2Points { get; set; } = 0;
        public IEnumerable<Card> Deck { get; set; } = Enumerable.Empty<Card>();
        public IEnumerable<Card> Pile { get; set; } = Enumerable.Empty<Card>();
        public Player? Player1 { get; set; }
        public Player? Player2 { get; set; }

        //public GameState(Player player1, Player player2)
        //{
        //    Player1 = player1;
        //    Player2 = player2;
        //}

        public GameState WithGameOver(bool gameOver)
        {
            var newState = this.Clone();
            newState.GameOver = gameOver;
            return newState;
        }

        public GameState WithWasCut(bool wasCut)
        {
            var newState = this.Clone();
            newState.WasCut = wasCut;
            return newState;
        }

        public GameState WithRemainingPlayerToCut(int remainingPlayersToCut)
        {
            var newState = this.Clone();
            newState.RemainingPlayersToCut = remainingPlayersToCut;
            return newState;
        }

        public override string? ToString()
        {
            return this.SerializeGameState();
        }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj is GameState)
            {
                return false;
            }

            return Equals(obj);
        }

        public bool Equals(GameState other)
        {
            return
                other != null &&
                Hand == other.Hand &&
                Turn == other.Turn &&
                PlayerAmount == other.PlayerAmount &&
                PlayerTurn == other.PlayerTurn &&
                Player1Cards.SequenceEqual(other.Player1Cards) &&
                Player1Points == other.Player1Points &&
                Player2Cards.SequenceEqual(other.Player2Cards) &&
                Player2Points == other.Player2Points &&
                Deck.SequenceEqual(other.Deck) &&
                Pile.SequenceEqual(other.Pile);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();
            hashCode.Add(Hand);
            hashCode.Add(Turn);
            hashCode.Add(PlayerAmount);
            hashCode.Add(PlayerTurn);
            hashCode.Add(Player1Cards);
            hashCode.Add(Player1Points);
            hashCode.Add(Player2Cards);
            hashCode.Add(Player2Points);
            hashCode.Add(Deck);
            hashCode.Add(Pile);

            return hashCode.ToHashCode();
        }

    }

    public static class GameStateExtensions
    {
        public static GameState Clone(this GameState gameState)
        {
            //GameState newGameState = new GameState(gameState.Player1, gameState.Player2);
            GameState newGameState = new GameState();


            PropertyInfo[] properties = typeof(GameState).GetProperties();

            foreach (var property in properties)
            {
                var propertyInfo = typeof(GameState).GetProperty(property.Name);

                if (propertyInfo == null)
                {
                    continue;
                }

                property.SetValue(newGameState, propertyInfo.GetValue(gameState));
            }

            return newGameState;
        }

        public static IEnumerable<Card> GetCurrentPlayerCards(this GameState state)
        {
            return state.PlayerTurn == 1
                    ? state.Player1Cards
                    : state.Player2Cards;
        }

        internal static GameState SetCurrentPlayerCards(this GameState state, IEnumerable<Card> cards)
        {
            var cloneState = state.Clone();

            if (state.PlayerTurn == 1)
            {
                cloneState.Player1Cards = cards;
            }
            else
            {
                cloneState.Player2Cards = cards;
            }

            return cloneState;
        }

        public static int GetCurrentPlayerPoints(this GameState state)
        {
            return state.PlayerTurn == 1
                    ? state.Player1Points
                    : state.Player2Points;
        }

        internal static GameState SetCurrentPlayerPoints(this GameState state, int points)
        {
            var cloneState = state.Clone();

            if (state.PlayerTurn == 1)
            {
                cloneState.Player1Points = points;
            }
            else
            {
                cloneState.Player2Points = points;
            }

            return cloneState;
        }

        public static string GetCurrentPlayerName(this GameState state)
        {
            return state.PlayerTurn == 1
                    ? "Player 1"
                    : "Player 2";
        }

        public static string SerializeGameState(this GameState state)
        {
            var properties = typeof(GameState).GetProperties();

            var propertiesToString = properties.Select(property =>
            {
                if (property.PropertyType.IsAssignableFrom(typeof(IEnumerable<Card>)))
                {
                    var cards = (IEnumerable<Card>?)property.GetValue(state);
                    return $"{property.Name}={string.Join(";", cards.Select(c => c.ToString()))}";
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
