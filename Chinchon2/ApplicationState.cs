using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Chinchon
{
    public class ApplicationState : IEquatable<ApplicationState>
    {
        public string Player1Name { get; set; } = "Player 1";
        public string Player2Name { get; set; } = "Player 2";
        public string Player3Name { get; set; } = "Player 3";
        public string Player4Name { get; set; } = "Player 4";

        public override bool Equals(object? obj)
        {
            return obj is null || !(obj is ApplicationState player) ? false : Equals(player);
        }

        public bool Equals(ApplicationState other)
        {
            return
                Player1Name == other.Player1Name &&
                Player2Name == other.Player2Name &&
                Player3Name == other.Player3Name &&
                Player4Name == other.Player4Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string? ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public static class ApplicationStateExtensions
    {
        public static string GetPlayerNameById(this ApplicationState appState, int playerId)
        {
            return playerId switch
            {
                1 => appState.Player1Name,
                2 => appState.Player2Name,
                3 => appState.Player3Name,
                4 => appState.Player4Name,
            };
        }
    }
}
