namespace AlgoArena
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool running = true;
            while (running)
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
                Console.Write("Choose your arena: ");
                string choice = Console.ReadLine()?.Trim() ?? "0";

                switch (choice)
                {
                    case "1": PrimeFortress.Run(); break;
                    case "2": NetworkRanger.Run(); break;
                    case "3": CipherMachine.Run(); break;
                    case "4": TextForensics.Run(); break;
                    case "5": CardArena.Run(); break;
                    case "6": PuzzleBox.Run(); break;
                    case "7": MazeExpedition.Run(); break;
                    case "0": running = false; break;
                    default: Console.WriteLine("Unknown arena. Try again."); break;
                }
            }
            Console.WriteLine("Until next battle, algorithm warrior! ⚔");
        }
    }
}