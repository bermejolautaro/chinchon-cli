using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chinchon.API
{
    public class ExtendedGameState
    {
        public Guid Player1Guid { get; set; }
        public Guid Player2Guid { get; set; }
        public Guid? Player3Guid { get; set; }
        public Guid? Player4Guid { get; set; }
        public int PlayerKeyToBeRetrieved { get; set; }
    }

    public static class ExtendedGameStateExtensions
    {
        public static int GetPlayerId(this ExtendedGameState extendedGameState, Guid playerGuid)
        {
            if(extendedGameState.Player1Guid == playerGuid)
            {
                return 1;
            }
            else if(extendedGameState.Player2Guid == playerGuid)
            {
                return 2;
            }
            else if (extendedGameState.Player3Guid == playerGuid)
            {
                return 3;
            }
            else if (extendedGameState.Player2Guid == playerGuid)
            {
                return 4;
            }
            else
            {
                throw new InvalidOperationException("Impossible state");
            }
        }
    }
}
