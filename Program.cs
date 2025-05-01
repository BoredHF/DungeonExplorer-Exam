using System;
using System.Diagnostics;
using DungeonExplorer.Managers.Game;
using DungeonExplorer.Testing;

namespace DungeonExplorer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Run tests if specified
            if (args.Length > 0 && args[0].ToLower() == "--test")
            {
                RunTests();
                return;
            }

            // Start the game
            Game game = new Game();
            game.Start();
        }

        static void RunTests()
        {
            Console.WriteLine("Running game tests...");
            Testing.Testing testing = new Testing.Testing();
            
            try
            {
                testing.RunAllTests();
                Console.WriteLine("All tests completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test failed: {ex.Message}");
                Debug.WriteLine($"Test failed: {ex.Message}");
            }
            finally
            {
                testing.Close();
            }
        }
    }
}
