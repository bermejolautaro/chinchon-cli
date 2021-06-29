using System;
using System.Collections.Generic;
using System.Linq;

namespace Chinchon.Domain.Modules
{
    static class CutModule
    {
        internal static IResult Cut(GameState gameState, Random random, IEnumerable<Group> groups, Card? cardToCutWith = null)
        {
            if (gameState.Turn <= gameState.GetPlayers().Count())
            {
                return new ErrorResult("Can't cut before the second round");
            }

            if (!PlayerHasTheseCards(gameState.GetCurrentPlayer().Cards, groups, cardToCutWith) )
            {
                return new ErrorResult("Can't cut with cards that you don't have in your hand");
            }

            if (!AreAllDistinct(groups, cardToCutWith))
            {
                return new ErrorResult("Duplicated cards");
            }

            var cards = gameState.GetCurrentPlayer().Cards;

            return (cards.Count()) switch
            {
                7 => CutWith7Cards(gameState, random, groups),
                8 when !(cardToCutWith is null) => CutWith8Cards(gameState, random, groups, cardToCutWith),
                _ => new ErrorResult("Invalid state")
            };
        }

        private static bool PlayerHasTheseCards(IEnumerable<Card> cards, IEnumerable<Group> groups, Card? cardToCutWith)
        {
            var allCards = groups.SelectMany(x => x).Concat(new[] { cardToCutWith }.Where(x => !(x is null))).Cast<Card>();

            return new HashSet<Card>(allCards).IsSubsetOf(new HashSet<Card>(cards.ToList()));
        }

        private static bool AreAllDistinct(IEnumerable<Group> groups, Card? cardToCutWith)
        {
            var allCards = groups.SelectMany(x => x).Concat(new[] { cardToCutWith }.Where(x => !(x is null)));

            return allCards.Count() == allCards.Distinct().Count();
        }

        private static IResult CutWith7Cards(GameState gameState, Random random, IEnumerable<Group> groups)
        {
            if (!gameState.WasCut)
            {
                return new ErrorResult("Can't cut with 7 cards before anyone cut with 8 cards");
            }

            return groups.Count() switch
            {
                0 => CutWith7Cards(gameState, random),
                1 => CutWith7Cards(gameState, random, groups.First()),
                2 => CutWith7Cards(gameState, random, groups.ElementAt(0), groups.ElementAt(1)),
                _ => new ErrorResult("Invalid amount of groups")
            };
        }

        private static IResult CutWith7Cards(GameState gameState, Random random)
        {
            var totalPoints =
                gameState.GetCurrentPlayer().Points +
                gameState.GetCurrentPlayer().Cards.Select(x => x.RankValue).Sum();

            var newGameState = gameState.SetCurrentPlayerPoints(totalPoints);

            return new SuccessResult(EndCut(newGameState, random));
        }

        private static IResult CutWith7Cards(GameState gameState, Random random, Group group)
        {
            if (!(group.Count() >= 3 && group.Count() <= 6))
            {
                return new ErrorResult("Invalid group size");
            }

            return CutWith7CardsHelp(gameState, random, group);
        }

        private static IResult CutWith7Cards(GameState gameState, Random random, Group firstGroup, Group secondGroup)
        {
            var validPairs = new[] { (4, 4), (4, 3), (3, 4), (3, 3) };
            var pair = (firstGroup.Count(), secondGroup.Count());

            if (!validPairs.Contains(pair))
            {
                return new ErrorResult("Invalid group size");
            }

            return CutWith7CardsHelp(gameState, random, firstGroup.Concat(secondGroup));
        }

        private static IResult CutWith7CardsHelp(GameState gameState, Random random, IEnumerable<Card> linkedCards)
        {
            var cards = gameState.GetCurrentPlayer().Cards;

            var card = cards.FirstOrDefault(c => !linkedCards.Contains(c));

            var totalPoints =
                gameState.GetCurrentPlayer().Points +
                cards.Where(c => !linkedCards.Contains(c)).Select(x => x.RankValue).Sum();

            gameState = gameState.SetCurrentPlayerPoints(totalPoints);

            return new SuccessResult(EndCut(gameState, random));
        }

        private static IResult CutWith8Cards(GameState gameState, Random random, IEnumerable<Group> groups, Card cardToCutWith)
        {
            return groups.Count() switch
            {
                1 => CutWith8Cards(gameState, random, groups.First(), cardToCutWith),
                2 => CutWith8Cards(gameState, random, groups.ElementAt(0), groups.ElementAt(1), cardToCutWith),
                _ => new ErrorResult("Invalid amount of groups")
            };
        }

        private static IResult CutWith8Cards(GameState gameState, Random random, Group group, Card cardToCutWith)
        {
            if (!(group.Count() == 6 || group.Count() == 7))
            {
                return new ErrorResult("Invalid group size");
            }

            if (group.IsChinchon())
            {
                gameState = gameState.With(options =>
                {
                    options.WasCut = true;
                });
            }

            return CutWith8CardsHelp(gameState, random, cardToCutWith, group);
        }

        private static IResult CutWith8Cards(GameState gameState, Random random, Group firstGroup, Group secondGroup, Card cardToCutWith)
        {
            var validPairs = new[] { (4, 4), (4, 3), (3, 4), (3, 3) };
            var pair = (firstGroup.Count(), secondGroup.Count());

            if (!validPairs.Contains(pair))
            {
                return new ErrorResult("Invalid group size");
            }

            return CutWith8CardsHelp(gameState, random, cardToCutWith, firstGroup.Concat(secondGroup));
        }

        private static IResult CutWith8CardsHelp(GameState gameState, Random random, Card cardToCutWith, IEnumerable<Card> linkedCards)
        {
            var cards = gameState.GetCurrentPlayer().Cards;

            var unlinkedCardValue = cards
                .Where(c => !linkedCards.Append(cardToCutWith).Contains(c))
                .Select(x => x.RankValue)
                .DefaultIfEmpty()
                .First();

            // TODO: Hacer el 5 parametrizable
            if (unlinkedCardValue > 5)
            {
                return new ErrorResult("Unlinked card must be less than 5");
            }

            var totalPoints =
                gameState.GetCurrentPlayer().Points + unlinkedCardValue;

            gameState = gameState.SetCurrentPlayerPoints(totalPoints);

            return new SuccessResult(EndCut(gameState, random));
        }

        private static GameState EndCut(GameState gameState, Random random)
        {
            // TODO: Hacer el 100 parametrizable
            if (gameState.GetCurrentPlayer().Points >= 100)
            {
                gameState = gameState.With(options => options.WasCut = true);
            }

            if (gameState.RemainingPlayersToCut <= 0)
            {
                gameState = gameState.With(options =>
                {
                    options.RemainingPlayersToCut = gameState.GetPlayers().Count();
                });
            }

            gameState = gameState.With(options =>
            {
                options.RemainingPlayersToCut = gameState.RemainingPlayersToCut - 1;
            });

            if (gameState.RemainingPlayersToCut > 0)
            {
                gameState = gameState.With(options => options.PlayerTurn = gameState.PlayerTurn % gameState.GetPlayers().Count() + 1);
                gameState = gameState.With(options => options.Turn = gameState.Turn + 1);
                return gameState.With(options => options.WasCut = true);
            }
            else
            {
                gameState = gameState.With(options => options.Turn = 1);
                gameState = gameState.With(options => options.Hand = gameState.Hand + 1);
                gameState = gameState.With(options => options.PlayerTurn = (gameState.Hand - 1) % gameState.GetPlayers().Count() + 1);
                gameState = gameState.With(options => options.WasCut = false);
                return gameState.ShuffleAndDealCards(random);
            }
        }
    }
}
