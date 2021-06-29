using Chinchon.Domain;
using ExhaustiveMatching;
using System;

namespace Chinchon
{
    [Closed(typeof(SaveStateAction), typeof(NothingAction), typeof(WriteAction), typeof(SaveConfigurationAction))]
    public interface IAction { }

    public class NothingAction : IAction { }

    public class SaveConfigurationAction : IAction
    {
        public SaveConfigurationAction(Guid gameGuid)
        {
            GameGuid = gameGuid;
        }

        public Guid GameGuid { get; }
    }

    public class SaveStateAction : IAction
    {
        public SaveStateAction(GameState gameState, ApplicationState appState)
        {
            GameState = gameState;
            AppState = appState;
        }

        public GameState GameState { get; }
        public ApplicationState AppState { get; }
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