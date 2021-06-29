using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Chinchon.Domain.CardsService;

namespace Chinchon.Domain
{
    public static class PlayerService
    {
        public static Player DeserializePlayer(string serializedPlayer)
        {
            PlayerOptions playerOptions = new PlayerOptions();
            var properties = typeof(PlayerOptions).GetProperties();

            var lines = serializedPlayer
                .Split("|")
                .Where(x => !string.IsNullOrEmpty(x));

            foreach (var line in lines)
            {
                if (line == null)
                {
                    break;
                }

                var splittedLine = line.Split("=");
                var key = splittedLine[0];
                var value = splittedLine[1];

                foreach (var property in properties)
                {
                    if (property.Name != key)
                    {
                        continue;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(string)))
                    {
                        property.SetValue(playerOptions, value);
                        break;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(bool)))
                    {
                        property.SetValue(playerOptions, bool.Parse(value));
                        break;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(int)))
                    {
                        property.SetValue(playerOptions, int.Parse(value));
                        break;
                    }

                    if (property.PropertyType.IsAssignableFrom(typeof(IEnumerable<Card>)))
                    {
                        var listValue = string.IsNullOrEmpty(value)
                                ? Enumerable.Empty<Card>()
                                : value.Split(";").Select(x => DeserializeCard(x));

                        property.SetValue(playerOptions, listValue);
                        break;
                    }
                }
            }

            return new Player(playerOptions);
        }
    }
}
