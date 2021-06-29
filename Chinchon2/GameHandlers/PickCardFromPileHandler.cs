using Chinchon.Domain;
using Chinchon.Domain.Modules;

namespace Chinchon.GameHandlers
{
    public class PickCardFromPileHandler : IGameHandler
    {
        public HandlerResponse Handle(string[] args, GameState gameState, ApplicationState appState)
        {
            return Mediator.Send(new PickCardFromPileRequest(gameState)) switch
            {
                SuccessResult result => new HandlerResponse() { Action = new SaveStateAction(result.GameState, appState) },
                ErrorResult result => new HandlerResponse() { Action = new WriteAction(result.ErrorMessage) },
            };
        }
    }
}
