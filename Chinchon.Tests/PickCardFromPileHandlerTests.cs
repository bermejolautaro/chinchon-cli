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
            var gameState = new GameState(new Player(1), new Player(2));

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.PlayerTurn = 1;
                stateOptions.Player1 = gameState.Player1.With(options =>
                {
                    options.Cards = CardsService.GetCards().Take(7);
                });
            });

            var pickCardFromDeckHandler = new PickCardFromPileHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromPile" }, gameState, new ApplicationState());

            (response.Action as WriteAction).Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given picked turn state When running pick card from pile command then show expected error message")]
        public void Given_PickedTurnState_When_RunningPickCardFromPileCommand_Then_ShowExpectedErrorMessage()
        {
            var expectedOutput = "Only can pick cards when having 7 cards";
            var gameState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.Deck = CardsService.GetCards();
            });

            var pickCardFromDeckHandler = new PickCardFromPileHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromPile" }, gameState, new ApplicationState());

            (response.Action as WriteAction).Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given initial turn state When running pick card from pile command then show expected error message")]
        public void Given_InitialTurnState_When_RunningPickCardFromPileCommand_Then_ShowExpectedErrorMessage()
        {
            var cards = new[] { new Card(SuitsEnum.Golds, RanksEnum.One) };
            var removeResponse = CardsService.RemoveTopCard(cards);

            var expectedOutput = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.Hand = 1;
                options.Turn = 1;
                options.PlayerTurn = 1;
                options.Pile = removeResponse.Cards;
            });

            expectedOutput = expectedOutput.With(stateOptions =>
            {
                stateOptions.Player1 = expectedOutput.Player1.With(options =>
                {
                    options.Cards = CardsService.GetCards().Take(7).Append(removeResponse.RemovedCard);
                });
            });

            var gameState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.Pile = cards;
            });

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.Player1 = gameState.Player1.With(options =>
                {
                    options.Cards = CardsService.GetCards().Take(7);
                });
            });

            var pickCardFromDeckHandler = new PickCardFromPileHandler();
            var response = pickCardFromDeckHandler.Handle(new[] { "pickCardFromPile" }, gameState, new ApplicationState());

            var saveAction = (SaveStateAction)response.Action;
            saveAction.GameState.ShouldBe(expectedOutput);
        }
    }
}
