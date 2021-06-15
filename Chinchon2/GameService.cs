using Chinchon.Domain2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Chinchon2.CardsService;

namespace Chinchon2
{
    public static class GameService
    {
        public static IEnumerable<Card> GetCurrentPlayerCards(GameState state)
        {
            return state.PlayerTurn == 1
                    ? state.Player1Cards
                    : state.Player2Cards;
        }

        public static GameState SetCurrentPlayerCards(GameState state, IEnumerable<Card> cards)
        {
            var cloneState = state.Clone();

            if(state.PlayerTurn == 1)
            {
                cloneState.Player1Cards = cards;
            }
            else
            {
                cloneState.Player2Cards = cards;
            }

            return cloneState;
        }

        public static int GetCurrentPlayerPoints(GameState state)
        {
            return state.PlayerTurn == 1
                    ? state.Player1Points
                    : state.Player2Points;
        }

        public static GameState SetCurrentPlayerPoints(GameState state, int points)
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

        public static string GetCurrentPlayerName(GameState state)
        {
            return state.PlayerTurn == 1
                    ? "Player 1"
                    : "Player 2";
        }

        public static GameState ShuffleAndDealCards(GameState gameState)
        {
            var deck = ShuffleCards(GetCards());

            var player1Cards = new List<Card>();
            var player2Cards = new List<Card>();

            for (int i = 0; i < 7; i++)
            {
                var response1 = RemoveTopCard(deck);
                deck = response1.Cards;
                player1Cards.Add(response1.RemovedCard);
                var response2 = RemoveTopCard(deck);
                deck = response2.Cards;
                player2Cards.Add(response2.RemovedCard);
            }

            var response3 = RemoveTopCard(deck);
            deck = response3.Cards;
            var pile = new List<Card>() { response3.RemovedCard };

            var cloneState = gameState.Clone();

            cloneState.Player1Cards = player1Cards;
            cloneState.Player2Cards = player2Cards;
            cloneState.Deck = deck;
            cloneState.Pile = pile;

            return cloneState;
        }

        public static string SerializeGameState(GameState state)
        {
            return
                $"Winner={state.Winner}\n" +
                $"Hand={state.Hand}\n" +
                $"Turn={state.Turn}\n" +
                $"TurnState={state.TurnState.ToString()}\n" +
                $"PlayerAmount={state.PlayerAmount}\n" +
                $"PlayerTurn={state.PlayerTurn}\n" +
                $"Player1Cards={string.Join(";", state.Player1Cards.Select(c => c.ToString()))}\n" +
                $"Player1Points={state.Player1Points}\n" +
                $"Player2Cards={string.Join(";", state.Player2Cards.Select(c => c.ToString()))}\n" +
                $"Player2Points={state.Player2Points}\n" +
                $"Deck={string.Join(";", state.Deck.Select(c => c.ToString()))}\n" +
                $"Pile={string.Join(";", state.Pile.Select(c => c.ToString()))}\n";
        }

        public static GameState DeserializeGameState(StreamReader reader)
        {
            GameState gameState = new GameState();

            while (true)
            {
                string line = reader.ReadLine();

                if (line == null)
                {
                    break;
                }

                var splittedLine = line.Split("=");
                var key = splittedLine[0];
                var value = splittedLine[1];

                switch (key)
                {
                    case nameof(GameState.Winner):
                        {
                            gameState.Winner = value;
                            break;
                        }
                    case nameof(GameState.Hand):
                        {
                            gameState.Hand = int.Parse(value);
                            break;
                        }
                    case nameof(GameState.Turn):
                        {
                            gameState.Turn = int.Parse(value);
                            break;
                        }
                    case nameof(GameState.TurnState):
                        {
                            gameState.TurnState = (TurnStateEnum)Enum.Parse(typeof(TurnStateEnum), value);
                            break;
                        }
                    case nameof(GameState.PlayerAmount):
                        {
                            gameState.PlayerAmount = int.Parse(value);
                            break;
                        }
                    case nameof(GameState.PlayerTurn):
                        {
                            gameState.PlayerTurn = int.Parse(value);
                            break;
                        }
                    case nameof(GameState.Player1Cards):
                        {
                            gameState.Player1Cards = string.IsNullOrEmpty(value)
                                ? new List<Card>()
                                : value.Split(";").Select(x => DeserializeCard(x));
                            break;
                        }
                    case nameof(GameState.Player1Points):
                        {
                            gameState.Player1Points = int.Parse(value);
                            break;
                        }
                    case nameof(GameState.Player2Cards):
                        {
                            gameState.Player2Cards = string.IsNullOrEmpty(value)
                                ? new List<Card>()
                                : value.Split(";").Select(x => DeserializeCard(x));
                            break;
                        }
                    case nameof(GameState.Player2Points):
                        {
                            gameState.Player2Points = int.Parse(value);
                            break;
                        }
                    case nameof(GameState.Deck):
                        {
                            gameState.Deck = string.IsNullOrEmpty(value)
                                ? new List<Card>()
                                : value.Split(";").Select(x => DeserializeCard(x));
                            break;
                        }
                    case nameof(GameState.Pile):
                        {
                            gameState.Pile = string.IsNullOrEmpty(value)
                                ? new List<Card>()
                                : value.Split(";").Select(x => DeserializeCard(x));
                            break;
                        }
                }

            }

            reader.Close();
            return gameState;
        }
    }
}
