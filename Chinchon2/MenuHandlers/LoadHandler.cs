using System;
using System.Collections.Generic;
using System.Text;

namespace Chinchon.MenuHandlers
{
    public class LoadHandler : IMenuHandler
    {
        public HandlerResponse Handle(string[] args)
        {
            return new HandlerResponse()
            {
                Action = new SaveConfigurationAction(new Guid(args[1]))
            };
        }
    }
}
