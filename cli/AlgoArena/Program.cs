namespace AlgoArena
{
    using System;

    public static unsafe class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            RunMainLoop();
            Console.WriteLine("Until next battle, algorithm warrior! ⚔");
        }

        private static void RunMainLoop()
        {
            bool running = true;
            while (running)
            {
                DisplayMainMenu();
                string choice = GetArenaChoice();
                running = ExecuteChoice(choice);
            }
        }

        private static void DisplayMainMenu()
        {
            Console.WriteLine();
            Console.WriteLine("╔══════════════════════════════════════════════════════╗");
            Console.WriteLine("║           ⚔  A L G O   A R E N A  ⚔               ║");
            Console.WriteLine("║      Where Algorithms Fight for Glory              ║");
            Console.WriteLine("╠══════════════════════════════════════════════════════╣");
            Console.WriteLine("║                                                      ║");
            Console.WriteLine("║  1. 🏰  Prime Fortress    — factorize & defend      ║");
            Console.WriteLine("║  2. 🌐  Network Ranger    — graph shortest paths     ║");
            Console.WriteLine("║  3. 🔐  Cipher Machine    — hash & xor encryption   ║");
            Console.WriteLine("║  4. 🔍  Text Forensics    — string algorithms        ║");
            Console.WriteLine("║  5. 🃏  Card Arena        — sort & search showdown   ║");
            Console.WriteLine("║  6. 🧩  Puzzle Box        — combinatorics tricks     ║");
            Console.WriteLine("║  7. 🏔  Maze Expedition   — DSU + segment tree       ║");
            Console.WriteLine("║  0. 🚪  Exit                                         ║");
            Console.WriteLine("║                                                      ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════╝");
        }

        private static string GetArenaChoice()
        {
            Console.Write("Choose your arena: ");
            return Console.ReadLine()?.Trim() ?? "0";
        }

        private static bool ExecuteChoice(string choice)
        {
            switch (choice)
            {
                case "1": PrimeFortress.Run(); return true;
                case "2": NetworkRanger.Run(); return true;
                case "3": CipherMachine.Run(); return true;
                case "4": TextForensics.Run(); return true;
                case "5": CardArena.Run(); return true;
                case "6": PuzzleBox.Run(); return true;
                case "7": MazeExpedition.Run(); return true;
                case "0": return false;
                default:
                    Console.WriteLine("Unknown arena. Try again.");
                    return true;
            }
        }
    }
}