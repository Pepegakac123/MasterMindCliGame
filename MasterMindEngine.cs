using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMindCliGame
{
    public class MasterMindEngine
    {
        // Available colors: r-red, y-yellow, g-green, b-blue, m-magenta, c-cyan
        public readonly char[] ValidColors = { 'r', 'y', 'g', 'b', 'm', 'c' };

        private const int CodeLength = 4;
        private const int MaxAttempts = 9;

        private char[] _secretCode;
        public int AttemptsUsed { get; private set; }
        public bool IsGameWon { get; private set; }

        public bool IsGameOver => IsGameWon || AttemptsUsed >= MaxAttempts;

        public MasterMindEngine()
        {
            StartNewGame();
        }

        public void StartNewGame()
        {
            _secretCode = GenerateSecretCode();
            AttemptsUsed = 0;
            IsGameWon = false;
        }

        private char[] GenerateSecretCode()
        {
            var random = new Random();
            var code = new char[CodeLength];
            for (int i = 0; i < CodeLength; i++)
            {
                code[i] = ValidColors[random.Next(ValidColors.Length)];
            }
            // debug (see the secret code)
            Console.WriteLine($"DEBUG: {new string(code)}");
            return code;
        }

        // Exact = Black (correct color, correct place)
        // Inexact = White (correct color, wrong place)
        public (int exact, int inexact) EvaluateGuess(string guess)
        {
            char[] guessChars = guess.ToLower().ToCharArray();

            int exact = 0;
            int inexact = 0;

            // Helper arrays to track which positions are already matched
            bool[] codeUsed = new bool[CodeLength];
            bool[] guessUsed = new bool[CodeLength];

            // Check for Exact matches
            for (int i = 0; i < CodeLength; i++)
            {
                if (guessChars[i] == _secretCode[i])
                {
                    exact++;
                    codeUsed[i] = true;
                    guessUsed[i] = true;
                }
            }

            // Check for Inexact matches
            for (int i = 0; i < CodeLength; i++)
            {
                if (guessUsed[i]) continue; // Already matched as exact

                for (int j = 0; j < CodeLength; j++)
                {
                    if (!codeUsed[j] && guessChars[i] == _secretCode[j])
                    {
                        inexact++;
                        codeUsed[j] = true;
                        break;
                    }
                }
            }

            AttemptsUsed++;

            if (exact == CodeLength)
            {
                IsGameWon = true;
            }

            return (exact, inexact);
        }

        public bool IsInputValid(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length != CodeLength) return false;

            foreach (char c in input.ToLower())
            {
                if (!ValidColors.Contains(c)) return false;
            }
            return true;
        }

        public string GetSecretCodeIfLost()
        {
            if (IsGameOver) return new string(_secretCode);
            return "????";
        }
    }
}
