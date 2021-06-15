using Chinchon.Domain2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chinchon.Domain2
{
    public class Game
    {
        private readonly int _amountOfPlayers;
        private readonly Dictionary<int, Player> _playersPerPlayerId;
        private readonly IList<Round> _rounds = new List<Round>();
        private int _currentPlayerId;
        private int _currentRound;

        public Game(int amountOfPlayers)
        {
            _amountOfPlayers = amountOfPlayers;

            _currentRound = 0;
            _currentPlayerId = 0;

            _playersPerPlayerId = new Dictionary<int, Player>()
            {
                [0] = new Player(),
                [1] = new Player(),
                [2] = new Player(),
                [3] = new Player()
            };

            _rounds.Add(new Round());

            for (int cardInHandPosition = 0; cardInHandPosition < 7; cardInHandPosition++)
            {
                for (int playerId = 0; playerId < _amountOfPlayers; playerId++)
                {
                    _playersPerPlayerId[playerId].AddCard(_rounds[_currentRound].PickCardFromDeck());
                }
            }

            _rounds[_currentRound].PutCardOnPile(_rounds[_currentRound].PickCardFromDeck());
        }

        public void NextPlayer()
        {
            _currentPlayerId = (_currentPlayerId + 1) % _amountOfPlayers;
        }

        public void NextRound()
        {
            _rounds.Add(new Round());
            _currentRound++; ;
        }

        public Card GetCurrentPlayerCardAtPosition(int position)
        {
            return GetCurrentPlayer().GetCardAtPosition(position);
        }

        public int GetCurrentPlayerId()
        {
            return _currentPlayerId;
        }

        public void CurrentPlayerCutWith(IEnumerable<Group> groups)
        {
            foreach (var player in _playersPerPlayerId.Values)
            {
                var points = player.CalculatePoints();
                player.AddPoints(points);
            }
        }

        public void CurrentPlayerCut(IEnumerable<Group> groups)
        {
            foreach (var player in _playersPerPlayerId.Values)
            {
                var points = player.CalculatePoints();
                player.AddPoints(points);
            }
        }

        public bool CanCurrentPlayerCutWith(IEnumerable<Group> groups)
        {
            var card = GetCurrentPlayerUngroupedCards(groups).Single();

            if (int.Parse(card.Rank) > 4)
                return false;

            if (groups.Select(g => g.Contains(card)).Any(hasCard => hasCard))
                return false;

            if (!GetCurrentPlayer().Cards.Contains(card))
                return false;

            return true;
        }

        public Player GetCurrentPlayer()
        {
            return _playersPerPlayerId[_currentPlayerId];
        }

        public IEnumerable<Player> GetOtherPlayers()
        {
            return _playersPerPlayerId.Where(p => p.Key != _currentPlayerId).Select(pair => pair.Value);
        }

        public bool HasWinner()
        {
            var howManyLose =
                _playersPerPlayerId.Values
                    .Take(_amountOfPlayers)
                    .Select(player => player.CalculatePoints())
                    .Where(points => points >= 100)
                    .Count();

            return howManyLose >= _amountOfPlayers - 1;
        }

        public bool CanCurrentPlayerFormGroup(string[] group)
        {
            return false;
        }

        private Round GetCurrentRound()
        {
            return _rounds[_currentRound];
        }

        public IEnumerable<Card> GetCurrentPlayerCards()
        {
            return GetCurrentPlayer().Cards;
        }

        public void CurrentPlayerDiscardCard(Card card)
        {
            PutCardOnPile(card);
            GetCurrentPlayer().RemoveCard(card);
        }

        public bool CanCurrentPlayerCut()
        {
            return false;
        }

        public void CurrentPlayerPickCardFromDeck()
        {
            GetCurrentPlayer().AddCard(GetCurrentRound().PickCardFromDeck());
        }

        public void CurrentPlayerPickCardFromPile()
        {
            GetCurrentPlayer().AddCard(GetCurrentRound().PickCardFromPile());
        }

        public bool IsPileEmpty()
        {
            return GetCurrentRound().IsPileEmpty();
        }

        public Card PeekCardFromPile()
        {
            return GetCurrentRound().PeekCardFromPile();
        }

        public void PutCardOnPile(Card card)
        {
            GetCurrentRound().PutCardOnPile(card);
        }

        public bool GroupIsValid(IEnumerable<Card> group)
        {
            return Group.IsValidGroup(group);
        }

        public bool AreAllCardsGrouped(IEnumerable<Group> groups)
        {
            return groups.Select(g => g.Cards.Count()).Sum() == 7;
        }

        public IEnumerable<Card> GetCurrentPlayerUngroupedCards(IEnumerable<Group> groups)
        {
            var ungroupedcards = GetCurrentPlayerCards().ToHashSet();
            ungroupedcards.ExceptWith(groups.SelectMany(g => g.Cards));

            return ungroupedcards;
        }
    }
}
