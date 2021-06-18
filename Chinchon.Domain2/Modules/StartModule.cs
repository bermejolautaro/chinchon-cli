using System;
using System.Collections.Generic;
using System.Text;

namespace Chinchon.Domain.Modules
{
    public static class StartModule
    {
        public static IResult Start(Random random)
        {
            var player1 = new Player(0, "Lautaro");
            var player2 = new Player(1, "Julieta");

            var initialState = new GameState()
            {
                Hand = 1,
                Turn = 1,
                PlayerAmount = 2,
                PlayerTurn = 1,
                Player1Points = 0,
                Player2Points = 0,
            }.WithRemainingPlayerToCut(-1);

            var newGameState = GameService.ShuffleAndDealCards(initialState, random);

            return new SuccessResult(newGameState);
        }
    }
}
