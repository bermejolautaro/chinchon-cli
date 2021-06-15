namespace Chinchon2
{
    public class StartHandler : IMenuHandler
    {
        public void Handle(string[] args)
        {
            var initialState = new GameState()
            {
                Winner = "",
                Hand = 1,
                Turn = 1,
                TurnState = TurnStateEnum.Initial,
                PlayerAmount = 2,
                PlayerTurn = 1,
                Player1Points = 0,
                Player2Points = 0,
            };

            var newGameState = GameService.ShuffleAndDealCards(initialState);

            IOGameService.SaveGameState(IOGameService.path, newGameState);
        }
    }
}
