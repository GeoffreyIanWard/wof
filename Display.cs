using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace LeapWoF
{
    class Display
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public int Center { get; private set; }

        private int rectangleHeight = 10;
        private ConsoleOutputProvider outputProvider;

        public Display()
        {
            outputProvider = new ConsoleOutputProvider();

            Width = Console.WindowWidth;
            Height = Console.WindowHeight;
            Center = Width / 2;
        }

        private void WriteAt(string str, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(str);
        }


        /// <summary>
        /// Draws a horizontal line of asterisks.
        /// </summary>
        private void DrawHLine(string ch, int y)
        {
            StringBuilder hLine = new StringBuilder();
            for (int i = 0; i < Width; i++)
            {
                hLine.Append(ch);
            }

            WriteAt(hLine.ToString(), 0, y);
        }

        private void DrawHLine(char ch, int y)
        {
            StringBuilder hLine = new StringBuilder();
            for (int i = 0; i < Width; i++)
            {
                hLine.Append(ch);
            }

            WriteAt(hLine.ToString(), 0, y);
        }

        private void DrawPuzzleRectangle()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Cyan;
            // horizontal line
            DrawHLine('*', 0);
            DrawHLine('*', 1);

            // rectangle verticals
            for (int i = 1; i <= rectangleHeight - 1; i++)
            {
                WriteAt("**", 0, i);
                WriteAt("**", Width - 2, i);
            }

            // horizontal line
            DrawHLine('*', rectangleHeight - 1);
            DrawHLine('*', rectangleHeight);

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Renders the current Puzzle state to the console
        /// </summary>
        /// <param name="currentPuzzle"></param>
        public void DrawPuzzle(Puzzle currentPuzzle)
        {

            // draw 4k high definition graphics
            DrawPuzzleRectangle();

            // write intro line
            string intro = "The puzzle is:";
            WriteAt(
                intro,
                Center - (intro.Length / 2),
                (rectangleHeight / 2) - (rectangleHeight / 4)
                );

            // write current puzzle state
            List<char> puzzle = currentPuzzle.DashedSolution;
            string displayPuzzle = String.Join(" | ", puzzle);
            WriteAt(
                displayPuzzle,
                Center - (displayPuzzle.Length / 2),
                (rectangleHeight / 2) + (rectangleHeight / 4) - 1
                );
        }

        public void DrawPlayerScore(Player player, int wager = 0, bool noName = false)
        {
            int nameHeight = rectangleHeight + 1;
            int scoreHeight = rectangleHeight + 2;
            ClearPlayerScore(nameHeight, scoreHeight, noName);
            if (!noName)
            {
                WriteAt($"Current Player: {player.Name}", 1, nameHeight);
            }
            WriteAt($"Current Score: {player.Score.CurrentValue}", 1, scoreHeight);

            string localWager = wager == 0 ? "" : wager.ToString();
            WriteAt($"Current Wager: {localWager}", 30, scoreHeight);
            Console.ForegroundColor = ConsoleColor.Red;
            DrawHLine('-', scoreHeight + 1);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private void ClearPlayerScore(int nameHeight, int scoreHeight, bool noName = false)
        {
            if (!noName)
            {
                WriteAt("                                ", 17, nameHeight);
            }
            WriteAt("         ", 16, scoreHeight);
            WriteAt("   ", 45, scoreHeight);
        }

        public void DrawCurrentTurn(int turn, int round)
        {
            int turnHeight = rectangleHeight + 2;
            WriteAt($"Current Turn: {turn}", 60, turnHeight);
            WriteAt($"Current Round: {round}", 3, 3);
        }

        public void DrawGuessedLetters(List<char> guesses)
        {
            string guessStr = String.Join(" . ", guesses);
            int firstLine = rectangleHeight + 1;
            int secondLine = rectangleHeight + 2;
            if (guesses.Count < 13)
            {
                WriteAt($"Guessed Letters: {guessStr}", 60, firstLine);
            }
            else
            {
                List<char> first = new List<char>();
                List<char> second = new List<char>();
                for (int i = 0; i < guesses.Count - 1; i++)
                {
                    if (i % 2 == 0)
                    {
                        first.Add(guesses[i]);
                    }
                    else
                    {
                        second.Add(guesses[i]);
                    }
                }
                string firstStr = String.Join(" . ", first);
                string secondStr = String.Join(" . ", second);
                WriteAt($"Guessed Letters: {firstStr}", 60, firstLine);
                WriteAt(secondStr, 77, secondLine);
            }
        }

        public void ResetScreen()
        {
            outputProvider.Clear();
        }

        internal void DrawPlayerPrompt(string playerString)
        {
            ClearPlayerPrompt();
            WriteAt(playerString, 1, 16);
        }

        internal void ClearPlayerPrompt()
        {
            DrawHLine(" ", 16);
        }

        internal void DisplayWinner(Player player)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            // draw star box
            for (int i = 0; i <= rectangleHeight; i++)
            {
                if (i % 2 == 0)
                {
                    DrawHLine("* ", i);
                }
                else
                {
                    DrawHLine(" *", i);
                }

                Thread.Sleep(100);
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            //draw winner box


            string kingString = "The king is dead! Long live the king!";
            Console.ForegroundColor = ConsoleColor.White;

            int innerWidth = kingString.Length > player.Name.Length ? kingString.Length : player.Name.Length;
            int padding = innerWidth / 5;
            int hPadding = innerWidth + padding * 2;

            StringBuilder blankLine = new StringBuilder();
            for (int i = 0; i <= hPadding; i++)
            {
                blankLine.Append(' ');
            }

            StringBuilder kingBuilder = new StringBuilder(kingString);
            for (int i = 0; i <= padding; i++)
            {
                kingBuilder.Append(" ");
                kingBuilder.Insert(0, " ");
            }
            string displayKing = kingBuilder.ToString().Remove(kingBuilder.Length - 1, 1);

            int namePadding = (hPadding - player.Name.Length) / 2;
            StringBuilder playerBuilder = new StringBuilder(player.Name);
            for (int i = 0; i <= namePadding; i++)
            {
                playerBuilder.Append(" ");
                playerBuilder.Insert(0, " ");
            }
            string displayName = playerBuilder.ToString().Remove(playerBuilder.Length - 1, 1); ;

            string scoreString = $" Score: {player.Score.CurrentValue}                                        ";
            int scorePadding = (hPadding - scoreString.Length) / 2;
            StringBuilder scoreBuilder = new StringBuilder(scoreString);
            for (int i = 0; i <= scorePadding; i++)
            {
                playerBuilder.Append(" ");
                playerBuilder.Insert(0, " ");
            }
            string displayScore = scoreBuilder.ToString();

            int xStart = Center - (hPadding / 2);
            int yStart = rectangleHeight / 4;

            int carriageReturn = yStart;

            WriteAt(blankLine.ToString(), xStart, carriageReturn++);
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteAt(displayKing, xStart, carriageReturn++);
            Console.ForegroundColor = ConsoleColor.White;
            WriteAt(blankLine.ToString(), xStart, carriageReturn++);
            Console.ForegroundColor = ConsoleColor.Green;
            WriteAt(displayName, xStart, carriageReturn++);
            Console.ForegroundColor = ConsoleColor.White;
            WriteAt(blankLine.ToString(), xStart, carriageReturn++);
            WriteAt(displayScore, xStart, carriageReturn++);
            WriteAt(blankLine.ToString(), xStart, carriageReturn++);
        }


    }
}
