using System;
using System.Collections.Generic;
using System.Text;

namespace Chinchon2
{
    public interface IGameHandler
    {
        public void Handle(string[] args, GameState gameState);
    }
}
