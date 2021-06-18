using System.Collections.Generic;
using System.Linq;

namespace Chinchon.Domain
{
    public class Player
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = "";
        public int Points { get; private set; }
        public IEnumerable<Card> Cards { get; private set; } = Enumerable.Empty<Card>();

        public Player(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
