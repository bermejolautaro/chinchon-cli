using System;
using System.Collections.Generic;
using System.Text;

namespace Chinchon
{
    public interface IMenuHandler
    {
        public HandlerResponse Handle(string[] args);
    }
}
