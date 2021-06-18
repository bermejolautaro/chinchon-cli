using System;
using System.Collections.Generic;
using System.Linq;
using static Chinchon.Domain.CardsService;

namespace Chinchon.Domain
{
    public static class GameService
    {
        //public static GameState CreateEmptyGameState()
        //{
        //    return new GameState(new Player(0, "Dummy1"), new Player(1, "Dummy2"));
        //}

        public static GameState ShuffleAndDealCards(GameState gameState, Random random)
        {
            var deck = ShuffleCards(random, GetCards());

            var player1Cards = new List<Card>();
            var player2Cards = new List<Card>();

            for (int i = 0; i < 7; i++)
            {
                var response1 = RemoveTopCard(deck);
                deck = response1.Cards;

                if (response1.RemovedCard is null)
                {
                    return gameState;
                }

                player1Cards.Add(response1.RemovedCard);

                var response2 = RemoveTopCard(deck);
                deck = response2.Cards;

                if (response2.RemovedCard is null)
                {
                    return gameState;
                }

                player2Cards.Add(response2.RemovedCard);
            }

            var response3 = RemoveTopCard(deck);
            deck = response3.Cards;

            if (response3.RemovedCard is null)
            {
                return gameState;
            }

            var pile = new List<Card>() { response3.RemovedCard };

            var cloneState = gameState.Clone();

            cloneState.Player1Cards = player1Cards;
            cloneState.Player2Cards = player2Cards;
            cloneState.Deck = deck;
            cloneState.Pile = pile;

            return cloneState;
        }

        public static GameState DeserializeGameState(string serializedGameState)
        {
            GameState gameState = new GameState();
            var properties = typeof(GameState).GetProperties();

            var lines = serializedGameState
                .Split(Environment.NewLine)
                .Where(x => !string.IsNullOrEmpty(x));

            foreach (var line in lines)
            {
                if (line == null)
                {
                    break;
                }

                var splittedLine = line.Split("=");
                var key = splittedLine[0];
                var value = splittedLine[1];

                foreach (var property in properties)
                {
                    if (property.Name != key)
                    {
                        continue;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(string)))
                    {
                        property.SetValue(gameState, value);
                        break;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(bool)))
                    {
                        property.SetValue(gameState, bool.Parse(value));
                        break;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(int)))
                    {
                        property.SetValue(gameState, int.Parse(value));
                        break;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(IEnumerable<Card>)))
                    {
                        var listValue = string.IsNullOrEmpty(value)
                                ? Enumerable.Empty<Card>()
                                : value.Split(";").Select(x => DeserializeCard(x));

                        property.SetValue(gameState, listValue);
                        break;
                    }
                }
            }

            return gameState;
        }
    }
}
