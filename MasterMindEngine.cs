using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterMindCliGame
{
    public class MasterMindEngine
    {
        private readonly char[] _colorAlphabet = { 'r', 'y', 'g', 'b', 'm', 'c', 'w', 'k' }; // do 8 kolorów
        private readonly char[] _digitAlphabet = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public char[] ValidCharacters { get; private set; }
        public int CodeLength { get; private set; }
        public int MaxAttempts { get; private set; }

        private char[] _secretCode;
        public int AttemptsUsed { get; private set; }
        public bool IsGameWon { get; private set; }
        public bool IsGameOver => IsGameWon || AttemptsUsed >= MaxAttempts;

        public MasterMindEngine() : this(4, 6, false) // Domyślny zadanie 1 i 2 - zwykłe zasady
        {
        }
        // Konstruktor parametryzowany (dla Zadania 3 i 5)
        public MasterMindEngine(int length, int charCount, bool useNumbers)
        {
            CodeLength = length;
            MaxAttempts = 9 + (length - 4) * 2;

            if (useNumbers)
            {
                // Zadanie 5: Cyfry
                ValidCharacters = _digitAlphabet.Take(charCount).ToArray();
            }
            else
            {
                // Zadanie 3: Kolory (6..8)
                ValidCharacters = _colorAlphabet.Take(charCount).ToArray();
            }

            StartNewGame();
        }
        public void StartNewGame()
        {
            _secretCode = GenerateSecretCode();
            AttemptsUsed = 0;
            IsGameWon = false;
        }

        public int GetMaxAttempts => MaxAttempts;

        private char[] GenerateSecretCode()
        {
            var random = new Random();
            var code = new char[CodeLength];
            for (int i = 0; i < CodeLength; i++)
            {
                code[i] = ValidCharacters[random.Next(ValidCharacters.Length)];
            }
            // Debug:
            // Console.WriteLine($"DEBUG: {new string(code)}");
            return code;
        }
        public (int exact, int inexact) EvaluateGuess(string guess)
        {
            var result = CalculateScore(new string(_secretCode), guess);

            AttemptsUsed++;
            if (result.exact == CodeLength) IsGameWon = true;

            return result;
        }

        // Will be used by ComputerSolver to filter possibilites
        public static (int exact, int inexact) CalculateScore(string code, string guess)
        {
            int len = code.Length;
            char[] codeChars = code.ToLower().ToCharArray();
            char[] guessChars = guess.ToLower().ToCharArray();

            int exact = 0;
            int inexact = 0;

            bool[] codeUsed = new bool[len];
            bool[] guessUsed = new bool[len];

            // Exact matches
            for (int i = 0; i < len; i++)
            {
                if (guessChars[i] == codeChars[i])
                {
                    exact++;
                    codeUsed[i] = true;
                    guessUsed[i] = true;
                }
            }

            // Inexact matches
            for (int i = 0; i < len; i++)
            {
                if (guessUsed[i]) continue;

                for (int j = 0; j < len; j++)
                {
                    if (!codeUsed[j] && guessChars[i] == codeChars[j])
                    {
                        inexact++;
                        codeUsed[j] = true;
                        break;
                    }
                }
            }

            return (exact, inexact);
        }

        public bool IsInputValid(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length != CodeLength) return false;

            foreach (char c in input.ToLower())
            {
                if (!ValidCharacters.Contains(c)) return false;
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
