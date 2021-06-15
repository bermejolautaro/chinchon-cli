using Chinchon.Domain2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon2
{
    public abstract class PickCardHandler : IGameHandler
    {
        public void Handle(string[] args, GameState gameState)
        {
            if (gameState.TurnState != TurnStateEnum.Initial)
            {
                Console.WriteLine("Only can pick cards in initial state");
                return;
            }

            var response = CardsService.RemoveTopCard(GetCards(gameState));

            if (gameState.PlayerTurn == 1)
            {
                gameState.Player1Cards = gameState.Player1Cards.Concat(new[] { response.RemovedCard });
            }
            else
            {
                gameState.Player2Cards = gameState.Player2Cards.Concat(new[] { response.RemovedCard });
            }

            SetCards(gameState, response.Cards);
            gameState.TurnState = TurnStateEnum.Picked;

            IOGameService.SaveGameState(IOGameService.path, gameState);
        }

        protected abstract IEnumerable<Card> GetCards(GameState gameState);
        protected abstract void SetCards(GameState gameState, IEnumerable<Card> cards);
    }
}
