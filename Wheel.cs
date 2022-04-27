using System;

namespace LeapWoF
{


    public class Wheel
    {
        private static int[] wedges = { 100, 200, 300, 400, 500, 600 };

        private int wedgeCount = wedges.Length;

        private Random random = new Random();

        public Wheel()
        {

        }

        public int Spin()
        {
            int index = random.Next(wedgeCount);
            int points = wedges[index];
            return points;
        }
    }
}