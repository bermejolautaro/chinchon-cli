using Chinchon.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chinchon.API.Responses
{
    public class CreateGameResponse
    {
        public Guid GameGuid { get; set; }
        public Guid Player1Guid { get; set; }
    }
}
