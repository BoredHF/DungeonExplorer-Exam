using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DungeonExplorer.Item.Items;
using Microsoft.Win32;

namespace DungeonExplorer.Managers.Game {
    internal class Game {
        private Player.Player Player { get; set; }
        private Room.Room CurrentRoom { get; set; }
        private RoomManager RoomManager { get; set; } // Add RoomManager instance

        /// <summary>
        /// Initializes the game with one room and one player.
        /// </summary>
        public Game()
        {
            Console.Clear();

            // Asks the player for their name, and then sets the player's name
            string playerName = string.Empty;
            while (string.IsNullOrWhiteSpace(playerName))
            {
                Console.WriteLine("What would you like to call this character?");
                Console.Write("> ");
                playerName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    Console.WriteLine("Player name cannot be empty. Please enter a valid name.");
                }
            }

            Player = new Player.Player(playerName, 100);
            CurrentRoom = new Room.Room($"Starting Room", Room.RoomType.Normal);
            RoomManager = new RoomManager(); // Initialize RoomManager
        }

        /// <summary>
        /// Main game loop.
        /// </summary>
        public void Start()
        {
            // Display help instructions
            DisplayHelp();

            // Wait for a few seconds
            Thread.Sleep(5000); // 5000 milliseconds = 5 seconds

            // Main game loop
            bool playing = true;
            while (playing)
            {
                DisplayGameStatus();
                RoomManager.DisplayMap();
                string action = GetPlayerAction();
                playing = HandlePlayerAction(action);
                Console.WriteLine("\n\n");
            }
        }

        /// <summary>
        /// Displays the player's health and inventory.
        /// </summary>
        private void DisplayGameStatus()
        {
            Console.WriteLine("===== GAME STATUS =====");
            Console.WriteLine($"Player Health: {Player.Health}/{Player.getMaxHealth()}");
            Console.WriteLine();
            Console.WriteLine("Player Inventory:");
            foreach (var item in Player.InventoryContents())
            {
                Console.WriteLine($"- {item.Name} (ID: {item.Id})");
            }
            Console.WriteLine();
            Console.WriteLine($"Current Room: {CurrentRoom.GetDescription()}");
            Console.WriteLine("=======================");
            Console.WriteLine();
        }

        /// <summary>
        /// Gets the player's action choice.
        /// </summary>
        /// <returns>Player's action as a string.</returns>
        private string GetPlayerAction()
        {
            Console.WriteLine("What would you like to do?");
            DisplayPlayerActions();
            Console.Write("> ");
            return Console.ReadLine();
        }

        /// <summary>
        /// Displays the available actions for the player.
        /// </summary>
        private void DisplayPlayerActions()
        {
            Console.WriteLine("Available Actions:");
            foreach (var item in Player.InventoryContents())
            {
                if (item.Useable)
                {
                    Console.WriteLine($"- Use {item.Name} (ID: {item.Id})");
                }
            }

            if (CurrentRoom.GetItems() != null)
            {
                foreach (var item in CurrentRoom.GetItems())
                {
                    Console.WriteLine($"- Pick up {item.Name} (ID: {item.Id})");
                }
            }

            Console.WriteLine("- Move Up");
            Console.WriteLine("- Move Down");
            Console.WriteLine("- Move Left");
            Console.WriteLine("- Move Right");
            Console.WriteLine("- Exit");
            Console.WriteLine();
        }

        /// <summary>
        /// Handles the player's action.
        /// </summary>
        /// <param name="action">The action input by the player.</param>
        /// <returns>True if the game continues, false if the game ends.</returns>
        private bool HandlePlayerAction(string action)
        {
            if (int.TryParse(action, out int itemId))
            {
                var item = Player.InventoryContents().FirstOrDefault(i => i.Id == itemId);
                if (item != null && item.Useable)
                {
                    Player.UseItem(item);
                }
                else
                {
                    Console.WriteLine("Invalid action. Please try again.");
                }
            }
            // Handle pick up action
            else if (action.StartsWith("pick up", StringComparison.OrdinalIgnoreCase))
            {
                var itemName = action.Substring("pick up".Length).Trim();
                var item = CurrentRoom.GetItems().FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
                if (item != null)
                {
                    Player.PickUpItem(item);
                    CurrentRoom.GetItems().Remove(item);
                    Console.WriteLine($"You picked up {item.Name}.");
                }
                else
                {
                    Console.WriteLine("Item not found in the room.");
                }
            }
            // Handle movement actions
            else if (action.Equals("move up", StringComparison.OrdinalIgnoreCase) ||
                     action.Equals("move down", StringComparison.OrdinalIgnoreCase) ||
                     action.Equals("move left", StringComparison.OrdinalIgnoreCase) ||
                     action.Equals("move right", StringComparison.OrdinalIgnoreCase))
            {
                string direction = action.Split(' ')[1];
                if (!RoomManager.MovePlayer(direction, Player))
                {
                    Console.WriteLine("You can't move in that direction.");
                }
            }
            // Handle exit action
            else if (action.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            else
            {
                Console.WriteLine("Invalid action. Please try again.");
            }
            return true;
        }
        /// <summary>
        /// Displays the help instructions for the player.
        /// </summary>
        private void DisplayHelp()
        {

            // Not really needed, but my flatmates when testing didn't understand how the commands worked
            Console.WriteLine("===== HELP =====");
            Console.WriteLine("Type the command that you want to perform.");
            Console.WriteLine("For items, use the following commands:");
            Console.WriteLine("- To pick up an item: pickup {id}");
            Console.WriteLine("- To use an item: use {id}");
            Console.WriteLine("Available commands:");
            Console.WriteLine("- Move Up");
            Console.WriteLine("- Move Down");
            Console.WriteLine("- Move Left");
            Console.WriteLine("- Move Right");
            Console.WriteLine("- Exit");
            Console.WriteLine("================");
        }

    }
}
