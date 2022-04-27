using LeapWof;

namespace LeapWoF
{
    public class Player
    {
        public Score Score { get; private set; }
        public string Name { get; private set; }

        public Player(string name)
        {
            Name = name;
            Score = new Score();
        }

    }
}