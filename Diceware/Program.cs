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
        private static Dictionary<int, string> wordList = new Dictionary<int, string>();
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
            int rowNo;
            Int32.TryParse(wordArr, out rowNo);
            Console.Write("{0} ", wordList[rowNo]);
        }

        static Dictionary<int, string> ReadWordList(string filename)
        {
            if(String.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            var rdr = new StreamReader(filename);
            var wordlist = new Dictionary<int, string>();
            string line;
            while ((line = rdr.ReadLine()) != null)
            {
                if (Regex.IsMatch(line, @"^\d"))
                {
                    var split = line.Split('\t');
                    int lineId;
                    Int32.TryParse(split[0], out lineId);
                    wordlist.Add(lineId, split[1]);
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
