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
            var gameState = new GameState(new Player(1), new Player(2));

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.PlayerTurn = 1;
                stateOptions.Player1 = gameState.Player1.With(options =>
                {
                    options.Cards = CardsService.GetCards().Take(7);
                });
            });
            
            var pickCardFromDeckHandler = new PickCardFromDeckHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromDeck" }, gameState, new ApplicationState());

            (response.Action as WriteAction).Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given picked turn state When running pick card from deck command then show expected error message")]
        public void Given_PickedTurnState_When_RunningPickCardFromDeckCommand_Then_ShowExpectedErrorMessage()
        {
            var expectedOutput = "Only can pick cards when having 7 cards";
            var gameState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.Deck = CardsService.GetCards();
            });

            var pickCardFromDeckHandler = new PickCardFromDeckHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromDeck" }, gameState, new ApplicationState());

            (response.Action as WriteAction).Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given initial turn state When running pick card from deck command then show expected error message")]
        public void Given_InitialTurnState_When_RunningPickCardFromDeckCommand_Then_ShowExpectedErrorMessage()
        {
            var cards = CardsService.GetCards();
            var removeResponse = CardsService.RemoveTopCard(cards);

            var expectedState = new GameState(new Player(1), new Player(2));

            expectedState = expectedState.With(stateOptions =>
            {
                stateOptions.Hand = 1;
                stateOptions.Turn = 1;
                stateOptions.PlayerTurn = 1;
                stateOptions.Deck = removeResponse.Cards;
                stateOptions.Player1 = expectedState.Player1.With(options =>
                {
                    options.Cards = CardsService.GetCards().Take(7).Append(removeResponse.RemovedCard);
                });
            });

            var gameState = new GameState(new Player(1), new Player(2));

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.PlayerTurn = 1;
                stateOptions.Deck = CardsService.GetCards();
                stateOptions.Player1 = gameState.Player1.With(options =>
                {
                    options.Cards = CardsService.GetCards().Take(7);
                });
            });

            var pickCardFromDeckHandler = new PickCardFromDeckHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromDeck" }, gameState, new ApplicationState());

            var saveAction = (response.Action as SaveStateAction);
            saveAction.GameState.ShouldBe(expectedState);
        }
    }
}
