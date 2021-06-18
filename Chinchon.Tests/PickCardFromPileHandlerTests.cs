using Chinchon.Domain;
using Chinchon.GameHandlers;
using Shouldly;
using System.Linq;
using Xunit;

namespace Chinchon.Tests
{
    public class PickCardFromPileHandlerTests
    {
        [Fact(DisplayName = "Given empty deck When running pick card from pile command then show expected error message")]
        public void Given_EmptyPile_When_RunningPickCardFromDeckCommand_Then_ShowExpectedErrorMessage()
        {
            var expectedOutput = "Pile is empty";
            var gameState = new GameState()
            {
                PlayerTurn = 1,
                Player1Cards = CardsService.GetCards().Take(7)
            };
            var pickCardFromDeckHandler = new PickCardFromPileHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromPile" }, gameState);

            (response.Action as WriteAction).Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given picked turn state When running pick card from pile command then show expected error message")]
        public void Given_PickedTurnState_When_RunningPickCardFromPileCommand_Then_ShowExpectedErrorMessage()
        {
            var expectedOutput = "Only can pick cards when having 7 cards";
            var gameState = new GameState()
            {
                Deck = CardsService.GetCards()
            };
            var pickCardFromDeckHandler = new PickCardFromPileHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromPile" }, gameState);

            (response.Action as WriteAction).Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given initial turn state When running pick card from pile command then show expected error message")]
        public void Given_InitialTurnState_When_RunningPickCardFromPileCommand_Then_ShowExpectedErrorMessage()
        {
            var cards = new[] { new Card(SuitsEnum.Golds, RanksEnum.One) };
            var removeResponse = CardsService.RemoveTopCard(cards);

            var expectedOutput = new GameState()
            {
                Hand = 1,
                Turn = 1,
                PlayerAmount = 2,
                PlayerTurn = 1,
                Player1Cards = CardsService.GetCards().Take(7).Append(removeResponse.RemovedCard),
                Player1Points = 0,
                Player2Cards = Enumerable.Empty<Card>(),
                Player2Points = 0,
                Deck = Enumerable.Empty<Card>(),
                Pile = removeResponse.Cards
            };

            var gameState = new GameState()
            {
                Player1Cards = CardsService.GetCards().Take(7),
                Pile = cards
            };
            var pickCardFromDeckHandler = new PickCardFromPileHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromPile" }, gameState);

            var saveAction = (response.Action as SaveAction);
            saveAction.GameState.ShouldBe(expectedOutput);
        }
    }
}
