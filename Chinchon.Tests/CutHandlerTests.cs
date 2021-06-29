using Chinchon.Domain;
using Chinchon.GameHandlers;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Chinchon.Tests
{
    public class CutHandlerTests
    {
        [Fact(DisplayName = "Given turn value less than or equal to amount of players When cuting Then return expected error message")]
        public void Given_TurnValueLessThanOrEqualToAmountOfPlayers_When_Cutting_Then_ReturnExpectedErrorMessage()
        {
            var cards = new[]
{
                new Card(SuitsEnum.Golds, RanksEnum.One),
                new Card(SuitsEnum.Swords, RanksEnum.One),
                new Card(SuitsEnum.Clubs, RanksEnum.One),
                new Card(SuitsEnum.Golds, RanksEnum.Two),
                new Card(SuitsEnum.Swords, RanksEnum.Two),
                new Card(SuitsEnum.Clubs, RanksEnum.Two),
                new Card(SuitsEnum.Golds, RanksEnum.Three),
            };

            var random = new Random(0);

            var expectedOutput = "Can't cut before the second round";
            var cutHandler = new CutHandler(random);
            var gameState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.Turn = 1;
            });

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.Player1 = gameState.Player1.With(options => options.Cards = cards);
            });

            var response = cutHandler.Handle(new string[] { "cut", "1;2;3", "4;5;6", "7" }, gameState, new ApplicationState());
            var writeAction = ((WriteAction)response.Action);
            writeAction.Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given player with 7 cards and cut false When cuting Then return expected error message")]
        public void Given_PlayerWith7CardsAndCutedFalse_When_Cutting_Then_ReturnExpectedErrorMessage()
        {
            var expectedOutput = "Can't cut with 7 cards before anyone cut with 8 cards";

            var cards = new[]
            {
                new Card(SuitsEnum.Golds, RanksEnum.One),
                new Card(SuitsEnum.Swords, RanksEnum.One),
                new Card(SuitsEnum.Clubs, RanksEnum.One),
                new Card(SuitsEnum.Golds, RanksEnum.Two),
                new Card(SuitsEnum.Swords, RanksEnum.Two),
                new Card(SuitsEnum.Clubs, RanksEnum.Two),
                new Card(SuitsEnum.Golds, RanksEnum.Three),
            };

            var random = new Random(0);

            var cutHandler = new CutHandler(random);

            var gameState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.Turn = 3;
                options.PlayerTurn = 1;
                options.WasCut = false;
            });

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.Player1 = gameState.Player1.With(options => options.Cards = cards);
            });

            var response = cutHandler.Handle(new string[] { "cut", "1;2;3", "4;5;6" }, gameState, new ApplicationState());
            var writeAction = ((WriteAction)response.Action);
            writeAction.Output.ShouldBe(expectedOutput);
        }

        [Fact(DisplayName = "Given player with 7 cards and no inputs When cuting Then return expected state")]
        public void Given_PlayerWith7CardsAndNoInputs_When_Cutting_Then_ReturnExpectedState()
        {
            var cards = CardsService.GetCards().Take(7);
            var expectedState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.WasCut = true;
                options.RemainingPlayersToCut = 1;
                options.Hand = 1;
                options.Turn = 4;
                options.PlayerTurn = 2;
                options.Deck = Enumerable.Empty<Card>();
                options.Pile = Enumerable.Empty<Card>();
            });

            expectedState = expectedState.With(stateOptions =>
            {
                stateOptions.Player1 = expectedState.Player1.With(options =>
                {
                    options.Cards = cards;
                    options.Points = 10;
                });
            });

            var random = new Random(0);

            var cutHandler = new CutHandler(random);

            var gameState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.WasCut = true;
                options.Turn = 3;
                options.PlayerTurn = 1;
            });

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.Player1 = gameState.Player1.With(options => options.Cards = cards);
            });


            var response = cutHandler.Handle(new string[] { "cut" }, gameState, new ApplicationState());
            var saveAction = (SaveStateAction)response.Action;
            saveAction.GameState.ShouldBe(expectedState);
        }

        [Theory(DisplayName = "Given player with 7 cards and one input wrong format When cuting Then return expected state")]
        [InlineData("1|2|3")]
        [InlineData("a|b|c")]
        [InlineData("a;b;c")]
        [InlineData("1|2|c")]
        public void Given_PlayerWith7CardsAndOneInputWrongFormat_When_Cutting_Then_ReturnExpectedState(string parameter)
        {
            var cards = CardsService.GetCards().Take(7);
            var expectedOuput = "Invalid format";
            var random = new Random(0);

            var cutHandler = new CutHandler(random);
            var gameState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.WasCut = true;
                options.Turn = 3;
                options.PlayerTurn = 1;
            });

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.Player1 = gameState.Player1.With(options => options.Cards = cards);
            });

            var response = cutHandler.Handle(new string[] { "cut", parameter }, gameState, new ApplicationState());
            var writeAction = (WriteAction)response.Action;
            writeAction.Output.ShouldBe(expectedOuput);
        }

        [Fact(DisplayName = "Given player with 8 cards and valid hand When cuting Then return expected success state")]
        public void Given_PlayerWith8CardsAndValidHand_When_Cutting_Then_ReturnExpectedSuccessState()
        {
            var cards = new[]
{
                new Card(SuitsEnum.Golds, RanksEnum.One),
                new Card(SuitsEnum.Swords, RanksEnum.One),
                new Card(SuitsEnum.Clubs, RanksEnum.One),
                new Card(SuitsEnum.Golds, RanksEnum.Two),
                new Card(SuitsEnum.Swords, RanksEnum.Two),
                new Card(SuitsEnum.Clubs, RanksEnum.Two),
                new Card(SuitsEnum.Golds, RanksEnum.Three),
                new Card(SuitsEnum.Clubs, RanksEnum.King)
            };

            var expectedState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.WasCut = true;
                options.RemainingPlayersToCut = 1;
                options.Hand = 1;
                options.Turn = 4;
                options.PlayerTurn = 2;
                options.Deck = Enumerable.Empty<Card>();
                options.Pile = Enumerable.Empty<Card>();
            });

            expectedState = expectedState.With(stateOptions =>
            {
                stateOptions.Player1 = expectedState.Player1.With(options =>
                {
                    options.Cards = cards;
                    options.Points = 3;
                });
            });

            var random = new Random(0);

            var cutHandler = new CutHandler(random);
            var gameState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.Turn = 3;
                options.PlayerTurn = 1;
                options.WasCut = false;
            });

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.Player1 = gameState.Player1.With(options =>
                {
                    options.Cards = cards;
                });
            });

            var response = cutHandler.Handle(new string[] { "cut", "1;2;3", "4;5;6", "8" }, gameState, new ApplicationState());
            var saveAction = (SaveStateAction)response.Action;
            saveAction.GameState.ShouldBe(expectedState);
        }

        [Fact(DisplayName = "Given player with 8 cards and invalid hand When cuting with same card as one grouped Then return expected error message")]
        public void Given_PlayerWith8CardsAndInvalidHand_When_Cutting_Then_ReturnExpectedErrorMessage()
        {
            var cards = new[]
{
                new Card(SuitsEnum.Golds, RanksEnum.One),
                new Card(SuitsEnum.Swords, RanksEnum.One),
                new Card(SuitsEnum.Clubs, RanksEnum.One),
                new Card(SuitsEnum.Golds, RanksEnum.Two),
                new Card(SuitsEnum.Swords, RanksEnum.Two),
                new Card(SuitsEnum.Clubs, RanksEnum.Two),
                new Card(SuitsEnum.Golds, RanksEnum.Three),
                new Card(SuitsEnum.Golds, RanksEnum.One)
            };

            var expectedOutput = "Duplicated cards";
            var random = new Random(0);

            var cutHandler = new CutHandler(random);
            var gameState = new GameState(new Player(1), new Player(2)).With(options =>
            {
                options.Turn = 3;
                options.PlayerTurn = 1;
                options.WasCut = true;
            });

            gameState = gameState.With(stateOptions =>
            {
                stateOptions.Player1 = gameState.Player1.With(options =>
                {
                    options.Cards = cards;
                });
            });

            var response = cutHandler.Handle(new string[] { "cut", "1;2;3", "4;5;6", "8" }, gameState, new ApplicationState());
            var writeAction = (WriteAction)response.Action;
            writeAction.Output.ShouldBe(expectedOutput);
        }
    }
}
