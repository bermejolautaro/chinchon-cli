using Chinchon.Domain.Modules;
using ExhaustiveMatching;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chinchon.Domain
{
    public static class Mediator
    {
        public static IResult Send(IRequest request)
        {
            if(request.GameState.GameOver)
            {
                return new ErrorResult("This games has ended");
            }

            return request switch
            {
                PickCardFromDeckRequest req => PickModule.PickCardFromDeck(req.GameState),
                PickCardFromPileRequest req => PickModule.PickCardFromPile(req.GameState),
                DiscardCardRequest req => DiscardModule.DiscardCard(req.GameState, req.CardToDiscard),
                MoveCardRequest req => MoveModule.Move(req.GameState, req.Card, req.ToPosition),
                CutRequest req => CutModule.Cut(req.GameState, req.Random, req.Groups, req.CardToCutWith),
                _ => throw ExhaustiveMatch.Failed(request)
            };
        }
    }
}
