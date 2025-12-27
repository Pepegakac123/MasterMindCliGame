using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMindCliGame
{
    // Implementation of Strategy 3
    public class ComputerSolver
    {
        private List<string> _possibleCodes;
        private Random _random = new Random();
        private int _codeLength;
        private char[] _validChars;
        public int PossibleCount => _possibleCodes.Count;

        public ComputerSolver(int codeLength, char[] validChars)
        {
            _codeLength = codeLength;
            _validChars = validChars;
            _possibleCodes = GenerateAllCodes();
        }

        // Generate list  of all codes
        private List<string> GenerateAllCodes()
        {
            var list = new List<string>();
            GenerateCodesRecursive("", list);
            return list;
        }
        private void GenerateCodesRecursive(string currentPrefix, List<string> resultList)
        {
            if (currentPrefix.Length == _codeLength)
            {
                resultList.Add(currentPrefix);
                return;
            }
            foreach (char c in _validChars)
            {
                GenerateCodesRecursive(currentPrefix + c, resultList);
            }
        }
        // Get the next best guess
        public string GetNextGuess()
        {
            if (_possibleCodes.Count == 0) return null;

            // Pick any element from list like in the srategy 3
            int index = _random.Next(_possibleCodes.Count);
            return _possibleCodes[index];
        }

        // Evaluate and Update List L
        public void ProcessFeedback(string guess, int exact, int inexact)
        {
            // Remove codes that does not produce the same (exact, inexact) variations
            for (int i = _possibleCodes.Count - 1; i >= 0; i--)
            {
                string candidate = _possibleCodes[i];
                var result = MasterMindEngine.CalculateScore(candidate, guess);

                // Compare candidate vs the guess we just made
                if (result.exact != exact || result.inexact != inexact)
                {
                    _possibleCodes.RemoveAt(i);
                }
            }
        }
    }
}
