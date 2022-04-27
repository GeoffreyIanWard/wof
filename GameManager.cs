using LeapWoF.Interfaces;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace LeapWoF
{

    /// <summary>
    /// The GameManager class, handles all game logic
    /// </summary>
    public class GameManager
    {

        /// <summary>
        /// The input provider
        /// </summary>
        private IInputProvider inputProvider;

        /// <summary>
        /// The output provider
        /// </summary>
        private IOutputProvider outputProvider;

        /// <summary>
        /// The render handler
        /// </summary>
        private Display display;

        /// <summary>
        /// The players who are playing the game
        /// </summary>
        public List<Player> players = new List<Player>();

        /// <summary>
        /// The puzzle currently being played
        /// </summary>
        public Puzzle currentPuzzle;

        public Wheel wheel = new Wheel();

        /// <summary>
        /// Gamestate is an enum that controls the state of the game
        /// </summary>
        public GameState GameState { get; private set; }
        public int Round { get; private set; }

        private int Turn = 1;
        private bool breakGameLoop;

        public GameManager() : this(new ConsoleInputProvider(), new ConsoleOutputProvider())
        {

        }

        public GameManager(IInputProvider inputProvider, IOutputProvider outputProvider)
        {
            if (inputProvider == null)
                throw new ArgumentNullException(nameof(inputProvider));
            if (outputProvider == null)
                throw new ArgumentNullException(nameof(outputProvider));

            this.inputProvider = inputProvider;
            this.outputProvider = outputProvider;

            display = new Display();

            GameState = GameState.WaitingToStart;
        }

        /// <summary>
        /// Manage game according to game state
        /// </summary>
        public void StartGame()
        {
            if (GameState != GameState.RoundOver)
            {
                InitGame();
            }
            else
            {
                breakGameLoop = false;
            }
            try
            {

                while (!breakGameLoop)
                {

                    if (GameState == GameState.GameOver)
                    {
                        outputProvider.WriteLine("Game over");
                        break;
                    }

                    if (GameState == GameState.RoundOver)
                    {
                        StartNewRound();
                        continue;
                    }

                    // Player turn game loop
                    foreach (Player player in players)
                    {
                        GameState = GameState.WaitingForUserInput;
                        while (GameState == GameState.WaitingForUserInput)
                        {
                            PerformSingleTurn(player);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                outputProvider?.WriteLine(ex.ToString());
                outputProvider?.WriteLine("Error Starting Game.");
            }
        }
        public void StartNewRound()
        {
            currentPuzzle = new Puzzle();
            Turn = 1;
            Round++;
            display.ResetScreen();
            // update the game state
            GameState = GameState.RoundStarted;
        }





        public void PerformSingleTurn(Player player)
        {
            GameState = GameState.WaitingForUserInput;
            display.DrawPuzzle(currentPuzzle);
            display.DrawPlayerScore(player);
            display.DrawCurrentTurn(Turn, Round);
            display.DrawGuessedLetters(currentPuzzle.GuessedList);
            display.DrawPlayerPrompt("Type 1 to spin, 2 to solve, 3 to pass: ");

            bool turnComplete = false;

            while (!turnComplete)
            {
                bool isValid = false;
                while (!isValid)
                {
                    string action = inputProvider.Read();
                    switch (action)
                    {
                        case "1":
                            isValid = true;
                            HandleSpin(player);
                            break;
                        case "2":
                            isValid = true;
                            HandleSolve(player);
                            break;
                        case "3":
                            isValid = true;
                            EndTurn();
                            break;
                        default:
                            display.DrawPlayerPrompt("Get real. Try again. 1, 2, or 3: ");
                            break;
                    }
                }
                turnComplete = true;
            }
        }

        /// <summary>
        /// Ends the current player's turn
        /// </summary>
        private void EndTurn()
        {
            display.DrawPlayerPrompt("Ending Turn...");
            Thread.Sleep(2000);
            GameState = GameState.PlayerTurnEnded;
            Turn++;
            display.DrawCurrentTurn(Turn, Round);
            display.DrawGuessedLetters(currentPuzzle.GuessedList);
        }

        private void HandleSolve(Player player)
        {
            display.DrawPlayerPrompt("Oh really? You think you've got it all figured out? Enter your solution below:");
            string playerSolution = inputProvider.Read();
            bool isWinner = currentPuzzle.Solve(playerSolution);
            if (isWinner)
            {
                HandleWin(player);

            }
            else
            {
                display.DrawPlayerPrompt("Ouch, you were way off.");
                Thread.Sleep(2000);
                EndTurn();
            }
        }

        private void HandleWin(Player player)
        {
            HandleReveal();
            player.Score.AddScore(2000);
            display.DrawPlayerScore(player);
            display.DisplayWinner(player);
            display.DrawPlayerPrompt("You've done it! Congratulations! Press 1 to start a new round.");
            bool breakWinLoop = false;
            while (!breakWinLoop)
            {
                string input = inputProvider.Read();
                switch (input)
                {
                    case "1":
                        display.DrawPlayerPrompt("Starting new game...");
                        Thread.Sleep(1000);
                        GameState = GameState.RoundOver;
                        breakWinLoop = true;
                        breakGameLoop = true;
                        break;
                    default:
                        display.DrawPlayerPrompt("Invalid input. You're drunk on your own success. Try again... Press 1 to start a new round.");
                        break;
                }
            }
            StartGame(); ;
        }

        private void HandleReveal()
        {
            currentPuzzle.RevealSolution();
            display.DrawPuzzle(currentPuzzle);
        }

        private void HandleSpin(Player player)
        {
            // simulated wheel spin
            display.DrawPlayerPrompt("Spinning the Wheel! Good Luck!");
            Thread.Sleep(2000);
            display.DrawPlayerPrompt("The Wheel lands on...");
            int spinResult = wheel.Spin();
            Thread.Sleep(2000);

            // render the result
            display.DrawPlayerPrompt($"{spinResult}! Nice.");
            Thread.Sleep(2000);
            display.DrawPlayerPrompt("Time to guess a letter...");
            Thread.Sleep(2000);

            HandleLetterGuess(player, spinResult);
            EndTurn();
        }

        private void HandleLetterGuess(Player player, int wager)
        {
            display.DrawPlayerScore(player, wager);

            display.DrawPlayerPrompt($"Ok {player.Name}, this is for all the beans. What letter are you guessing? ");

            bool guessComplete = false;

            while (!guessComplete)
            {
                bool isValid = false;

                bool isSubmitted;
                int multiplier;
                string guess = inputProvider.Read();

                //validate
                do
                {
                    isValid = ValidateGuess(guess);
                    if (!isValid)
                    {
                        guess = inputProvider.Read();
                    }
                } while (!isValid);

                //submit
                GuessResult result = currentPuzzle.GuessLetter(guess);
                isSubmitted = result.UniqueLetter;
                multiplier = result.Multiplier;

                while (!isSubmitted)
                {
                    display.DrawPlayerPrompt($"The letter {guess} has already been guessed. Try again.");
                    string newGuess = inputProvider.Read();
                    do
                    {
                        isValid = ValidateGuess(newGuess);
                        if (!isValid)
                        {
                            newGuess = inputProvider.Read();
                        }
                    } while (!isValid);
                    result = currentPuzzle.GuessLetter(newGuess);
                    isSubmitted = result.UniqueLetter;
                    multiplier = result.Multiplier;
                }
                if (isSubmitted)
                {
                    //success!
                    if (result.InSolution)
                    {
                        string plural = multiplier > 1 ? "s" : "";
                        display.DrawPlayerPrompt($"Success! Your letter was in the word {multiplier} time{plural}.");
                        player.Score.AddScore(wager * multiplier);
                        display.DrawPlayerScore(player);
                        Thread.Sleep(2000);
                        if (result.IsSolved)
                        {
                            display.DrawPuzzle(currentPuzzle);
                            HandleWin(player);
                        }
                        else
                        {
                            display.DrawPuzzle(currentPuzzle);
                        }

                    }
                    else
                    {
                        display.DrawPlayerPrompt("Ouch. No dice. Better luck next time.");
                        Thread.Sleep(2000);
                    }


                    display.DrawPlayerScore(player);
                    display.DrawGuessedLetters(currentPuzzle.GuessedList);

                    guessComplete = true;
                }

            }
        }

        private bool ValidateGuess(string guess)
        {
            if (guess.Length == 0)
            {
                display.DrawPlayerPrompt("C'mon, you have to guess *something*. Give me a letter!");
                return false;
            }
            else if (guess.Length > 1)
            {
                display.DrawPlayerPrompt("Maybe I was unclear. Please limit your guesses to 1 letter at a time.");
                return false;
            }
            else if (ContainsNumeric(guess))
            {
                display.DrawPlayerPrompt("No numbers! Try again.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool ContainsNumeric(string guess)
        {
            if (Regex.IsMatch(guess, @"^\d+$"))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Optional logic to accept configuration options
        /// </summary>
        public void InitGame()
        {
            GameState = GameState.WaitingToStart;
            outputProvider.WriteLine("Welcome to Wheel of Fortune!");

            outputProvider.WriteLine("How many players are going to play? Press 1 or 2: ");
            string playerCountString = inputProvider.Read();
            bool isValid = ValidatePlayers(playerCountString);
            while (!isValid)
            {
                outputProvider.WriteLine("Invalid entry. Try again. 1 or 2?");
                playerCountString = inputProvider.Read();
                isValid = ValidatePlayers(playerCountString);
            }
            int playerCount = int.Parse(playerCountString);
            for (int i = 0; i < playerCount; i++)
            {
                outputProvider.WriteLine($"Player {i + 1}, Please enter your name:");
                string name = inputProvider.Read();
                Player player = new Player(name);
                players.Add(player);
            }

            outputProvider.WriteLine("Starting Game...");
            Thread.Sleep(1000);
            StartNewRound();
        }

        private bool ValidatePlayers(string numOfPlayers)
        {
            bool isValid = false;
            switch (numOfPlayers)
            {
                case "1":
                case "2":
                    isValid = true;
                    break;
                default:
                    break;
            }
            return isValid;
        }
    }
}
