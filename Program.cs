using System;

namespace MasterMindCliGame
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== MASTERMIND ===");
            Console.WriteLine("Autor: Kacper Adamczyk");
            Console.WriteLine("Wybierz tryb gry:");
            Console.WriteLine("1. Ty zgadujesz kod komputera");
            Console.WriteLine("2. Komputer zgaduje Twój kod");
            Console.Write("Twój wybór (1/2): ");

            var key = Console.ReadKey().KeyChar;
            Console.WriteLine("\n");

            if (key == '2')
            {
                PlayComputerGuesserMode();
            }
            else
            {
                PlayHumanGuesserMode();
            }

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby zakończyć...");
            Console.ReadKey();
        }

        static void PlayHumanGuesserMode()
        {
            MasterMindEngine game = new MasterMindEngine();
            Console.WriteLine("=== TRYB: CZŁOWIEK ZGADUJE ===");
            Console.WriteLine($"Dostępne kolory: {string.Join(", ", game.ValidColors)}");
            Console.WriteLine("Przykład: rrgb");
            Console.WriteLine($"Liczba prób: {game.GetMaxAttempts}");

            while (!game.IsGameOver)
            {
                Console.WriteLine();
                Console.Write($"Próba {game.AttemptsUsed + 1}. Podaj kod: ");
                string input = Console.ReadLine();

                if (!game.IsInputValid(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Błąd! Użyj 4 znaków z: " + string.Join(", ", game.ValidColors));
                    Console.ResetColor();
                    continue;
                }

                var result = game.EvaluateGuess(input);
                PrintResult(input, result.exact, result.inexact);
            }

            if (game.IsGameWon)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nGRATULACJE! Wygrałeś w {game.AttemptsUsed} prób.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nKONIEC GRY.");
                Console.WriteLine($"Sekretny kod to: {game.GetSecretCodeIfLost()}");
                Console.ResetColor();
            }
        }

        static void PlayComputerGuesserMode()
        {
            Console.WriteLine("=== TRYB: KOMPUTER ZGADUJE ===");
            Console.WriteLine("Pomyśl o kodzie (4 kolory z: r, y, g, b, m, c).");
            Console.WriteLine("Zapisz go sobie na kartce.");
            Console.WriteLine("Naciśnij ENTER, gdy będziesz gotowy.");
            Console.ReadLine();

            ComputerSolver solver = new ComputerSolver();
            int attempts = 0;

            while (true)
            {
                attempts++;
                string guess = solver.GetNextGuess();
                if (guess == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nBŁĄD: Lista możliwych kodów jest pusta!");
                    Console.WriteLine("Prawdopodobnie podałeś błędne oceny w poprzednich krokach.");
                    Console.ResetColor();
                    return;
                }

                Console.WriteLine($"\nRuch komputera #{attempts}:");
                PrintGuessOnly(guess);
                Console.WriteLine($" (Możliwe kody: {solver.PossibleCount})");
                Console.WriteLine("Oceń ten ruch:");
                int exact = GetIntInput("  -> Liczba trafień dokładnych (Czarne/X): ", 0, 4);

                if (exact == 4)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\nKomputer odgadł Twój kod w {attempts} ruchach!");
                    Console.ResetColor();
                    break;
                }
                int maxInexact = 4 - exact;
                int inexact = GetIntInput($"  -> Liczba trafień niedokładnych (Białe/O, max {maxInexact}): ", 0, maxInexact);
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
                ConsoleColor color = ConsoleColor.Gray;
                switch (c)
                {
                    case 'r': color = ConsoleColor.Red; break;
                    case 'y': color = ConsoleColor.Yellow; break;
                    case 'g': color = ConsoleColor.Green; break;
                    case 'b': color = ConsoleColor.Blue; break;
                    case 'm': color = ConsoleColor.Magenta; break;
                    case 'c': color = ConsoleColor.Cyan; break;
                }
                Console.ForegroundColor = color;
                Console.Write(c);
            }
            Console.ResetColor();
        }
    }
}
