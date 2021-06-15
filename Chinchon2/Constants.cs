using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon2
{
    public static class Constants
    {
        public static readonly IEnumerable<string> Suits = new[] { "Clubs", "Golds", "Cups", "Swords" };
        public static readonly IEnumerable<string> Ranks = Enumerable.Range(1, 12).Select(x => x.ToString());
    }
}
