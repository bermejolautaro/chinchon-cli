using Chinchon.Domain;
using Chinchon.Domain.Modules;
using System.Collections.Generic;

namespace Chinchon.GameHandlers
{
    public class PickCardFromDeckHandler : IGameHandler
    {
        public HandlerResponse Handle(string[] args, GameState gameState)
        {
            return Mediator.Send(new PickCardFromDeckRequest(gameState)) switch 
            {
                SuccessResult result => new HandlerResponse() { Action = new SaveAction(result.GameState) },
                ErrorResult result => new HandlerResponse() { Action = new WriteAction(result.ErrorMessage) },
            };
        }
    }
}
