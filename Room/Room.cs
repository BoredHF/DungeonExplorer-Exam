using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer.Creature;
using DungeonExplorer.Item;
using DungeonExplorer.Item.Items;
using DungeonExplorer.Player;

namespace DungeonExplorer.Room
{
    /// <summary>
    /// Represents a room in the game.
    /// </summary>
    public class Room {
        private string description;
        private RoomType roomType;
        private List<Item.Item> items; // List of items in the room
        private List<Monster> monsters;
        private bool isCleared;

        /// <summary>
        /// Initializes a new instance of the Room class.
        /// </summary>
        /// <param name="description">The description of the room.</param>
        /// <param name="roomType">The type of the room.</param>
        /// 

        private static Random random = new Random();
        public Room(string description, RoomType roomType)
        {
            this.description = description;
            this.roomType = roomType;
            this.items = new List<Item.Item>();
            this.monsters = new List<Monster>();
            this.isCleared = roomType == RoomType.Safe; // Safe rooms start cleared

            switch (roomType)
            {
                case RoomType.Safe:
                    description += " | SAFE ZONE |";
                    GenerateItems(); // Even safe rooms can have items
                    break;
                case RoomType.Normal:
                    description += " | NORMAL ROOM |";
                    AddRandomMonster();
                    GenerateItems();
                    break;
                case RoomType.Boss:
                    description += " | BOSS ROOM |";
                    monsters.Add(new Monster("Dragon", MonsterType.Dragon, 200, 30, 100));
                    GenerateItems(); // Boss rooms have better chances for good items
                    break;
                case RoomType.Event:
                    description += " | EVENT ROOM |";
                    GenerateItems();
                    isCleared = true; // Event rooms don't need clearing
                    break;
            }
        }

        private void AddRandomMonster()
        {
            Random random = new Random();
            int monsterType = random.Next(2); // 0 or 1

            switch (monsterType)
            {
                case 0:
                    monsters.Add(new Monster("Goblin", MonsterType.Goblin, 30, 5, 10));
                    break;
                case 1:
                    monsters.Add(new Monster("Orc", MonsterType.Orc, 50, 10, 20));
                    break;
            }
        }

        /// <summary>
        /// Gets the description of the room.
        /// </summary>
        /// <returns>The room description.</returns>
        public string GetDescription()
        {
            return description + (RequiresCombat() ? " [Combat Required]" : "");
        }

        /// <summary>
        /// Gets the type of the room.
        /// </summary>
        /// <returns>The room type.</returns>
        public RoomType GetRoomType()
        {
            return roomType;
        }

        /// <summary>
        /// Gets the items in the room.
        /// </summary>
        /// <returns>A list of items in the room.</returns>
        public List<Item.Item> GetItems()
        {
            return items;
        }

        public List<Monster> GetMonsters()
        {
            return monsters;
        }

        /// <summary>
        /// Adds an item to the room.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(Item.Item item)
        {
            items.Add(item);
        }

        public void AddMonster(Monster monster)
        {
            monsters.Add(monster);
        }

        /// <summary>
        /// Handles the player entering the room.
        /// </summary>
        /// <param name="player">The player entering the room.</param>
        public void EnterRoom(DungeonExplorer.Player.Player player)
        {
            Console.WriteLine($"\nYou entered {description}");
            
            if (monsters.Any())
            {
                Console.WriteLine("Monsters in the room:");
                foreach (var monster in monsters)
                {
                    Console.WriteLine($"- {monster.Name} (Health: {monster.Health}/{monster.MaxHealth})");
                }
            }

            if (items.Any())
            {
                Console.WriteLine("Items in the room:");
                foreach (var item in items)
                {
                    Console.WriteLine($"- {item.Name} (ID: {item.Id})");
                }
            }
        }

        public Monster GetStrongestMonster()
        {
            return monsters.OrderByDescending(m => m.AttackPower).FirstOrDefault();
        }

        private void GenerateItems()
        {
            int itemCount = random.Next(0, 3); // 0-2 items per room

            for (int i = 0; i < itemCount; i++)
            {
                // 30% chance for potion, 20% chance for weapon
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
                    
                    items.Add(potion);
                }
                else if (random.NextDouble() < 0.8)
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
                    
                    items.Add(weapon);
                }
            }
        }

        public bool RequiresCombat()
        {
            return !isCleared && monsters.Any();
        }

        public bool IsCleared()
        {
            return isCleared;
        }

        public void CheckRoomClear()
        {
            if (!monsters.Any())
            {
                isCleared = true;
            }
        }
    }
}
