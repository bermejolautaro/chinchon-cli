using Chinchon.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Chinchon.GameHandlers
{
    public class CutHandler : IGameHandler
    {
        private readonly Random _random;

        public CutHandler(Random random)
        {
            _random = random;
        }

        public HandlerResponse Handle(string[] args, GameState gameState, ApplicationState appState)
        {
            return Cut(gameState, appState, _random, args.Skip(1));
        }

        public static HandlerResponse Cut(GameState gameState, ApplicationState appState, Random random, IEnumerable<string> parameters)
        {
            if (int.TryParse(parameters.LastOrDefault(), out int _))
            {
                return CutWith8Cards(gameState, appState, random, parameters);
            }
            else
            {
                return CutWith7Cards(gameState, appState, random, parameters);
            }
        }

        public static HandlerResponse CutWith7Cards(GameState gameState, ApplicationState appState, Random random, IEnumerable<string> groupsStrings)
        {
            if (groupsStrings.Any(groupString => !IsValidFormat(groupString)))
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid format") };
            }

            var groupsIndexes = groupsStrings.Select(groupString => groupString.Split(";").Select(x => int.Parse(x)));

            if (groupsIndexes.Select(groupIndexes => groupIndexes.All(y => y >= 1 && y <= 8)).Any(isValidGroupIndexes => !isValidGroupIndexes))
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid indexes") };
            }

            var cards = gameState.GetCurrentPlayer().Cards;

            var groupsCards = groupsIndexes.Select(groupIndexes => IndexesToCards(cards, groupIndexes));

            if (groupsCards.Any(groupCards => !Group.IsValidGroup(groupCards)))
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid Group") };
            }

            var groups = groupsCards.Select(groupCards => Group.CreateGroup(groupCards));

            return Mediator.Send(new CutRequest(gameState, random, groups)) switch
            {
                SuccessResult result => new HandlerResponse() { Action = new SaveStateAction(result.GameState, appState) },
                ErrorResult result => new HandlerResponse() { Action = new WriteAction(result.ErrorMessage) },
            };
        }

        public static HandlerResponse CutWith8Cards(GameState gameState, ApplicationState appState, Random random, IEnumerable<string> groupsStringsAndCardToCutWith)
        {
            var cardToCutWithString = groupsStringsAndCardToCutWith.Last();
            var groupsStrings = groupsStringsAndCardToCutWith.SkipLast(1);

            if (groupsStrings.Append(cardToCutWithString).Any(groupString => !IsValidFormat(groupString)))
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid format") };
            }

            var groupsIndexes = groupsStrings.Select(groupString => groupString.Split(";").Select(x => int.Parse(x)));
            var cardToCutWithIndex = int.Parse(cardToCutWithString);

            if (cardToCutWithIndex < 1 || cardToCutWithIndex > 8)
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid indexes") };
            }

            if (groupsIndexes.Append(new[] { cardToCutWithIndex }).Select(groupIndexes => groupIndexes.All(y => y >= 1 && y <= 8)).Any(isValidGroupIndexes => !isValidGroupIndexes))
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid indexes") };
            }

            var cards = gameState.GetCurrentPlayer().Cards;

            var groupsCards = groupsIndexes.Select(groupIndexes => IndexesToCards(cards, groupIndexes));

            var cardToCutWith = cards.ElementAt(cardToCutWithIndex - 1);

            if (groupsCards.Any(groupCards => !Group.IsValidGroup(groupCards)))
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid Group") };
            }

            var groups = groupsCards.Select(groupCards => Group.CreateGroup(groupCards));

            return Mediator.Send(new CutRequest(gameState, random, groups, cardToCutWith)) switch
            {
                SuccessResult result => new HandlerResponse() { Action = new SaveStateAction(result.GameState, appState) },
                ErrorResult result => new HandlerResponse() { Action = new WriteAction(result.ErrorMessage) },
            };
        }

        private static IEnumerable<Card> IndexesToCards(IEnumerable<Card> cards, IEnumerable<int> indexes)
        {
            return cards
                .Select((card, index) => new { Index = index + 1, Card = card })
                .Where(x => indexes.Contains(x.Index))
                .Select(x => x.Card);
        }

        public static bool IsValidFormat(string groupString)
        {
            return groupString.Split(";").Select(x => int.TryParse(x, out int _)).All(x => x);
        }
    }
}
