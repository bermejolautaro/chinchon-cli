using Chinchon.Domain;
using Chinchon.Domain.Modules;
using System;

namespace Chinchon.MenuHandlers
{
    public class StartHandler : IMenuHandler
    {
        private readonly Random _random;

        public StartHandler(Random random)
        {
            _random = random;
        }

        public HandlerResponse Handle(string[] args)
        {
            var appState = new ApplicationState();

            if (args.Length >= 2)
            {
                appState.Player1Name = args[1];
            }

            if (args.Length >= 3)
            {
                appState.Player2Name = args[2];
            }

            if (args.Length >= 4)
            {
                appState.Player3Name = args[3];
            }

            if (args.Length == 5)
            {
                appState.Player4Name = args[4];
            }

            if (args.Length == 3)
            {
                return StartModule.Start(_random, new Player(1), new Player(2)) switch
                {
                    SuccessResult result => new HandlerResponse() { Action = new SaveStateAction(result.GameState, appState) },
                    ErrorResult result => new HandlerResponse() { Action = new WriteAction(result.ErrorMessage) },
                };
            }
            else if (args.Length == 4)
            {
                return StartModule.Start(_random, new Player(1), new Player(2), new Player(3)) switch
                {
                    SuccessResult result => new HandlerResponse() { Action = new SaveStateAction(result.GameState, appState) },
                    ErrorResult result => new HandlerResponse() { Action = new WriteAction(result.ErrorMessage) },
                };
            }
            else
            {
                return StartModule.Start(_random, new Player(1), new Player(2), new Player(3), new Player(4)) switch
                {
                    SuccessResult result => new HandlerResponse() { Action = new SaveStateAction(result.GameState, appState) },
                    ErrorResult result => new HandlerResponse() { Action = new WriteAction(result.ErrorMessage) },
                };
            }
        }
    }
}
