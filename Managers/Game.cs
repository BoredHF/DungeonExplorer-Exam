using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DungeonExplorer.Item.Items;
using Microsoft.Win32;
using DungeonExplorer.Creature;
using DungeonExplorer.Player;
using DungeonExplorer.Room;
using DungeonExplorer.Item;
using System.IO;

namespace DungeonExplorer.Managers.Game
{
    internal class Game
    {
        private Player.Player Player { get; set; }
        private Room.Room CurrentRoom { get; set; }
        private RoomManager RoomManager { get; set; }
        private Queue<(string Message, ConsoleColor Color)> MessageQueue { get; set; } = new Queue<(string, ConsoleColor)>();
        private bool isGameOver = false;

        /// <summary>
        /// Initializes the game with one room and one player.
        /// </summary>
        public Game()
        {
            // Adjust the console buffer size
            Console.SetBufferSize(Console.WindowWidth, 300); // Increase buffer height to 300

            // Get player name with validation
            string playerName = string.Empty;
            const int MAX_NAME_LENGTH = 16;
            
            while (string.IsNullOrWhiteSpace(playerName) || playerName.Length > MAX_NAME_LENGTH)
            {
                Console.WriteLine($"What would you like to call this character? (Max {MAX_NAME_LENGTH} characters)");
                Console.Write("> ");
                playerName = Console.ReadLine()?.Trim();
                
                if (string.IsNullOrWhiteSpace(playerName))
                {
                    Console.WriteLine("Player name cannot be empty. Please enter a valid name.");
                }
                else if (playerName.Length > MAX_NAME_LENGTH)
                {
                    Console.WriteLine($"Player name is too long. Maximum length is {MAX_NAME_LENGTH} characters.");
                }
            }

            Debug.Assert(!string.IsNullOrWhiteSpace(playerName), "Player name should not be empty.");
            Debug.Assert(playerName.Length <= MAX_NAME_LENGTH, "Player name should not exceed maximum length.");

            Player = new DungeonExplorer.Player.Player(playerName);
            RoomManager = new RoomManager();
            CurrentRoom = RoomManager.GetCurrentRoom();

            Debug.Assert(Player != null, "Player should be initialized.");
            Debug.Assert(CurrentRoom != null, "CurrentRoom should be initialized.");
            Debug.Assert(RoomManager != null, "RoomManager should be initialized.");
        }

        /// <summary>
        /// Main game loop.
        /// </summary>
        public void Start()
        {
            // Display help instructions
            DisplayHelp();

            // Wait for a few seconds
            Thread.Sleep(2000); // 2000 milliseconds = 2 seconds

            // Main game loop
            while (!isGameOver)
            {
                DisplayGameStatus();
                string action = GetPlayerAction();
                HandlePlayerAction(action);
                
                if (!isGameOver)
                {
                    Console.WriteLine("\nPress Enter to continue...");
                    Console.ReadLine();
                }
            }
        }

        /// <summary>
        /// Displays the player's health and inventory.
        /// </summary>
        private void DisplayGameStatus()
        {
            Console.Clear();
            // Ensure adequate console buffer size
            try
            {
                Console.SetBufferSize(Math.Max(Console.WindowWidth, 120), Math.Max(Console.WindowHeight, 40));
            }
            catch (Exception)
            {
                // Ignore if we can't set the buffer size
            }
            
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            
            // Draw the main border
            DrawMainBorder();

            // Player Section (Top Left)
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(2, 1);
            Console.WriteLine("Name: " + Player.Name);
            Console.SetCursorPosition(2, 2);
            Console.WriteLine($"Level: {Player.Level}");
            
            // XP Bar
            Console.SetCursorPosition(2, 3);
            int xpForNextLevel = Player.Level * 100;
            Console.Write("XP: [");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('█', (int)((double)Player.Experience / xpForNextLevel * 20)));
            Console.Write(new string('░', 20 - (int)((double)Player.Experience / xpForNextLevel * 20)));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"] {Player.Experience}/{xpForNextLevel}");
            
            // Health Bar
            Console.SetCursorPosition(2, 4);
            Console.Write("HP: [");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(new string('█', (int)((double)Player.Health / Player.MaxHealth * 20)));
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(new string('░', 20 - (int)((double)Player.Health / Player.MaxHealth * 20)));
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"] {Player.Health}/{Player.MaxHealth}");
            
            Console.SetCursorPosition(2, 5);
            Console.WriteLine($"ATK: {Player.AttackPower}");

            // Statistics Section
            Console.SetCursorPosition(2, 7);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Statistics:");
            Console.SetCursorPosition(2, 8);
            Console.WriteLine($"Monsters Defeated: {Player.Stats.GetStats()["MonstersDefeated"]}");
            Console.SetCursorPosition(2, 9);
            Console.WriteLine($"Items Collected: {Player.Stats.GetStats()["ItemsCollected"]}");
            Console.SetCursorPosition(2, 10);
            Console.WriteLine($"Rooms Explored: {Player.Stats.GetStats()["RoomsExplored"]}");
            Console.SetCursorPosition(2, 11);
            Console.WriteLine($"Deaths: {Player.Stats.GetStats()["Deaths"]}");

            // Achievements Section
            Console.SetCursorPosition(2, 13);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Achievements:");
            var achievements = Player.Stats.GetAchievements();
            if (achievements != null && achievements.Count > 0)
            {
                for (int i = 0; i < achievements.Count; i++)
                {
                    Console.SetCursorPosition(2, 14 + i);
                    Console.WriteLine($"★ {achievements[i]}");
                }
            }
            else
            {
                Console.SetCursorPosition(2, 14);
                Console.WriteLine("No achievements yet");
            }

            // Inventory Section
            Console.SetCursorPosition(2, 16);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Inventory:");
            int invY = 17;

            // Show equipped items first
            var equipped = Player.Equipment;
            if (equipped != null && equipped.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(2, invY++);
                Console.WriteLine("Equipped:");
                foreach (var item in equipped)
                {
                    Console.SetCursorPosition(4, invY++);
                    if (item is Weapon weapon)
                        Console.WriteLine($"* {item.Name} (+{weapon.BaseDamage} ATK)");
                    else
                        Console.WriteLine($"* {item.Name}");
                }
                invY++;
            }

            // Show inventory items
            Console.ForegroundColor = ConsoleColor.Yellow;
            var inventory = Player.InventoryContents();
            if (inventory.Any())
            {
                Console.SetCursorPosition(2, invY++);
                Console.WriteLine("Carrying:");
                foreach (var item in inventory)
                {
                    Console.SetCursorPosition(4, invY++);
                    if (item is Weapon weapon)
                        Console.WriteLine($"- {item.Name} (ID: {item.Id}) [+{weapon.BaseDamage} ATK]");
                    else
                        Console.WriteLine($"- {item.Name} (ID: {item.Id})");
                }
            }
            else if (!equipped.Any())
            {
                Console.SetCursorPosition(2, invY++);
                Console.WriteLine("(Empty)");
            }

            // Current Location
            Console.SetCursorPosition(2, invY + 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"Location: Level {RoomManager.CurrentLevel} - Room (0,0)");

            // Room Items
            int itemsStartY = invY + 3;
            if (CurrentRoom.GetItems().Any())
            {
                Console.SetCursorPosition(2, itemsStartY++);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Room Items:");
                foreach (var item in CurrentRoom.GetItems())
                {
                    Console.SetCursorPosition(2, itemsStartY++);
                    Console.WriteLine($"- {item.Name} (ID: {item.Id})");
                }
                Console.SetCursorPosition(2, itemsStartY++);
                Console.WriteLine("Type 'pick up [item name]' or 'pick up [ID]' to collect");
            }

            // Combat Section
            if (CurrentRoom.RequiresCombat())
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.SetCursorPosition(2, itemsStartY + 2);
                Console.WriteLine("Combat Commands:");
                Console.SetCursorPosition(2, itemsStartY + 3);
                Console.WriteLine("- attack [monster name]");
                Console.SetCursorPosition(2, itemsStartY + 4);
                Console.WriteLine("- [item ID] to use items");
            }

            // Map Section (Right Side)
            Console.SetCursorPosition(windowWidth / 2 + 2, 1);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Dungeon Map");
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(windowWidth / 2 + 2, 3);
            Console.WriteLine($"Level {RoomManager.CurrentLevel}");
            
            // Display the map
            string mapDisplay = GetMapDisplay()
                .Replace($"Level {RoomManager.CurrentLevel}", "")
                .Replace("Map Legend:", "")
                .Trim();
            
            string[] allLines = mapDisplay.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string[] mapLines = allLines.Where(line => line.Contains("|")).Take(3).ToArray();

            int mapStartY = 5;
            for (int i = 0; i < 3; i++)
            {
                Console.SetCursorPosition(windowWidth / 2 + 2, mapStartY + i);
                if (i < mapLines.Length)
                {
                    Console.WriteLine(mapLines[i].TrimEnd());
                }
                else
                {
                    Console.WriteLine("|? ? ?|");
                }
            }

            // Map Legend
            Console.SetCursorPosition(windowWidth / 2 + 2, mapStartY + 4);
            Console.WriteLine("Map Legend:");
            string[] legendItems = {
                "P - Your Position",
                "B - Boss Room",
                "N - Normal Room",
                "T - Safe Room",
                "E - Event Room",
                "# - Wall",
                "? - Unexplored"
            };

            int legendY = mapStartY + 5;
            foreach (string item in legendItems)
            {
                Console.SetCursorPosition(windowWidth / 2 + 2, legendY++);
                Console.WriteLine(item);
            }

            // Combat Log
            Console.SetCursorPosition(windowWidth / 2 + 2, legendY + 1);
            Console.WriteLine("Combat Log:");
            
            int logStartY = legendY + 2;
            int maxLogMessages = windowHeight - logStartY - 7;
            var messages = MessageQueue.Reverse().Take(maxLogMessages).Reverse().ToList();
            
            if (messages.Any())
            {
                for (int i = 0; i < messages.Count; i++)
                {
                    Console.SetCursorPosition(windowWidth / 2 + 2, logStartY + i);
                    Console.ForegroundColor = messages[i].Color;
                    Console.WriteLine(messages[i].Message);
                }
            }

            // Available Commands at bottom
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(2, Console.WindowHeight - 3);
            if (CurrentRoom.RequiresCombat())
            {
                Console.WriteLine("- attack [monster name]");
                Console.SetCursorPosition(2, Console.WindowHeight - 2);
                Console.WriteLine("- [item ID] to use items");
            }
            else
            {
                Console.WriteLine("- move up/down/left/right");
                if (CurrentRoom.GetItems().Any())
                {
                    Console.SetCursorPosition(2, Console.WindowHeight - 2);
                    Console.WriteLine("- pick up [item name] or [ID]");
                }
            }

            // Input prompt
            Console.SetCursorPosition(2, Console.WindowHeight - 1);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("> ");
            Console.ResetColor();
        }

        private void DrawMainBorder()
        {
            int width = Console.WindowWidth;
            int height = Console.WindowHeight;
            int halfWidth = width / 2;
            
            // Border characters
            char horizontal = '─';
            char vertical = '│';
            char topLeft = '┌';
            char topRight = '┐';
            char bottomLeft = '└';
            char bottomRight = '┘';
            char teeRight = '├';
            char teeLeft = '┤';
            char teeDown = '┬';
            char teeUp = '┴';
            char cross = '┼';

            // Draw the box
            Console.Write(topLeft);
            for (int i = 1; i < width - 1; i++)
            {
                if (i == halfWidth) Console.Write(teeDown);
                else Console.Write(horizontal);
            }
            Console.WriteLine(topRight);

            // Draw middle sections
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(0, i);
                if (i == height - 7)  // Adjusted for new layout
                {
                    Console.Write(teeRight);
                    for (int j = 1; j < width - 1; j++)
                    {
                        if (j == halfWidth) Console.Write(cross);
                        else Console.Write(horizontal);
                    }
                    Console.Write(teeLeft);
                }
                else
                {
                    Console.Write(vertical);
                    Console.SetCursorPosition(halfWidth, i);
                    Console.Write(vertical);
                    Console.SetCursorPosition(width - 1, i);
                    Console.Write(vertical);
                }
            }

            // Draw bottom
            Console.SetCursorPosition(0, height - 1);
            Console.Write(bottomLeft);
            for (int i = 1; i < width - 1; i++)
            {
                if (i == halfWidth) Console.Write(teeUp);
                else Console.Write(horizontal);
            }
            Console.Write(bottomRight);
        }

        private void DisplayMessages(int x, int y)
        {
            int count = 0;
            var messages = new List<(string Message, ConsoleColor Color)>();
            while (MessageQueue.Count > 0 && count < 8)
            {
                messages.Add(MessageQueue.Dequeue());
                count++;
            }

            // Display messages in reverse order (newest first)
            for (int i = messages.Count - 1; i >= 0; i--)
            {
                Console.SetCursorPosition(x, y + (messages.Count - 1 - i));
                Console.ForegroundColor = messages[i].Color;
                Console.WriteLine(messages[i].Message);
            }
        }

        private string CenterText(string text)
        {
            int windowWidth = Console.WindowWidth;
            int padding = (windowWidth - text.Length) / 2;
            return new string(' ', padding) + text;
        }

        private string GetMapDisplay()
        {
            var originalOut = Console.Out;
            using (var sw = new StringWriter())
            {
                Console.SetOut(sw);
                RoomManager.DisplayMap();
                Console.SetOut(originalOut);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Gets the player's action choice.
        /// </summary>
        /// <returns>Player's action as a string.</returns>
        private string GetPlayerAction()
        {
            var action = Console.ReadLine();
            Debug.Assert(!string.IsNullOrWhiteSpace(action), "Player action should not be empty.");
            return action;
        }
        /// <summary>
        /// Handles the player's action.
        /// </summary>
        /// <param name="action">The action input by the player.</param>
        public void HandlePlayerAction(string action)
        {
            if (string.IsNullOrWhiteSpace(action)) return;

            // Check if player is dead
            if (!Player.IsAlive())
            {
                HandlePlayerDeath();
                return;
            }

            if (action.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                DisplayHelp();
                return;
            }

            if (action.Equals("stats", StringComparison.OrdinalIgnoreCase) || action == "6")
            {
                Console.Clear();
                Console.WriteLine(Player.Stats.GetFormattedStats());
                Console.WriteLine("\nPress Enter to continue...");
                Console.ReadLine();
                return;
            }

            if (action.Equals("exit", StringComparison.OrdinalIgnoreCase))
            {
                isGameOver = true;
                return;
            }

            // If combat is required, only allow combat actions and item usage
            if (CurrentRoom.RequiresCombat())
            {
                if (action.StartsWith("attack", StringComparison.OrdinalIgnoreCase))
                {
                    HandleCombat(action);
                    CurrentRoom.CheckRoomClear();
                }
                else if (int.TryParse(action, out int itemId))
                {
                    var item = Player.InventoryContents().FirstOrDefault(i => i.Id == itemId);
                    if (item != null)
                    {
                        if (item is Weapon weapon)
                        {
                            // Remove from carrying if it's being equipped
                            Player.InventoryContents().Remove(weapon);
                            
                            // Check if already equipped
                            if (Player.Equipment.Contains(weapon))
                            {
                                AddMessage($"{weapon.Name} is already equipped!", ConsoleColor.Yellow);
                            }
                            else
                            {
                                // Unequip current weapon if any
                                var currentWeapon = Player.Equipment.OfType<Weapon>().FirstOrDefault();
                                if (currentWeapon != null)
                                {
                                    Player.AttackPower -= currentWeapon.BaseDamage;
                                    Player.Equipment.Remove(currentWeapon);
                                    // Add the unequipped weapon back to inventory
                                    Player.InventoryContents().Add(currentWeapon);
                                    AddMessage($"Unequipped {currentWeapon.Name}", ConsoleColor.Yellow);
                                }

                                // Equip the new weapon
                                Player.Equipment.Add(weapon);
                                Player.AttackPower += weapon.BaseDamage;
                                AddMessage($"Equipped {weapon.Name}! Attack power increased to {Player.AttackPower}!", ConsoleColor.Green);
                            }
                        }
                        else if (item.Useable)
                        {
                            Player.UseItem(item);
                            AddMessage($"Used {item.Name}.", ConsoleColor.Green);
                        }
                        else
                        {
                            AddMessage("This item cannot be used.", ConsoleColor.Red);
                        }
                    }
                    else
                    {
                        AddMessage("Invalid item ID or item not found in inventory.", ConsoleColor.Red);
                    }
                }
                else
                {
                    AddMessage("Type 'attack [monster name]' to fight or use an item ID", ConsoleColor.Yellow);
                }
                return;
            }

            // Normal actions when not in combat
            if (action.StartsWith("attack", StringComparison.OrdinalIgnoreCase))
            {
                HandleCombat(action);
                CurrentRoom.CheckRoomClear();
            }
            else if (int.TryParse(action, out int itemId))
            {
                var item = Player.InventoryContents().FirstOrDefault(i => i.Id == itemId);
                if (item != null && item.Useable)
                {
                    Player.UseItem(item);
                    AddMessage($"You used {item.Name}.", ConsoleColor.Green);
                }
                else
                {
                    AddMessage("Invalid item ID or item cannot be used.", ConsoleColor.Red);
                }
            }
            else if (action.StartsWith("pick up", StringComparison.OrdinalIgnoreCase))
            {
                var itemName = action.Substring("pick up".Length).Trim();
                
                // Try to parse as ID first
                if (int.TryParse(itemName, out int pickupItemId))
                {
                    var itemById = CurrentRoom.GetItems().FirstOrDefault(i => i.Id == pickupItemId);
                    if (itemById != null)
                    {
                        Player.PickUpItem(itemById);
                        CurrentRoom.GetItems().Remove(itemById);
                        AddMessage($"You picked up {itemById.Name}.", ConsoleColor.Green);
                    }
                    else
                    {
                        AddMessage($"Could not find item with ID: {pickupItemId}", ConsoleColor.Red);
                    }
                }
                // If not an ID, try to find by name
                else
                {
                    var item = CurrentRoom.GetItems().FirstOrDefault(i => i.Name.Equals(itemName, StringComparison.OrdinalIgnoreCase));
                    if (item != null)
                    {
                        Player.PickUpItem(item);
                        CurrentRoom.GetItems().Remove(item);
                        AddMessage($"You picked up {item.Name}.", ConsoleColor.Green);
                    }
                    else
                    {
                        AddMessage($"Could not find item: {itemName}", ConsoleColor.Red);
                    }
                }
            }
            else if (action.StartsWith("move", StringComparison.OrdinalIgnoreCase))
            {
                string[] parts = action.Split(' ');
                if (parts.Length < 2)
                {
                    AddMessage("Please specify a direction (up/down/left/right).", ConsoleColor.Red);
                    return;
                }

                string direction = parts[1];
                if (RoomManager.MovePlayer(direction, Player))
                {
                    CurrentRoom = RoomManager.GetCurrentRoom();
                    AddMessage($"\nYou entered {CurrentRoom.GetDescription()}", ConsoleColor.White);
                    
                    if (CurrentRoom.RequiresCombat())
                    {
                        AddMessage("A monster blocks your path! You must fight!", ConsoleColor.Red);
                        if (CurrentRoom.GetMonsters().Any())
                        {
                            AddMessage("\nMonsters in the room:", ConsoleColor.Red);
                            foreach (var monster in CurrentRoom.GetMonsters())
                            {
                                AddMessage($"- {monster.Name} (Health: {monster.Health}/{monster.MaxHealth})", ConsoleColor.Red);
                            }
                        }
                        InitiateCombat();
                    }
                    else
                    {
                        if (CurrentRoom.GetMonsters().Any())
                        {
                            AddMessage("\nMonsters in the room:", ConsoleColor.Red);
                            foreach (var monster in CurrentRoom.GetMonsters())
                            {
                                AddMessage($"- {monster.Name} (Health: {monster.Health}/{monster.MaxHealth})", ConsoleColor.Red);
                            }
                        }
                        
                        if (CurrentRoom.GetItems().Any())
                        {
                            AddMessage("\nItems in the room:", ConsoleColor.Green);
                            foreach (var item in CurrentRoom.GetItems())
                            {
                                AddMessage($"- {item.Name} (ID: {item.Id})", ConsoleColor.Green);
                            }
                        }
                    }
                }
                else
                {
                    AddMessage($"Cannot move {direction}.", ConsoleColor.Red);
                }
            }
            else
            {
                AddMessage("Invalid command. Type 'help' for commands.", ConsoleColor.Red);
            }
        }

        private void InitiateCombat()
        {
            MessageQueue.Clear(); // Clear previous combat messages
            var monster = CurrentRoom.GetMonsters().First();
            
            AddMessage($"A {monster.Name} appears! (Health: {monster.Health}/{monster.MaxHealth})", ConsoleColor.Red);
            AddMessage($"Type 'attack {monster.Name}' to fight", ConsoleColor.Yellow);

            // Show available weapons only if they exist
            var weapons = Player.InventoryContents().OfType<Weapon>();
            if (weapons.Any())
            {
                foreach (var weapon in weapons)
                {
                    AddMessage($"Type '{weapon.Id}' - Equip {weapon.Name} (+{weapon.BaseDamage} ATK)", ConsoleColor.Yellow);
                }
            }

            // Show available health potions only if they exist
            var potions = Player.GetPotions();
            if (potions.Any())
            {
                foreach (var potion in potions)
                {
                    AddMessage($"Type '{potion.Id}' - Use {potion.Name}", ConsoleColor.Green);
                }
            }
        }

        private void HandleCombat(string action)
        {
            var monsterName = action.Substring("attack".Length).Trim();
            var monster = CurrentRoom.GetMonsters().FirstOrDefault(m => m.Name.Equals(monsterName, StringComparison.OrdinalIgnoreCase));

            if (monster != null)
            {
                // Player's turn
                Player.Attack(monster);
                int playerDamage = Player.AttackPower;
                double damagePercentage = (double)playerDamage / monster.MaxHealth * 100;
                
                AddMessage($"\nYou attack the {monster.Name}!", ConsoleColor.Cyan);
                AddMessage($"Hit! {playerDamage} damage dealt! ({damagePercentage:F1}% of max HP)", ConsoleColor.Yellow);
                AddMessage($"{monster.Name}'s Health: {monster.Health}/{monster.MaxHealth}", ConsoleColor.White);

                if (!monster.IsAlive())
                {
                    AddMessage($"\nVictory! You have defeated the {monster.Name}!", ConsoleColor.Green);
                    int xpGained = monster.ExperienceValue;
                    Player.AddExperience(xpGained);
                    AddMessage($"Experience gained: +{xpGained} XP", ConsoleColor.Cyan);

                    // Check if this was a boss monster
                    if (CurrentRoom.GetRoomType() == RoomType.Boss)
                    {
                        AddMessage("You have defeated the boss of this level!", ConsoleColor.Yellow);
                        if (RoomManager.DefeatBossAndAdvance())
                        {
                            AddMessage($"\nLevel {RoomManager.CurrentLevel} completed!", ConsoleColor.Green);
                            AddMessage("You have advanced to the next level!", ConsoleColor.Green);
                            AddMessage("Your health has been fully restored!", ConsoleColor.Green);
                            Player.Heal(Player.MaxHealth);
                            CurrentRoom = RoomManager.GetCurrentRoom();
                            DisplayLevelTransition();
                        }
                    }

                    Random random = new Random();
                    if (random.NextDouble() < monster.GetDropRate())
                    {
                        List<string> droppedItems = new List<string>();

                        if (random.NextDouble() < 0.7)
                        {
                            // Generate a potion  
                            double potionRoll = random.NextDouble();
                            HealthPotion potion;

                            if (potionRoll < 0.4)
                                potion = HealthPotion.CreateMinorHealingPotion();
                            else if (potionRoll < 0.8)
                                potion = HealthPotion.CreateGreaterHealingPotion();
                            else if (potionRoll < 0.95)
                                potion = HealthPotion.CreateRegenPotion();
                            else
                                potion = HealthPotion.CreateDivineElixir();

                            CurrentRoom.AddItem(potion);
                            droppedItems.Add($"{potion.Name} (ID: {potion.Id})");
                        }
                        
                        if (random.NextDouble() < 0.8)
                        {
                            // Generate a weapon  
                            double weaponRoll = random.NextDouble();
                            Weapon weapon;

                            if (weaponRoll < 0.4)
                                weapon = Weapon.CreateRustyDagger();
                            else if (weaponRoll < 0.7)
                                weapon = Weapon.CreateIronSword();
                            else if (weaponRoll < 0.9)
                                weapon = Weapon.CreateBattleAxe();
                            else
                                weapon = Weapon.CreateEnchantedStaff();

                            CurrentRoom.AddItem(weapon);
                            droppedItems.Add($"{weapon.Name} (ID: {weapon.Id})");
                        }

                        if (droppedItems.Count > 0)
                        {
                            AddMessage($"\nWoah! The {monster.Name} dropped:", ConsoleColor.Green);
                            foreach (string item in droppedItems)
                            {
                                AddMessage($"- {item}", ConsoleColor.Green);
                            }
                        }
                    }
                    else
                    {
                        AddMessage($"\nUnlucky.. The {monster.Name} didn't drop anything.", ConsoleColor.Red);
                    }

                    CurrentRoom.GetMonsters().Remove(monster);

                    // Check for level up  
                    if (Player.CheckLevelUp())
                    {
                        AddMessage($"Level Up! You are now level {Player.Level}!", ConsoleColor.Yellow);
                        AddMessage($"Your health and attack power have increased!", ConsoleColor.Yellow);
                        DisplayLevelUpStats();
                    }

                    if (!CurrentRoom.GetMonsters().Any())
                    {
                        AddMessage("The room is now clear! You can proceed!", ConsoleColor.Green);
                        // Clear combat mode and refresh display
                        DisplayGameStatus();
                    }
                }
                else
                {
                    // Monster's turn
                    monster.Attack(Player);
                    int monsterDamage = monster.AttackPower;
                    double playerDamagePercentage = (double)monsterDamage / Player.MaxHealth * 100;
                    
                    string attackDescription = GetRandomAttackDescription(monster.Type);
                    AddMessage($"\n{attackDescription}", ConsoleColor.Red);
                    AddMessage($"You take {monsterDamage} damage! ({playerDamagePercentage:F1}% of your HP)", ConsoleColor.Red);
                    AddMessage($"Your Health: {Player.Health}/{Player.MaxHealth}", ConsoleColor.White);

                    // Warning messages based on health status
                    if (Player.Health <= Player.MaxHealth * 0.2)
                    {
                        AddMessage("\n⚠ CRITICAL HEALTH WARNING! ⚠", ConsoleColor.Red);
                        AddMessage("Use a health potion immediately!", ConsoleColor.Red);
                    }
                    else if (Player.Health <= Player.MaxHealth * 0.5)
                    {
                        AddMessage("\n⚠ Low Health Warning", ConsoleColor.Yellow);
                        AddMessage("Consider using a health potion", ConsoleColor.Yellow);
                    }
                }
            }
            else
            {
                AddMessage($"There is no monster named '{monsterName}' here.", ConsoleColor.Red);
            }
        }

        private string GetRandomAttackDescription(MonsterType monsterType)
        {
            Random random = new Random();
            List<string> descriptions = new List<string>();
            
            switch (monsterType)
            {
                case MonsterType.Goblin:
                    descriptions.Add("The Goblin lunges with its rusty dagger!");
                    descriptions.Add("The Goblin performs a quick slash!");
                    descriptions.Add("The Goblin attempts a sneaky strike!");
                    break;
                case MonsterType.Orc:
                    descriptions.Add("The Orc swings its massive battle axe!");
                    descriptions.Add("The Orc charges with tremendous force!");
                    descriptions.Add("The Orc unleashes a powerful strike!");
                    break;
                case MonsterType.Dragon:
                    descriptions.Add("The Dragon unleashes a devastating breath attack!");
                    descriptions.Add("The Dragon swipes with its massive claws!");
                    descriptions.Add("The Dragon's tail whips through the air!");
                    break;
                default:
                    descriptions.Add("The monster attacks!");
                    break;
            }
            
            return descriptions[random.Next(descriptions.Count)];
        }

        private void HandlePlayerDeath()
        {
            isGameOver = true;
            AddMessage("\n=== GAME OVER ===", ConsoleColor.Red);
            AddMessage("You have been defeated!", ConsoleColor.Red);
            AddMessage($"\nFinal Stats:", ConsoleColor.Yellow);
            AddMessage($"Level: {Player.Level}", ConsoleColor.Yellow);
            AddMessage($"Experience: {Player.Experience}", ConsoleColor.Yellow);
            AddMessage($"Rooms Explored: {RoomManager.CurrentLevel}", ConsoleColor.Yellow);
            AddMessage("\nPress Enter to exit...", ConsoleColor.White);
        }

        private void DisplayLevelTransition()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("           LEVEL TRANSITION           ");
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine($"\nWelcome to Level {RoomManager.CurrentLevel}!");
            Console.WriteLine("\nThe dungeon grows more dangerous...");
            Console.WriteLine("Monsters are stronger...");
            Console.WriteLine("But the rewards are greater!");
            Console.WriteLine("\nPress Enter to continue...");
            Console.ResetColor();
            Console.ReadLine();
        }

        private void DisplayLevelUpStats()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n=== LEVEL UP STATS ===");
            Console.WriteLine($"Health: {Player.MaxHealth} (+20)");
            Console.WriteLine($"Attack: {Player.AttackPower} (+5)");
            Console.WriteLine("══════════════════════");
            Console.ResetColor();
        }

        /// <summary>
        /// Displays the help instructions for the player.
        /// </summary>
        private void DisplayHelp()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("═══════════════════════════════════════");
            Console.WriteLine("           GAME CONTROLS               ");
            Console.WriteLine("═══════════════════════════════════════");
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nCombat:");
            Console.WriteLine("  Type 'attack [monster name]' to fight");
            Console.WriteLine("  Type an item's ID to use it in combat (weapons/potions)");
            
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nItems & Inventory:");
            Console.WriteLine("  Your inventory is always visible on the left side");
            Console.WriteLine("  Type 'pick up [item name]' or 'pick up [ID]' to collect items");
            Console.WriteLine("  Type an item's ID to use it from your inventory");
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nMovement:");
            Console.WriteLine("  Type 'move [direction]' to move");
            Console.WriteLine("  Directions: up, down, left, right");
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nOther:");
            Console.WriteLine("  Type 'exit' to quit the game");
            Console.WriteLine("  Type 'help' to show this screen");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n═══════════════════════════════════════");
            Console.WriteLine(" ");
            Console.WriteLine("\nPress Enter to continue...");
            Console.ResetColor();
            Console.ReadLine();
        }

        private void AddMessage(string message, ConsoleColor color = ConsoleColor.White)
        {
            MessageQueue.Enqueue((message, color));
        }
    }
}
