using Chinchon.Domain;
using ExhaustiveMatching;

namespace Chinchon
{
    [Closed(typeof(SuccessResult), typeof(ErrorResult))]
    public interface IResult { }

    public class SuccessResult : IResult
    {
        public SuccessResult(GameState gameState)
        {
            GameState = gameState;
        }

        public GameState GameState { get; }
    }

    public class ErrorResult : IResult
    {
        public ErrorResult(string Output)
        {
            this.ErrorMessage = Output;
        }

        public string ErrorMessage { get; }
    }
}
