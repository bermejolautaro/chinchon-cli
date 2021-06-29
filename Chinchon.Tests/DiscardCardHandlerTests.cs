using Chinchon.Domain;
using Chinchon.GameHandlers;
using Shouldly;
using System.Linq;
using Xunit;

namespace Chinchon.Tests
{
    public class DiscardCardHandlerTests
    {
        [Fact(DisplayName = "Given a player with 7 cards When discarding Then should show expected error message")]
        public void Given_PlayerWith7Cards_When_Discarding_Then_ShouldShowExpectedErrorMessage()
        {
            var expectedOutput = "Only can discard cards when you have eight cards in your hand";
            var discardCardHandler = new DiscardCardHandler();
            var gameState = new GameState(new Player(1), new Player(2));

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.PlayerTurn = 1;
                stateOptions.Player1 = gameState.Player1.With(options =>
                {
                    options.Cards = CardsService.GetCards().Take(7);
                });
            });

            var response = discardCardHandler.Handle(new string[] { "discardCard", "1" }, gameState, new ApplicationState());

            ((WriteAction)response.Action).Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given an non-number card index When discarding Then should show expected error message")]
        public void Given_NonNumberCardIndex_When_Discarding_Then_ShouldShowExpectedErrorMessage()
        {
            var expectedOutput = "Invalid input";
            var discardCardHandler = new DiscardCardHandler();
            var gameState = new GameState(new Player(1), new Player(2));

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.PlayerTurn = 1;
                stateOptions.Player1 = gameState.Player1.With(options =>
                {
                    options.Cards = CardsService.GetCards().Take(8);
                });
            });
            var response = discardCardHandler.Handle(new string[] { "discardCard", "uno" }, gameState, new ApplicationState());

            ((WriteAction)response.Action).Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given a number out of bounds When discarding Then should show expected error message")]
        public void Given_NumberOutOfBounds_When_Discarding_Then_ShouldShowExpectedErrorMessage()
        {
            var expectedOutput = "Invalid index";
            var discardCardHandler = new DiscardCardHandler();
            var gameState = new GameState(new Player(1), new Player(2));

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.PlayerTurn = 1;
                stateOptions.Player1 = gameState.Player1.With(options =>
                {
                    options.Cards = CardsService.GetCards().Take(8);
                });
            });

            var response = discardCardHandler.Handle(new string[] { "discardCard", "0" }, gameState, new ApplicationState());

            ((WriteAction)response.Action).Output.ShouldBe(expectedOutput);
        }
    }
}
