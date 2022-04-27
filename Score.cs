namespace LeapWof
{
    public class Score
    {
        public int CurrentValue { get; set; }

        public Score()
        {
            CurrentValue = 0;
        }

        public int AddScore(int number)
        {
            CurrentValue += number;
            if (CurrentValue < 0) { CurrentValue = 0; }
            return CurrentValue;
        }

        public void ClearScore()
        {
            CurrentValue = 0;
        }

        public int GetScore()
        {
            return CurrentValue;
        }
    }
}