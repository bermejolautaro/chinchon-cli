using Chinchon.Domain;
using ExhaustiveMatching;

namespace Chinchon
{
    [Closed(typeof(SaveAction), typeof(NothingAction), typeof(WriteAction))]
    public interface IAction { }

    public class NothingAction : IAction { }

    public class SaveAction : IAction
    {
        public SaveAction(GameState gameState)
        {
            GameState = gameState;
        }

        public GameState GameState { get; }
    }

    public class WriteAction : IAction
    {
        public WriteAction(string Output)
        {
            this.Output = Output;
        }

        public string Output { get; }
    }
}