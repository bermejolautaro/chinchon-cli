using System;
using System.Collections.Generic;
using System.Text;

namespace Chinchon.Domain.Modules
{
    public static class StartModule
    {
        public static IResult Start(Random random, Player player1, Player player2)
        {
            return new SuccessResult(new GameState(player1, player2).ShuffleAndDealCards(random));
        }

        public static IResult Start(Random random, Player player1, Player player2, Player player3)
        {
            return new SuccessResult(new GameState(player1, player2, player3).ShuffleAndDealCards(random));
        }

        public static IResult Start(Random random, Player player1, Player player2, Player player3, Player player4)
        {
            return new SuccessResult(new GameState(player1, player2, player3, player4).ShuffleAndDealCards(random));
        }
    }
}
