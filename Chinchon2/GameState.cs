using Chinchon.Domain2;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Chinchon2
{
    public enum TurnStateEnum
    {
        Initial,
        Picked,
        Cuted
    }
    public class GameState
    {
        public string Winner { get; set; }
        public int Hand { get; set; }
        public int Turn { get; set; }
        public TurnStateEnum TurnState { get; set; }
        public int PlayerAmount { get; set; }
        public int PlayerTurn { get; set; }
        public IEnumerable<Card> Player1Cards { get; set; }
        public int Player1Points { get; set; }
        public IEnumerable<Card> Player2Cards { get; set; }
        public int Player2Points { get; set; }
        public IEnumerable<Card> Deck { get; set; }
        public IEnumerable<Card> Pile { get; set; }

    }

    public static class GameStateExtensions
    {
        public static GameState Clone(this GameState gameState)
        {
            GameState newGameState = new GameState();

            PropertyInfo[] properties = typeof(GameState).GetProperties();

            foreach (var property in properties)
            {
                property.SetValue(newGameState, typeof(GameState).GetProperty(property.Name).GetValue(gameState));
            }

            return newGameState;
        }
    }
}
