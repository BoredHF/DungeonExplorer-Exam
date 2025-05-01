using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using DungeonExplorer.Creature;
using DungeonExplorer.Item;
using DungeonExplorer.Item.Items;
using DungeonExplorer.Player;
using DungeonExplorer.Room;

namespace DungeonExplorer.Testing
{
    public class Testing
    {
        private static readonly string LogFilePath = "game_test_log.txt";
        private static StreamWriter logWriter;

        public Testing()
        {
            // Initialize log file
            logWriter = new StreamWriter(LogFilePath, true);
            logWriter.WriteLine($"\n=== Test Session Started: {DateTime.Now} ===");
        }

        public void RunAllTests()
        {
            TestPlayerCreation();
            TestItemSystem();
            TestRoomSystem();
            TestCombatSystem();
            TestInventorySystem();
            TestGameMap();
            TestMonsterAI();
        }

        private void LogTestResult(string testName, bool passed, string message)
        {
            string result = passed ? "PASSED" : "FAILED";
            string logMessage = $"[{DateTime.Now}] {testName}: {result} - {message}";
            logWriter.WriteLine(logMessage);
            Debug.WriteLine(logMessage);
        }

        private void TestPlayerCreation()
        {
            try
            {
                // Test valid player creation
                Player.Player player = new Player.Player("TestPlayer");
                Debug.Assert(player != null, "Player should not be null");
                Debug.Assert(player.Name == "TestPlayer", "Player name should match");
                Debug.Assert(player.Health == 100, "Player should start with 100 health");
                Debug.Assert(player.Level == 1, "Player should start at level 1");
                LogTestResult("TestPlayerCreation", true, "Player created successfully with correct initial values");

                // Test player level up
                player.AddExperience(100);
                bool leveledUp = player.CheckLevelUp();
                Debug.Assert(leveledUp, "Player should level up with sufficient experience");
                Debug.Assert(player.Level == 2, "Player should be level 2 after leveling up");
                LogTestResult("TestPlayerLevelUp", true, "Player leveled up correctly");
            }
            catch (Exception ex)
            {
                LogTestResult("TestPlayerCreation", false, $"Exception: {ex.Message}");
                throw;
            }
        }

        private void TestItemSystem()
        {
            try
            {
                // Test weapon creation
                Weapon sword = new Weapon(
                    "Test Sword",
                    "A test weapon",
                    10,
                    Weapon.WeaponType.Sword,
                    Weapon.WeaponRarity.Common,
                    "Test lore"
                );
                Debug.Assert(sword != null, "Weapon should not be null");
                Debug.Assert(sword.BaseDamage == 10, "Weapon damage should match");
                LogTestResult("TestWeaponCreation", true, "Weapon created successfully");

                // Test potion creation
                HealthPotion potion = HealthPotion.CreateMinorHealingPotion();
                Debug.Assert(potion != null, "Potion should not be null");
                Debug.Assert(potion.IsStackable, "Minor healing potion should be stackable");
                LogTestResult("TestPotionCreation", true, "Potion created successfully");
            }
            catch (Exception ex)
            {
                LogTestResult("TestItemSystem", false, $"Exception: {ex.Message}");
                throw;
            }
        }

        private void TestRoomSystem()
        {
            try
            {
                // Test room creation
                Room.Room room = new Room.Room("Test Room", RoomType.Normal);
                Debug.Assert(room != null, "Room should not be null");
                Debug.Assert(room.GetRoomType() == RoomType.Normal, "Room type should match");
                LogTestResult("TestRoomCreation", true, "Room created successfully");

                // Test room with items
                Weapon testWeapon = new Weapon(
                    "Test Weapon",
                    "A test weapon",
                    5,
                    Weapon.WeaponType.Sword,
                    Weapon.WeaponRarity.Common,
                    "Test lore"
                );
                room.AddItem(testWeapon);
                Debug.Assert(room.GetItems().Count == 1, "Room should have one item");
                LogTestResult("TestRoomItems", true, "Room items handled correctly");
            }
            catch (Exception ex)
            {
                LogTestResult("TestRoomSystem", false, $"Exception: {ex.Message}");
                throw;
            }
        }

        private void TestCombatSystem()
        {
            try
            {
                Player.Player player = new Player.Player("TestPlayer");
                Monster monster = new Monster("TestMonster", MonsterType.Goblin, 50, 5, 10);

                // Test initial health
                Debug.Assert(monster.Health == 50, "Monster should start with correct health");
                Debug.Assert(player.Health == 100, "Player should start with correct health");

                // Test damage application
                int initialMonsterHealth = monster.Health;
                player.Attack(monster);
                Debug.Assert(monster.Health < initialMonsterHealth, "Monster should take damage");
                LogTestResult("TestCombatSystem", true, "Combat system working correctly");
            }
            catch (Exception ex)
            {
                LogTestResult("TestCombatSystem", false, $"Exception: {ex.Message}");
                throw;
            }
        }

        private void TestInventorySystem()
        {
            try
            {
                Player.Player player = new Player.Player("TestPlayer");
                Weapon weapon = new Weapon(
                    "Test Weapon",
                    "A test weapon",
                    5,
                    Weapon.WeaponType.Sword,
                    Weapon.WeaponRarity.Common,
                    "Test lore"
                );

                // Test item pickup
                player.PickUpItem(weapon);
                Debug.Assert(player.GetInventory().Count == 1, "Player inventory should have one item");
                LogTestResult("TestInventorySystem", true, "Inventory system working correctly");
            }
            catch (Exception ex)
            {
                LogTestResult("TestInventorySystem", false, $"Exception: {ex.Message}");
                throw;
            }
        }

        private void TestGameMap()
        {
            try
            {
                GameMap.GameMap gameMap = new GameMap.GameMap();
                Debug.Assert(gameMap != null, "GameMap should not be null");
                Debug.Assert(gameMap.CurrentLevel == 1, "GameMap should start at level 1");
                LogTestResult("TestGameMap", true, "GameMap created successfully");
            }
            catch (Exception ex)
            {
                LogTestResult("TestGameMap", false, $"Exception: {ex.Message}");
                throw;
            }
        }

        private void TestMonsterAI()
        {
            try
            {
                Monster monster = new Monster("TestMonster", MonsterType.Goblin, 50, 5, 10);
                Player.Player player = new Player.Player("TestPlayer");

                // Test monster behavior when healthy
                bool shouldAttack = monster.ShouldAttack(player);
                Debug.Assert(shouldAttack, "Monster should attack when healthy");

                // Test monster behavior when weak
                monster.TakeDamage(45); // Make monster weak
                bool shouldFlee = monster.ShouldFlee();
                Debug.Assert(shouldFlee, "Monster should flee when weak");
                LogTestResult("TestMonsterAI", true, "Monster AI working correctly");
            }
            catch (Exception ex)
            {
                LogTestResult("TestMonsterAI", false, $"Exception: {ex.Message}");
                throw;
            }
        }

        public void Close()
        {
            logWriter.WriteLine($"=== Test Session Ended: {DateTime.Now} ===\n");
            logWriter.Close();
        }
    }
} 