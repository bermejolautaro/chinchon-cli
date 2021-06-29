using Chinchon.Domain;
using Chinchon.GameHandlers;
using Shouldly;
using Xunit;

namespace Chinchon.Tests
{
    public class SeeHandlerTests
    {
        [Fact(DisplayName = "Given empty game state When running see command then show expected output")]
        public void Given_EmptyGameState_When_RunningSeeCommand_Then_ShowExpectedOutput()
        {
            var expectedOuput =
                $"Lautaro\r\n" +
                $"Points: 0\r\n" +
                $"Pile: \r\n";
            var gameState = new GameState(new Player(1), new Player(2));
            var appState = new ApplicationState()
            {
                Player1Name = "Lautaro",
                Player2Name = "Julieta"
            };

            var seeHandler = new SeeHandler();
            var response = seeHandler.Handle(new[] { "see", "current" }, gameState, appState);

            (response.Action as WriteAction).Output.ShouldBe(expectedOuput);
        }
    }
}
