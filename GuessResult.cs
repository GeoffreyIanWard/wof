namespace LeapWoF
{
    public class GuessResult
    {
        public int Multiplier { get; set; }
        public bool UniqueLetter { get; set; }

        public bool InSolution { get; set; }

        public bool IsSolved { get; set; }

        public GuessResult(bool uniqueLetter, int multiplier, bool inSolution, bool isSolved)
        {
            UniqueLetter = uniqueLetter;
            Multiplier = multiplier;
            InSolution = inSolution;
            IsSolved = isSolved;
        }
    }
}