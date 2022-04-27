using System;
using System.Collections.Generic;

namespace LeapWoF
{
    public class Puzzle
    {
        public string Solution { get; private set; }

        private List<char> charSolution;
        public List<char> DashedSolution
        {
            get;
            private set;
        }

        // Use this to display the dashed solution as a string
        public string GetObscuredString()
        {
            string charString = new string(DashedSolution.ToArray());
            return charString;
        }

        public List<char> GuessedList { get; private set; }
        public bool IsSolved { get; internal set; }

        private IEncyclopedia encyclopedia;

        public Puzzle()
        {
            encyclopedia = new Encyclopedia();
            Solution = encyclopedia.GenerateWord();
            charSolution = new List<char>(Solution.ToCharArray());
            DashedSolution = new List<char>();
            GuessedList = new List<char>();
            foreach (char ch in Solution)
            {
                DashedSolution.Add('-');
            }
        }

        public void RevealSolution()
        {
            List<char> undashed = new List<char>(Solution.ToCharArray());
            DashedSolution = undashed;
        }
        public bool Solve(string userInput)
        {
            if (userInput.ToUpper() == Solution.ToUpper())
            {
                RevealSolution();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// handles a letter guess, return is a bool representing whether the letter has been guessed (or not)
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        public GuessResult GuessLetter(string userInput)
        {
            bool uniqueLetter = false;
            bool inSolution = false;
            bool isSolved = false;
            int multiplier = 0;


            char[] letters = userInput.ToUpper().ToCharArray();
            char letter = letters[0];

            try
            {
                // if it's a valid letter, updates the current dashed solution;
                if (Solution.Contains(letter.ToString()))
                {
                    inSolution = true;
                    for (int i = 0; i < Solution.Length; i++)
                    {
                        if (Solution[i] == letter)
                        {
                            DashedSolution[i] = letter;
                            multiplier++;
                        }
                    }
                }

                // handles redundant guesses
                if (!GuessedList.Contains(letter))
                {
                    GuessedList.Add(letter);
                    uniqueLetter = true;
                }

                //have we solved the puzzle with our guess?
                string currentDashedString = String.Join("", DashedSolution);
                if (Solution == currentDashedString)
                {
                    isSolved = true;
                }

                return new GuessResult(uniqueLetter, multiplier, inSolution, isSolved);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new GuessResult(uniqueLetter, multiplier, inSolution, isSolved);
            }
        }


    }
}