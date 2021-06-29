using Chinchon.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Chinchon.API.Dtos
{
    public class GameStateDto
    {
        public bool GameOver { get; set; }
        public bool WasCut { get; set; }
        public int RemainingPlayersToCut { get; set; }
        public int Hand { get; set; }
        public int Turn { get; set; }
        public int PlayerTurn { get; set; }
        public ICollection<CardDto> Deck { get; set; } = new Collection<CardDto>();
        public ICollection<CardDto> Pile { get; set; } = new Collection<CardDto>();
        public PlayerDto? Player1 { get; set; }
        public PlayerDto? Player2 { get; set; }
        public PlayerDto? Player3 { get; set; }
        public PlayerDto? Player4 { get; set; }
    }

    public static class GameStateDtoExtensions
    {
        public static GameState ToGameState(this GameStateDto g)
        {
            return new GameState(
                new GameStateOptions
                {
                    Deck = g.Deck.Select(x => new Card(x.Suit, x.Rank)),
                    GameOver = g.GameOver,
                    Hand = g.Hand,
                    Pile = g.Pile.Select(x => new Card(x.Suit, x.Rank)),
                    Player1 = g.Player1!.ToPlayer(),
                    Player2 = g.Player2!.ToPlayer(),
                    Player3 = g.Player3?.ToPlayer(),
                    Player4 = g.Player4?.ToPlayer(),
                    PlayerTurn = g.PlayerTurn,
                    RemainingPlayersToCut = g.RemainingPlayersToCut,
                    Turn = g.Turn,
                    WasCut = g.WasCut
                });
        }
    }
}
