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
            var expectedState = new GameState()
            {
                Hand = 1,
                Turn = 1,
                PlayerAmount = 2,
                PlayerTurn = 1,
                Player1Points = 0,
                Player2Points = 0,
            };

            expectedState = GameService.ShuffleAndDealCards(expectedState, new Random(seed));

            var startHandler = new StartHandler(new Random(seed));
            var result = startHandler.Handle(new string[0]);

            var saveAction = (SuccessResult)result;
            saveAction.GameState.ShouldBe(expectedState);
            saveAction.ShouldBeOfType(typeof(SuccessResult));
        }
    }
}
