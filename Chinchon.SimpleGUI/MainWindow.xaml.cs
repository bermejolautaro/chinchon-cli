using Chinchon.Domain;
using Chinchon.Domain.Modules;
using ExhaustiveMatching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace Chinchon.SimpleGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string _path;
        private readonly Random _random;

        private readonly IDictionary<int, string> _namesByPlayerId = new Dictionary<int, string>()
        {
            [1] = "Lautaro",
            [2] = "Julieta"
        };

        private int _lastSelectedIndex;
        private Group _firstGroup;
        private Group _secondGroup;
        private Card _cardToCutWith;

        public string PlayerNameText { get; set; }

        public MainWindow()
        {
            _path = "./game.txt";
            _lastSelectedIndex = 0;
            _random = new Random();

            var gameState = FetchState(_path);

            InitializeComponent();
            UpdateUI(gameState);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            HandleResponse(StartModule.Start(_random, new Player(1), new Player(2)));
        }

        private void PickCardFromDeck_Click(object sender, RoutedEventArgs e)
        {
            var gameState = FetchState(_path);

            HandleResponse(Mediator.Send(new PickCardFromDeckRequest(gameState)));
        }

        private void PickCardFromPile_Click(object sender, RoutedEventArgs e)
        {
            var gameState = FetchState(_path);

            HandleResponse(Mediator.Send(new PickCardFromPileRequest(gameState)));
        }

        private void DiscardCard_Click(object sender, RoutedEventArgs e)
        {
            var gameState = FetchState(_path);

            if (CardsListBox.SelectedItem == null)
            {
                return;
            }
            else
            {
                var card = (Card) CardsListBox.Items.GetItemAt(CardsListBox.SelectedIndex);
                HandleResponse(Mediator.Send(new DiscardCardRequest(gameState, card)));
            }
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            var gameState = FetchState(_path);
            if (_firstGroup is null)
            {
                HandleResponse(Mediator.Send(new CutRequest(gameState, _random, Enumerable.Empty<Group>())));
            }
            else
            {
                if (_cardToCutWith is null)
                {

                    if (_secondGroup is null)
                    {
                        HandleResponse(Mediator.Send(new CutRequest(gameState, _random, new[] { _firstGroup, })));
                    }
                    else
                    {

                        HandleResponse(Mediator.Send(new CutRequest(gameState, _random, new[] { _firstGroup, _secondGroup })));
                    }
                }
                else
                {
                    if (_secondGroup is null)
                    {
                        HandleResponse(Mediator.Send(new CutRequest(gameState, _random, new[] { _firstGroup, }, _cardToCutWith)));
                    }
                    else
                    {
                        HandleResponse(Mediator.Send(new CutRequest(gameState, _random, new[] { _firstGroup, _secondGroup }, _cardToCutWith)));

                    }

                }
            }


            gameState = FetchState(_path);
            _firstGroup = null;
            _secondGroup = null;
            _cardToCutWith = null;

            UpdateUI(gameState);
        }

        private void MoveUp_Click(object sender, RoutedEventArgs e)
        {
            var gameState = FetchState(_path);

            if (CardsListBox.SelectedItem == null)
            {
                CardsListBox.SelectedIndex = _lastSelectedIndex;
            }
            else
            {
                _lastSelectedIndex = CardsListBox.SelectedIndex;
            }

            if (_lastSelectedIndex < 1)
            {
                return;
            }

            var card = (Card)CardsListBox.Items.GetItemAt(_lastSelectedIndex);
            HandleResponse(Mediator.Send(new MoveCardRequest(gameState, card, _lastSelectedIndex)));
            _lastSelectedIndex -= 1;
        }

        private void MoveDown_Click(object sender, RoutedEventArgs e)
        {
            var gameState = FetchState(_path);

            if (CardsListBox.SelectedItem == null)
            {
                CardsListBox.SelectedIndex = _lastSelectedIndex;
            }
            else
            {
                _lastSelectedIndex = CardsListBox.SelectedIndex;
            }

            if (_lastSelectedIndex > 8)
            {
                return;
            }

            var card = (Card)CardsListBox.Items.GetItemAt(_lastSelectedIndex);
            HandleResponse(Mediator.Send(new MoveCardRequest(gameState, card, _lastSelectedIndex + 2)));
            _lastSelectedIndex += 1;
        }

        private void SubmitFirstGroup_Click(object sender, RoutedEventArgs e)
        {
            var gameState = FetchState(_path);

            if (Group.IsValidGroup(CardsListBox.SelectedItems.Cast<Card>()))
            {
                _firstGroup = Group.CreateGroup(CardsListBox.SelectedItems.Cast<Card>().ToList());
            }

            UpdateUI(gameState);
        }

        private void SubmitSecondGroup_Click(object sender, RoutedEventArgs e)
        {
            var gameState = FetchState(_path);

            if (Group.IsValidGroup(CardsListBox.SelectedItems.Cast<Card>()))
            {
                _secondGroup = Group.CreateGroup(CardsListBox.SelectedItems.Cast<Card>().ToList());
            }

            UpdateUI(gameState);

        }

        private void SubmitCardToCutWith_Click(object sender, RoutedEventArgs e)
        {
            var gameState = FetchState(_path);

            _cardToCutWith = (Card)CardsListBox.SelectedItem;

            UpdateUI(gameState);
        }

        private void HandleResponse(IResult result)
        {
            switch (result)
            {
                case SuccessResult action:
                    {
                        SaveGameState(_path, action.GameState);
                        UpdateUI(action.GameState);
                        break;
                    }
                case ErrorResult action:
                    {
                        Trace.WriteLine(action.ErrorMessage);
                        break;
                    }
                default: throw ExhaustiveMatch.Failed(result);
            }
        }

        private void UpdateUI(GameState gameState)
        {
            CardsListBox.Items.Clear();

            PlayerNameTextBlock.Text = _namesByPlayerId[gameState.GetCurrentPlayer().Id];
            PlayerPointsTextBlock.Text = $"Points: {gameState.GetCurrentPlayer().Points}";
            PileTextBlock.Text = $"Pile: {gameState.Pile.FirstOrDefault()}";
            FirstGroupTextBlock.Text = $"First Group: {_firstGroup?.ToString()}";
            SecondGroupTextBlock.Text = $"Second Group: {_secondGroup?.ToString()}";
            CardToCutWithTextBlock.Text = $"Card to Cut With: {_cardToCutWith?.ToString()}";

            foreach (var card in gameState.GetCurrentPlayer().Cards)
            {
                CardsListBox.Items.Add(card);
            }
        }

        private static string ReadGameState(StreamReader sr)
        {
            var serializedGameStateBuilder = new StringBuilder();

            while (true)
            {
                string line = sr.ReadLine();

                if (line == null)
                {
                    break;
                }

                serializedGameStateBuilder.AppendLine(line);
            }

            return serializedGameStateBuilder.ToString();
        }

        private static void SaveGameState(string path, GameState gameState)
        {
            try
            {
                using StreamWriter fs = File.CreateText(path);
                fs.Write(gameState.SerializeGameState());
                fs.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static GameState FetchState(string path)
        {
            try
            {
                using StreamReader sr = File.OpenText(path);
                return GameService.DeserializeGameState(ReadGameState(sr));
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}\nCan't fetch state");
            }
        }
    }
}
