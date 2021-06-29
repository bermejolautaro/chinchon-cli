using Chinchon.Domain;

namespace Chinchon
{
    public interface IGameHandler
    {
        public HandlerResponse Handle(string[] args, GameState gameState, ApplicationState appState);
    }
}
