using System;

namespace MasterMindCliGame
{
    class Program
    {
        static void Main(string[] args)
        {
            MasterMindEngine game = new MasterMindEngine();

            Console.WriteLine("=== GRA MASTERMIND ===");
            Console.WriteLine("Autor: Kacper Adamczyk");
            Console.WriteLine("Odgadnij sekretny kod (4 kolory).");
            Console.WriteLine($"Dostępne kolory: {string.Join(", ", game.ValidColors)}");
            Console.WriteLine("Przykład formatu: rrgb (red, red, green, blue)");
            Console.WriteLine($"Maksymalna liczba prób: {game.GetMaxAttempts}");

            while (!game.IsGameOver)
            {
                Console.WriteLine();
                Console.Write($"Próba {game.AttemptsUsed + 1}. Podaj kod: ");
                string input = Console.ReadLine();

                if (!game.IsInputValid(input))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Nieprawidłowe dane! Użyj 4 znaków z dostępnych kolorów.");
                    Console.ResetColor();
                    continue;
                }

                var result = game.EvaluateGuess(input);

                PrintResult(input, result.exact, result.inexact);
            }

            Console.WriteLine();
            if (game.IsGameWon)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"GRATULACJE! Wygrałeś w {game.AttemptsUsed} prób.");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("KONIEC GRY.");
                Console.WriteLine($"Sekretny kod to: {game.GetSecretCodeIfLost()}");
            }
            Console.ResetColor();

            Console.WriteLine("\nNaciśnij dowolny klawisz, aby zakończyć...");
            Console.ReadKey();
        }

        static void PrintResult(string guess, int exact, int inexact)
        {
            Console.Write("Twój typ: ");

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

            Console.Write(" | Wynik: ");

            // Exact Match (REd X)
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < exact; i++) Console.Write("X ");

            // Inexact Match (White O)
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < inexact; i++) Console.Write("O ");

            Console.ResetColor();
            Console.Write($" (Trafione: {exact}, Tylko kolor: {inexact})");
        }
    }
}
