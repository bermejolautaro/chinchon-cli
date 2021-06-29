using Chinchon.Domain;
using Chinchon.Domain.Modules;
using System.Linq;

namespace Chinchon.GameHandlers
{
    public class MoveHandler : IGameHandler
    {
        public HandlerResponse Handle(string[] args, GameState gameState, ApplicationState appState)
        {
            if (args.Length != 3)
            {
                return new HandlerResponse()
                {
                    Action = new WriteAction("Invalid amount of arguments")
                };
            }

            var isValidFirstArgument = int.TryParse(args[1], out int cardIndex);
            var isValidSecondArgument = int.TryParse(args[2], out int toIndex);

            if (!isValidFirstArgument || !isValidSecondArgument)
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid arguments") };
            }

            if(cardIndex < 1 || cardIndex > 8)
            {
                return new HandlerResponse() { Action = new WriteAction("Invalid index") };
            }

            var card = gameState.GetCurrentPlayer().Cards.ElementAt(cardIndex);

            return Mediator.Send(new MoveCardRequest(gameState, card, toIndex)) switch
            {
                SuccessResult result => new HandlerResponse() { Action = new SaveStateAction(result.GameState, appState) },
                ErrorResult result => new HandlerResponse() { Action = new WriteAction(result.ErrorMessage) },
            };
        }
    }
}
