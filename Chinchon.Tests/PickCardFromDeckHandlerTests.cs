using Chinchon.Domain;
using Chinchon.GameHandlers;
using Shouldly;
using System.Linq;
using Xunit;

namespace Chinchon.Tests
{
    public class PickCardFromDeckHandlerTests
    {
        [Fact(DisplayName = "Given empty deck When running pick card from deck command then show expected error message")]
        public void Given_EmptyDeck_When_RunningPickCardFromDeckCommand_Then_ShowExpectedErrorMessage()
        {
            var expectedOutput = "Deck is empty";
            var gameState = new GameState()
            {
                PlayerTurn = 1,
                Player1Cards = CardsService.GetCards().Take(7)
            };
            var pickCardFromDeckHandler = new PickCardFromDeckHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromDeck" }, gameState);

            (response.Action as WriteAction).Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given picked turn state When running pick card from deck command then show expected error message")]
        public void Given_PickedTurnState_When_RunningPickCardFromDeckCommand_Then_ShowExpectedErrorMessage()
        {
            var expectedOutput = "Only can pick cards when having 7 cards";
            var gameState = new GameState()
            {
                Deck = CardsService.GetCards()
            };
            var pickCardFromDeckHandler = new PickCardFromDeckHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromDeck" }, gameState);

            (response.Action as WriteAction).Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given initial turn state When running pick card from deck command then show expected error message")]
        public void Given_InitialTurnState_When_RunningPickCardFromDeckCommand_Then_ShowExpectedErrorMessage()
        {
            var cards = CardsService.GetCards();
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
                Deck = removeResponse.Cards,
                Pile = Enumerable.Empty<Card>(),
            };

            var gameState = new GameState()
            {
                PlayerTurn = 1,
                Player1Cards = CardsService.GetCards().Take(7),
                Deck = CardsService.GetCards()
            };
            var pickCardFromDeckHandler = new PickCardFromDeckHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromDeck" }, gameState);

            var saveAction = (response.Action as SaveAction);
            saveAction.GameState.ShouldBe(expectedOutput);
        }
    }
}
