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

        public IResult Handle(string[] args)
        {
            return StartModule.Start(_random);
        }
    }
}
