using Chinchon.Domain2;
using System;
using System.Linq;

namespace Chinchon2
{
    public class CutHandler : IGameHandler
    {
        public void Handle(string[] args, GameState gameState)
        {
            var currentPlayerCards = GameService.GetCurrentPlayerCards(gameState);
            if (currentPlayerCards.Count() == 7)
            {
                HandlePickedState(args, gameState);
                return;
            }
            else if (currentPlayerCards.Count() == 8)
            {
                HandleCutedState(args, gameState);
                return;
            }
            else
            {
                Console.WriteLine("Invalid state");
                return;
            }
        }

        private void HandleCutedState(string[] args, GameState gameState)
        {
            if (args.Length == 3)
            {
                // Straight - Chinchon
                var firstGroupIndexes = Enumerable.Empty<int>();

                if (!string.IsNullOrEmpty(args[1]))
                {
                    firstGroupIndexes = args[1].Split(";").Select(x => int.Parse(x));
                }

                if (firstGroupIndexes.Any(x => x < 1 || x > 8))
                {
                    Console.WriteLine("Invalid indexes");
                    return;
                }

                if (firstGroupIndexes.Count() != firstGroupIndexes.Distinct().Count())
                {
                    Console.WriteLine("Invalid input");
                    return;
                }

                if (!(firstGroupIndexes.Count() == 6 || firstGroupIndexes.Count() == 7))
                {
                    Console.WriteLine("Invalid group size");
                    return;
                }

                var cards = GameService.GetCurrentPlayerCards(gameState);
                var cardToCutWith = cards.ToList()[int.Parse(args[2]) - 1];
                var cardsWithoutCardUsedToCut = cards.Where(c => c != cardToCutWith);

                var firstGroup = cardsWithoutCardUsedToCut
                    .Select((card, index) => new { Index = index + 1, Card = card })
                    .Where(x => firstGroupIndexes.Contains(x.Index))
                    .Select(x => x.Card);


                if (!Group.IsValidStraight(firstGroup))
                {
                    Console.WriteLine("Invalid Group");
                    return;
                }

                if(Group.IsChinchon(firstGroup))
                {
                    gameState.Winner = GameService.GetCurrentPlayerName(gameState);
                }

                var card = cards.FirstOrDefault(c => !firstGroup.Contains(c));

                if(int.Parse(card?.Rank ?? "0") > 5)
                {
                    Console.WriteLine("Unlinked card must be less than 5");
                    return;
                }

                var unlinkedCardSum = cardsWithoutCardUsedToCut
                        .Where(c => !firstGroup.Contains(c))
                        .Select(x => int.Parse(x.Rank))
                        .Sum();

                var totalPoints =
                    GameService.GetCurrentPlayerPoints(gameState) +
                    unlinkedCardSum;

                gameState = GameService.SetCurrentPlayerPoints(gameState, totalPoints);

                GameState newGameState = null;
                if (gameState.PlayerTurn < gameState.PlayerAmount)
                {
                    gameState.TurnState = TurnStateEnum.Cuted;
                    gameState.Turn++;
                    gameState.PlayerTurn = ((gameState.Turn - 1) % gameState.PlayerAmount) + 1;
                    newGameState = gameState.Clone();
                }
                else
                {
                    gameState.TurnState = TurnStateEnum.Initial;
                    gameState.Turn = 1;
                    gameState.Hand++;
                    gameState.PlayerTurn = ((gameState.Hand - 1) % gameState.PlayerAmount) + 1;
                    newGameState = GameService.ShuffleAndDealCards(gameState);
                }

                IOGameService.SaveGameState(IOGameService.path, newGameState);
            }
            else if (args.Length == 4)
            {
                if (gameState.TurnState != TurnStateEnum.Picked)
                {
                    Console.WriteLine("Only can cut on Picked state");
                    return;
                }

                var firstGroupIndexes = Enumerable.Empty<int>();
                var secondGroupIndexes = Enumerable.Empty<int>();

                if (!string.IsNullOrEmpty(args[1]))
                {
                    firstGroupIndexes = args[1].Split(";").Select(x => int.Parse(x));
                }

                if (!string.IsNullOrEmpty(args[2]))
                {
                    secondGroupIndexes = args[2].Split(";").Select(x => int.Parse(x));
                }

                if (firstGroupIndexes.Any(x => x < 1 || x > 8) || secondGroupIndexes.Any(x => x < 1 || x > 8))
                {
                    Console.WriteLine("Invalid indexes");
                    return;
                }

                if (firstGroupIndexes.Concat(secondGroupIndexes).Count() != firstGroupIndexes.Concat(secondGroupIndexes).Distinct().Count())
                {
                    Console.WriteLine("Invalid input");
                    return;
                }

                if (!(firstGroupIndexes.Count() == 4 && secondGroupIndexes.Count() == 4 ||
                     firstGroupIndexes.Count() == 4 && secondGroupIndexes.Count() == 3 ||
                     firstGroupIndexes.Count() == 3 && secondGroupIndexes.Count() == 4 ||
                     firstGroupIndexes.Count() == 3 && secondGroupIndexes.Count() == 3))
                {
                    Console.WriteLine("Invalid group size");
                    return;
                }

                var cards = GameService.GetCurrentPlayerCards(gameState);

                var firstGroup = cards
                    .Select((card, index) => new { Index = index + 1, Card = card })
                    .Where(x => firstGroupIndexes.Contains(x.Index))
                    .Select(x => x.Card);

                var secondGroup = cards
                    .Select((card, index) => new { Index = index + 1, Card = card })
                    .Where(x => secondGroupIndexes.Contains(x.Index))
                    .Select(x => x.Card);

                if (!Group.IsValidGroup(firstGroup) || !Group.IsValidGroup(secondGroup))
                {
                    Console.WriteLine("Invalid Group");
                    return;
                }

                var cardToCutWith = cards.ToList()[int.Parse(args[3]) - 1];

                var card = cards.FirstOrDefault(c => !firstGroup.Concat(secondGroup).Concat(new[] { cardToCutWith }).Contains(c));

                if (int.Parse(card.Rank) > 5)
                {
                    Console.WriteLine("Unlinked card must be lesser than 5");
                    return;
                }

                var unlinkedCardSum = cards
                        .Where(c => !firstGroup.Concat(secondGroup).Concat(new[] { cardToCutWith }).Contains(c))
                        .Select(x => int.Parse(x.Rank))
                        .Sum();

                var totalPoints =
                    GameService.GetCurrentPlayerPoints(gameState) +
                    unlinkedCardSum;

                gameState = GameService.SetCurrentPlayerPoints(gameState, totalPoints);

                GameState newGameState = null;
                if (gameState.PlayerTurn < gameState.PlayerAmount)
                {
                    gameState.TurnState = TurnStateEnum.Cuted;
                    gameState.Turn++;
                    gameState.PlayerTurn = ((gameState.Turn - 1) % gameState.PlayerAmount) + 1;
                    newGameState = gameState.Clone();
                }
                else
                {
                    gameState.TurnState = TurnStateEnum.Initial;
                    gameState.Turn = 1;
                    gameState.Hand++;
                    gameState.PlayerTurn = ((gameState.Hand - 1) % gameState.PlayerAmount) + 1;
                    newGameState = GameService.ShuffleAndDealCards(gameState);
                }

                IOGameService.SaveGameState(IOGameService.path, newGameState);
            }
        }

        public static void HandlePickedState(string[] args, GameState gameState)
        {
            if (args.Length == 1)
            {
                var cards = GameService.GetCurrentPlayerCards(gameState);

                var unlinkedCardSum = cards
                        .Select(x => int.Parse(x.Rank))
                        .Sum();

                var totalPoints =
                    GameService.GetCurrentPlayerPoints(gameState) +
                    unlinkedCardSum;

                gameState = GameService.SetCurrentPlayerPoints(gameState, totalPoints);

                GameState newGameState = null;

                if (gameState.PlayerTurn < gameState.PlayerAmount)
                {
                    gameState.TurnState = TurnStateEnum.Cuted;
                    gameState.Turn++;
                    gameState.PlayerTurn = ((gameState.Turn - 1) % gameState.PlayerAmount) + 1;
                    newGameState = gameState.Clone();
                }
                else
                {
                    gameState.TurnState = TurnStateEnum.Initial;
                    gameState.Turn = 1;
                    gameState.Hand++;
                    gameState.PlayerTurn = ((gameState.Hand - 1) % gameState.PlayerAmount) + 1;
                    newGameState = GameService.ShuffleAndDealCards(gameState);
                }

                IOGameService.SaveGameState(IOGameService.path, newGameState);

                return;
            }
            else if (args.Length == 2)
            {
                var firstGroupIndexes = Enumerable.Empty<int>();

                if (!string.IsNullOrEmpty(args[1]))
                {
                    firstGroupIndexes = args[1].Split(";").Select(x => int.Parse(x));
                }

                if (firstGroupIndexes.Any(x => x < 1 || x > 8))
                {
                    Console.WriteLine("Invalid indexes");
                    return;
                }

                if (firstGroupIndexes.Count() != firstGroupIndexes.Distinct().Count())
                {
                    Console.WriteLine("Invalid input");
                    return;
                }

                if (!(firstGroupIndexes.Count() == 3 ||
                     firstGroupIndexes.Count() == 4 ||
                     firstGroupIndexes.Count() == 5 ||
                     firstGroupIndexes.Count() == 6))
                {
                    Console.WriteLine("Invalid group size");
                    return;
                }

                var cards = GameService.GetCurrentPlayerCards(gameState);

                var firstGroup = cards
                    .Select((card, index) => new { Index = index + 1, Card = card })
                    .Where(x => firstGroupIndexes.Contains(x.Index))
                    .Select(x => x.Card);

                if (!Group.IsValidGroup(firstGroup))
                {
                    Console.WriteLine("Invalid Group");
                    return;
                }

                var unlinkedCardSum = cards
                        .Where(c => !firstGroup.Contains(c))
                        .Select(x => int.Parse(x.Rank))
                        .Sum();

                var totalPoints =
                    GameService.GetCurrentPlayerPoints(gameState) +
                    unlinkedCardSum;

                gameState = GameService.SetCurrentPlayerPoints(gameState, totalPoints);

                GameState newGameState = null;
                if (gameState.PlayerTurn < gameState.PlayerAmount)
                {
                    gameState.TurnState = TurnStateEnum.Cuted;
                    gameState.Turn++;
                    gameState.PlayerTurn = ((gameState.Turn - 1) % gameState.PlayerAmount) + 1;
                    newGameState = gameState.Clone();
                }
                else
                {
                    gameState.TurnState = TurnStateEnum.Initial;
                    gameState.Turn = 1;
                    gameState.Hand++;
                    gameState.PlayerTurn = ((gameState.Hand - 1) % gameState.PlayerAmount) + 1;
                    newGameState = GameService.ShuffleAndDealCards(gameState);
                }

                IOGameService.SaveGameState(IOGameService.path, newGameState);
            }
            else if (args.Length == 3)
            {
                var firstGroupIndexes = Enumerable.Empty<int>();
                var secondGroupIndexes = Enumerable.Empty<int>();

                if (!string.IsNullOrEmpty(args[1]))
                {
                    firstGroupIndexes = args[1].Split(";").Select(x => int.Parse(x));
                }

                if (!string.IsNullOrEmpty(args[2]))
                {
                    secondGroupIndexes = args[2].Split(";").Select(x => int.Parse(x));
                }

                if (firstGroupIndexes.Any(x => x < 1 || x > 8) || secondGroupIndexes.Any(x => x < 1 || x > 8))
                {
                    Console.WriteLine("Invalid indexes");
                    return;
                }

                if (firstGroupIndexes.Concat(secondGroupIndexes).Count() != firstGroupIndexes.Concat(secondGroupIndexes).Distinct().Count())
                {
                    Console.WriteLine("Invalid input");
                    return;
                }

                if (!(firstGroupIndexes.Count() == 4 && secondGroupIndexes.Count() == 4 ||
                     firstGroupIndexes.Count() == 4 && secondGroupIndexes.Count() == 3 ||
                     firstGroupIndexes.Count() == 3 && secondGroupIndexes.Count() == 4 ||
                     firstGroupIndexes.Count() == 3 && secondGroupIndexes.Count() == 3))
                {
                    Console.WriteLine("Invalid group size");
                    return;
                }

                var cards = GameService.GetCurrentPlayerCards(gameState);

                var firstGroup = cards
                    .Select((card, index) => new { Index = index + 1, Card = card })
                    .Where(x => firstGroupIndexes.Contains(x.Index))
                    .Select(x => x.Card);

                var secondGroup = cards
                    .Select((card, index) => new { Index = index + 1, Card = card })
                    .Where(x => secondGroupIndexes.Contains(x.Index))
                    .Select(x => x.Card);

                if (!Group.IsValidGroup(firstGroup) || !Group.IsValidGroup(secondGroup))
                {
                    Console.WriteLine("Invalid Group");
                    return;
                }

                var card = cards.FirstOrDefault(c => !firstGroup.Concat(secondGroup).Contains(c));


                var unlinkedCardSum = cards
                        .Where(c => !firstGroup.Concat(secondGroup).Contains(c))
                        .Select(x => int.Parse(x.Rank))
                        .Sum();

                var totalPoints =
                    GameService.GetCurrentPlayerPoints(gameState) +
                    unlinkedCardSum;

                gameState = GameService.SetCurrentPlayerPoints(gameState, totalPoints);

                GameState newGameState = null;
                if (gameState.PlayerTurn < gameState.PlayerAmount)
                {
                    gameState.TurnState = TurnStateEnum.Cuted;
                    gameState.Turn++;
                    gameState.PlayerTurn = ((gameState.Turn - 1) % gameState.PlayerAmount) + 1;
                    newGameState = gameState.Clone();
                }
                else
                {
                    gameState.TurnState = TurnStateEnum.Initial;
                    gameState.Turn = 1;
                    gameState.Hand++;
                    gameState.PlayerTurn = ((gameState.Hand - 1) % gameState.PlayerAmount) + 1;
                    newGameState = GameService.ShuffleAndDealCards(gameState);
                }

                IOGameService.SaveGameState(IOGameService.path, newGameState);
            }
        }
    }
}
