using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer.Item.Items;
using Microsoft.Win32;

namespace DungeonExplorer.Managers.Game {
    internal class Game {
        private Player.Player Player { get; set; }
        private Room.Room CurrentRoom { get; set; }

        public Game()
        {
            // Initialize the game with one room and one player
            Player = new Player.Player("Player", 100);
            Player.PickUpItem(new HealthPotion());

            CurrentRoom = new Room.Room($"Starting Room", Room.RoomType.Normal);
        }

        public void Start()
        {
            // Change the playing logic into true and populate the while loop
            bool playing = true;
            while (playing)
            {
                DisplayGameStatus();
                string action = GetPlayerAction();
                playing = HandlePlayerAction(action);
                Console.WriteLine("\n\n");
            }
        }

        private void DisplayGameStatus()
        {
            // Method to display the player's health and inventory
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

        private string GetPlayerAction()
        {
            // Method to display the available actions and get the player's choice
            Console.WriteLine("What would you like to do?");
            DisplayPlayerActions();
            Console.Write("> ");
            return Console.ReadLine();
        }

        private void DisplayPlayerActions()
        {
            // Method to display all the actions the player has
            Console.WriteLine("Available Actions:");
            foreach (var item in Player.InventoryContents())
            {
                if (item.Useable)
                {
                    Console.WriteLine($"- Use {item.Name} (ID: {item.Id})");
                }
            }
            Console.WriteLine("- Exit");
            Console.WriteLine();
        }

        private bool HandlePlayerAction(string action)
        {
            // Method to handle player actions
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
    }
}
