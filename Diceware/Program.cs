using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Diceware
{
    class Program
    {
        private static readonly RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        private static Dictionary<string, string> wordList = new Dictionary<string, string>();
        private const int wordCount = 8;
        static void Main(string[] args)
        {

            wordList = ReadWordList(@".\beale.wordlist.asc.txt");
            Console.WriteLine("Press enter for a new passphrase...");
            do
            {
                GetPassPhrase(wordCount);
            } while (Console.ReadLine() != null);
 
            rngCsp.Dispose();
        }

        private static void GetPassPhrase(int noOfWords)
        {
            for (var i = 0; i < noOfWords; i++)
            {
                string wordArr = String.Empty;
                for (var j = 0; j < 5; j++)
                {
                    var roll = RollDice();
                    wordArr += roll;
                }
                PrintFoundWord(wordArr);
            }
            Console.WriteLine();
        }

        private static void PrintFoundWord(string wordArr)
        {
            Console.Write("{0} ", wordList[wordArr]);
        }

        static Dictionary<string, string> ReadWordList(string filename)
        {
            if(String.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            var rdr = new StreamReader(filename);
            var wordlist = new Dictionary<string, string>();
            string line;
            while ((line = rdr.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, @"^\d"))
                {
                    var split = line.Split('\t');
                    wordlist.Add(split[0], split[1]);
                }
            }
            return wordlist;
        }

        public static byte RollDice()
        {
            var randomNumber = new byte[1];
            do
            {
                rngCsp.GetBytes(randomNumber);
            }
            while (!IsFairRoll(randomNumber[0], 6));
            return (byte)((randomNumber[0] % 6) + 1);
        }

        private static bool IsFairRoll(byte roll, byte numSides)
        {
            int fullSetsOfValues = Byte.MaxValue / numSides;
            return roll < numSides * fullSetsOfValues;
        }
    }
}
