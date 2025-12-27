using System;

namespace MasterMindCliGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== MASTERMIND ===");
            Console.WriteLine("Autor: Kacper Adamczyk");

            while (true)
            {
                Console.WriteLine("\n--- MENU GŁÓWNE ---");
                Console.WriteLine("1. Gra Klasyczna (Zadanie 1 i 2) [4 kolory, kod dł. 4]");
                Console.WriteLine("2. Gra Niestandardowa (Zadanie 3 i 5) [Wybór parametrów]");
                Console.WriteLine("3. Wyjście");
                Console.Write("Twój wybór: ");

                var key = Console.ReadKey().KeyChar;
                Console.WriteLine();

                if (key == '3') break;

                // Domyślne ustawienia (Klasyczne)
                int length = 4;
                int colors = 6;
                bool useNumbers = false;

                if (key == '2')
                {
                    (length, colors, useNumbers) = ConfigureGame();
                }

                MasterMindEngine game = new MasterMindEngine(length, colors, useNumbers);

                Console.WriteLine("\nKto ma zgadywać?");
                Console.WriteLine("1. Człowiek");
                Console.WriteLine("2. Komputer");
                var modeKey = Console.ReadKey().KeyChar;
                Console.WriteLine();

                if (modeKey == '2')
                {
                    PlayComputerGuesserMode(game);
                }
                else
                {
                    PlayHumanGuesserMode(game);
                }
            }
        }
        // Konfiguracan trybu gry z parametrami zadanie 3 i 5
        static (int length, int colors, bool useNumbers) ConfigureGame()
        {
            Console.WriteLine("\n--- KONFIGURACJA (Zadanie 3 i 5) ---");

            Console.WriteLine("Rodzaj znaków:");
            Console.WriteLine("k - Kolory (klasyczne)");
            Console.WriteLine("c - Cyfry (0-9) [Zadanie 5]");
            bool useNumbers = Console.ReadKey().KeyChar == 'c';
            Console.WriteLine();

            int maxItems = useNumbers ? 10 : 8;
            int colorsCount = GetIntInput($"Liczba dostępnych znaków (6-{maxItems}): ", 6, maxItems);
            int length = GetIntInput("Długość kodu (4-6): ", 4, 6);

            return (length, colorsCount, useNumbers);
        }
        static void PlayHumanGuesserMode(MasterMindEngine game)
        {
            Console.WriteLine($"\nZGADNIJ KOD! Długość: {game.CodeLength}, Znaków: {game.ValidCharacters.Length}");
            Console.WriteLine($"Dostępne znaki: {string.Join(", ", game.ValidCharacters)}");

            while (!game.IsGameOver)
            {
                Console.WriteLine();
                Console.Write($"Próba {game.AttemptsUsed + 1}/{game.MaxAttempts}. Podaj kod: ");
                string input = Console.ReadLine();

                if (!game.IsInputValid(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Błąd! Sprawdź długość i dostępne znaki.");
                    Console.ResetColor();
                    continue;
                }

                var result = game.EvaluateGuess(input);
                PrintResult(input, result.exact, result.inexact);
            }

            if (game.IsGameWon)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nGRATULACJE! Wygrałeś!");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nPRZEGRAŁEŚ. Kod: {game.GetSecretCodeIfLost()}");
            }
            Console.ResetColor();
        }

        static void PlayComputerGuesserMode(MasterMindEngine game)
        {
            Console.WriteLine("\nKOMPUTER ZGADUJE");
            Console.WriteLine($"Pomyśl kod o długości {game.CodeLength} używając znaków: {string.Join("", game.ValidCharacters)}");
            Console.WriteLine("Naciśnij ENTER gdy gotowy...");
            Console.ReadLine();

            // Przekazujemy parametry do Solvera!
            ComputerSolver solver = new ComputerSolver(game.CodeLength, game.ValidCharacters);
            int attempts = 0;

            while (true)
            {
                attempts++;
                string guess = solver.GetNextGuess();

                if (guess == null)
                {
                    Console.WriteLine("BŁĄD: Lista pusta (oszukiwałeś?).");
                    return;
                }

                Console.WriteLine($"\nRuch komputera #{attempts}:");
                PrintGuessOnly(guess);
                Console.WriteLine($" (Możliwości: {solver.PossibleCount})");

                Console.WriteLine("Oceń:");
                int exact = GetIntInput("  -> Trafienia dokładne (X): ", 0, game.CodeLength);

                if (exact == game.CodeLength)
                {
                    Console.WriteLine("Komputer wygrał!");
                    break;
                }

                int maxInexact = game.CodeLength - exact;
                int inexact = GetIntInput($"  -> Trafienia niedokładne (O, max {maxInexact}): ", 0, maxInexact);

                solver.ProcessFeedback(guess, exact, inexact);
            }
        }

        // Helper for getting numbers
        static int GetIntInput(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int val) && val >= min && val <= max)
                {
                    return val;
                }
                Console.WriteLine($"Błąd. Wpisz liczbę od {min} do {max}.");
            }
        }

        static void PrintResult(string guess, int exact, int inexact)
        {
            PrintGuessOnly(guess);
            Console.Write(" | Wynik: ");
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < exact; i++) Console.Write("X ");
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < inexact; i++) Console.Write("O ");
            Console.ResetColor();
            Console.Write($" ({exact}, {inexact})");
        }

        static void PrintGuessOnly(string guess)
        {
            foreach (char c in guess)
            {
                if (char.IsDigit(c)) Console.ForegroundColor = ConsoleColor.Cyan;
                else if (c == 'r') Console.ForegroundColor = ConsoleColor.Red;
                else if (c == 'g') Console.ForegroundColor = ConsoleColor.Green;
                else if (c == 'b') Console.ForegroundColor = ConsoleColor.Blue;
                else if (c == 'y') Console.ForegroundColor = ConsoleColor.Yellow;
                else Console.ForegroundColor = ConsoleColor.Magenta;

                Console.Write(c);
            }
            Console.ResetColor();
        }
    }
}
