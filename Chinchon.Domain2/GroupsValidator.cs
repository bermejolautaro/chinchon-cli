using Chinchon.Domain2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.Domain2
{
    public class GroupsValidator
    {
        public static bool CanCutWithTheseGroups(IEnumerable<Group> groups)
        {
            return groups.Select(g => g.Cards.Count()).Sum() == 7;
        }
    }
}
