/* Collatz Sequence:
 * n → n/2 (n is even)
 * n → 3n + 1 (n is odd)
 */

using System;
using System.Collections.Generic;

namespace ConsoleApp_Sandbox.Puzzles.LongestCollatz
{
    public class LongestCollatzPuzzle
    {
        const int MAX = 1000000;
        private const int END_VALUE = 1;

        public static void Run()
        {
            var pathLengthCache = new Dictionary<int, int>();
            
            var maxChainLength = 0;
            var maxChainSeed = -1;
            var currentChainLength = 0;
            var currentValue = 0;
            for (var currentSeed = 0; currentSeed < MAX; currentSeed++)
            {
                currentValue = currentSeed;
                currentChainLength = 0;
                
                while (currentValue > END_VALUE)
                {
                    if (pathLengthCache.ContainsKey(currentValue))
                    {
                        currentChainLength += pathLengthCache[currentValue];
                        currentValue = END_VALUE;
                    }
                    else
                    {
                        currentChainLength++;
                        currentValue = getNextValue(currentValue);
                    }
                }
                // currentValue == END_VALUE -> completed number
                // add to path length cache
                pathLengthCache.Add(currentSeed, currentChainLength);
                
                // update longest chain
                if (currentChainLength > maxChainLength)
                {
                    maxChainLength = currentChainLength;
                    maxChainSeed = currentSeed;
                }
            }
            
            Console.WriteLine($"Longest chain between 0 and {MAX.ToString()} - seed: {maxChainSeed} - value: {maxChainLength}");
        }

        static int getNextValue(int currentValue)
        {
            return currentValue % 2 == 0 ? currentValue / 2 : 3 * currentValue + 1;
        }
    }
}