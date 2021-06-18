using Chinchon.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chinchon
{
    public interface IGameHandler
    {
        public HandlerResponse Handle(string[] args, GameState gameState);
    }
}
