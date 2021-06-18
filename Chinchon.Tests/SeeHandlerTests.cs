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
                "Player 1\r\n" +
                "Points: 0\r\n" +
                "Pile: \r\n";
            var gameState = new GameState();
            var seeHandler = new SeeHandler();
            var response = seeHandler.Handle(new[] { "path", "see", "current" }, gameState);

            (response.Action as WriteAction).Output.ShouldBe(expectedOuput);
        }
    }
}
