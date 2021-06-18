using ExhaustiveMatching;
using System;
using System.Collections.Generic;

namespace Chinchon.Domain
{
    [Closed(
        typeof(PickCardFromDeckRequest),
        typeof(PickCardFromPileRequest),
        typeof(DiscardCardRequest),
        typeof(MoveCardRequest),
        typeof(CutRequest)
    )]
    public interface IRequest 
    {
        public GameState GameState { get; }
    }

    public class PickCardFromDeckRequest : IRequest { 
        public PickCardFromDeckRequest(GameState gameState)
        {
            GameState = gameState;
        }

        public GameState GameState { get; }
    }

    public class PickCardFromPileRequest : IRequest
    {
        public PickCardFromPileRequest(GameState gameState)
        {
            GameState = gameState;
        }

        public GameState GameState { get; }
    }

    public class DiscardCardRequest : IRequest
    {
        public DiscardCardRequest(GameState gameState, Card cardToDiscard)
        {
            GameState = gameState;
            CardToDiscard = cardToDiscard;
        }

        public GameState GameState { get; }
        public Card CardToDiscard { get; }
    }

    public class MoveCardRequest : IRequest
    {
        public MoveCardRequest(GameState gameState, Card cardToMove, int position)
        {
            GameState = gameState;
            Card = cardToMove;
            ToPosition = position;
        }

        public GameState GameState { get; }
        public Card Card { get; }
        public int ToPosition { get; }
    }

    public class CutRequest : IRequest
    {
        public CutRequest(GameState gameState, Random random, IEnumerable<Group> groups, Card? cardToCutWith = null)
        {
            GameState = gameState;
            Random = random;
            Groups = groups;
            CardToCutWith = cardToCutWith;
        }

        public GameState GameState { get; }
        public Random Random { get; }
        public IEnumerable<Group> Groups { get; }
        public Card? CardToCutWith { get; }
    }

}
