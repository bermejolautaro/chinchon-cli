using Chinchon.Domain;
using Chinchon.Domain.Modules;
using System.Linq;

namespace Chinchon.GameHandlers
{
    public class DiscardCardHandler : IGameHandler
    {
        public HandlerResponse Handle(string[] args, GameState gameState)
        {
            var isInteger = int.TryParse(args[1], out int cardIndex);

            if (!isInteger)
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid input") };
            }

            if(cardIndex < 1 || cardIndex > 8)
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid index") };
            }

            var card = gameState.GetCurrentPlayerCards().ElementAt(cardIndex - 1);

            return Mediator.Send(new DiscardCardRequest(gameState, card)) switch
            {
                SuccessResult result => new HandlerResponse() { Action = new SaveAction(result.GameState) },
                ErrorResult result => new HandlerResponse() { Action = new WriteAction(result.ErrorMessage) },
            };
        }
    }
}
