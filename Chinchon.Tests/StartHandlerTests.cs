using Chinchon.Domain;
using Chinchon.MenuHandlers;
using Shouldly;
using System;
using Xunit;

namespace Chinchon.Tests
{
    public class StartHandlerTests
    {
        [Fact(DisplayName = "Given a new game When started Then should be initialized with correct values")]
        public void Given_NewGame_When_Started_Then_ShouldBeInitializedWithCorrectValues()
        {
            var seed = 0;
            var expectedGameState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.Hand = 1;
                options.Turn = 1;
                options.PlayerTurn = 1;
            });

            var expectedAppState = new ApplicationState()
            {
                Player1Name = "Lautaro",
                Player2Name = "Julieta"
            };

            expectedGameState = expectedGameState.ShuffleAndDealCards(new Random(seed));

            var startHandler = new StartHandler(new Random(seed));
            var result = startHandler.Handle(new string[] { "start", "Lautaro", "Julieta" });

            var saveAction = (SaveStateAction) result.Action;
            saveAction.GameState.ShouldBe(expectedGameState);
            saveAction.AppState.ShouldBe(expectedAppState);
            saveAction.ShouldBeOfType(typeof(SaveStateAction));
        }
    }
}
