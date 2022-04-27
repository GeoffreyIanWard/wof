using System;
using System.Collections.Generic;

namespace LeapWoF
{
    class Encyclopedia : IEncyclopedia
    {
        //BG
        //should this be public?
        public List<string> Words = new List<string>();


        //BG
        //constructor function 
        //not sure if we need to include this parameter here
        public Encyclopedia()
        {
            SpinUpList(Words);
        }

        private void SpinUpList(List<string> words)
        {

            words.Add("DANGEROUS");
            words.Add("DIABOLICAL");
            words.Add("GENIUS");
            words.Add("ELEPHANT");
            words.Add("LASER");
            words.Add("BOOKSHELF");
            words.Add("RACECAR");
            words.Add("MISSISSIPPI");
            words.Add("CRUMBLE");
            words.Add("TENNESSEE");
            words.Add("LOCATION");
            words.Add("BALLOON");
            words.Add("ELBOW");
            words.Add("SHOGUN");
            words.Add("ALLIGATOR");
        }

        public Encyclopedia(List<string> _words)
        {
            Words = _words;
        }
        //BG
        //this method is randomizing our list of words, 
        //and will return the value at that randomly chosen index
        public string GenerateWord()
        {
            var random = new Random();
            int randWordIndex = random.Next(Words.Count);

            return Words[randWordIndex];
        }
    }
}