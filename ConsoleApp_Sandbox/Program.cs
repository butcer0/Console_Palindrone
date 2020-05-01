using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ConsoleApp_Sandbox
{
    /// <summary>
    /// Palindrone Puzzle
    /// </summary>
    class Program
    {
        const string EXAMPLE = "cecarar";
        const string EXAMPLE_2 = "cecarara";
        const string IMPOSSIBLE = "-1";
        
        static void Main(string[] args)
        {
            Console.WriteLine(generatePalindrone(EXAMPLE));
            Console.WriteLine(generatePalindrone(EXAMPLE_2));
        }

        private static string generatePalindrone(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return IMPOSSIBLE;
            }
            var isEven = (input?.Length ?? 0) % 2 == 0 ;
            var letters = new Dictionary<string, int>();
            
            for (var inputIndex = 0; inputIndex < input.Length; inputIndex++)
            {
                var currentInputLetter = input[inputIndex].ToString();
                // letters.Add(currentInputLetter, letters.ContainsKey(currentInputLetter)? letters[currentInputLetter] + 1 : 1);
                if (letters.ContainsKey(currentInputLetter))
                {
                    letters[currentInputLetter] = letters[currentInputLetter] + 1;
                }
                else
                {
                    letters.Add(currentInputLetter, 1);
                }
            }

            string center = null;
            try
            {
                center = findCenterOrInvalid(letters, isEven);
            }
            catch (ArgumentException e)
            {
                return IMPOSSIBLE;
            }

            var inputLength = input.Length;
            var outputArray = new string[inputLength];
            outputArray[inputLength / 2] = center;
            
            foreach (var x in letters.Select((letter, index) => new {letter, index}))
            {
                if (x.letter.Value % 2 == 1)
                {
                    continue;
                }

                outputArray[x.index] = x.letter.Key;
                outputArray[((outputArray.Length - 1) - x.index)] = x.letter.Key;
            }

            return string.Join(string.Empty, outputArray);
        }

        /// <summary>
        /// Find center or determine invalid
        /// </summary>
        /// <returns>string :  odd valid center
        /// empty string : even and valid
        /// IMPOSSIBLE : invalid palindrone
        /// </returns>
        private static string findCenterOrInvalid(Dictionary<string,int> letters, bool isEven)
        {
            var centerCount = 0;
            string center = String.Empty;
            var letterCenterValue = 0;
            foreach (var x in letters.Select((letter, lIndex) => new {letter, lIndex}))
            {
                letterCenterValue = ((x.letter.Value % 2 == 0) ? 0 : 1);
                centerCount += letterCenterValue;
                center = x.letter.Key;
            }

            if (isEven)
            {
                if (centerCount > 0)
                {
                    throw new ArgumentException("Even word has center");
                }
            }
            else
            {
                if (centerCount > 1)
                {
                    throw new ArgumentException("Odd word has more than one center");
                }
            }

            return center;
        }
    }
}