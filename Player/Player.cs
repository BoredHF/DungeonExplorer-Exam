using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer.Creature;
using DungeonExplorer.Interfaces;
using DungeonExplorer.Item;
using DungeonExplorer.Item.Items;

namespace DungeonExplorer.Player
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player : Creature.Creature
    {
        private List<Item.Item> inventory = new List<Item.Item>();
        public int Experience { get; private set; }
        public int Level { get; private set; }
        public DungeonExplorer.Player.Statistics Stats { get; private set; }

        /// <summary>
        /// Initializes a new instance of the Player class.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        public Player(string name) : base(name, 100, 10)
        {
            Level = 1;
            Experience = 0;
            Stats = new DungeonExplorer.Player.Statistics();
        }

        /// <summary>
        /// Adds experience to the player.
        /// </summary>
        /// <param name="amount">The amount of experience to add.</param>
        public void AddExperience(int amount)
        {
            Experience += amount;
            Stats.UpdateHighestLevel(Level);
        }

        public bool CheckLevelUp()
        {
            int experienceNeeded = Level * 100;  // Simple level up formula
            if (Experience >= experienceNeeded)
            {
                Level++;
                MaxHealth += 20;  // Increase max health on level up
                Health = MaxHealth;  // Heal to full on level up
                AttackPower += 5;  // Increase attack power on level up
                Stats.UpdateHighestLevel(Level);
                return true;
            }
            return false;
        }

        public override void Attack(Creature.Creature target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Cannot attack null target.");

            int damage = AttackPower;
            target.TakeDamage(damage);

            if (target is Monster)
            {
                Stats.AddDamageDealt(damage);
                if (!target.IsAlive())
                {
                    Stats.AddMonsterDefeated();
                }
            }
        }

        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);
            Stats.AddDamageTaken(amount);
            if (!IsAlive())
            {
                Stats.AddDeath();
            }
        }

        /// <summary>
        /// Creates and returns the string of a player health bar.
        /// </summary>
        public string displayHealthBar() {
            // 
            var healthBar = "[";
            var healthRatio = (double)Health / MaxHealth;
            var filledBars = (int)Math.Floor(healthRatio * 20);
            var emptyBars = 20 - filledBars;

            healthBar += new string('|', filledBars); // Adds all the filled bars
            healthBar += new string('-', emptyBars);  // Adds all the empty bars
            healthBar += "]";

            return healthBar;
        }

        /// <summary>
        /// Adds an item to the player's inventory.
        /// </summary>
        /// <param name="item">The item to pick up.</param>
        public void PickUpItem(Item.Item item)
        {
            if (item == null)
            {
                return;
            }

            inventory.Add(item);
            Stats.AddItemCollected();
            Console.WriteLine($"{Name} picked up {item.Name}");
        }

        /// <summary>
        /// Uses an item from the player's inventory.
        /// </summary>
        /// <param name="item">The item to use.</param>
        public void UseItem(Item.Item item)
        {
            if (item is Weapon weapon)
            {
                // Check if the weapon is already equipped
                if (Equipment.Contains(weapon))
                {
                    Console.WriteLine($"{Name} already has {weapon.Name} equipped!");
                    return;
                }

                // Equip the weapon
                Equipment.Add(weapon);
                AttackPower += weapon.BaseDamage; // Increase attack power
                Console.WriteLine($"{Name} equipped {weapon.Name}!");
            }
            else if (item is HealthPotion potion)
            {
                // Use health potions
                potion.Use(this);
                Console.WriteLine($"{Name} used {potion.Name}!");
                RemoveItem(item); // Remove the potion from the inventory
            }
            else
            {
                Console.WriteLine($"{Name} cannot use {item.Name}.");
            }
        }

        /// <summary>
        /// Gets the contents of the player's inventory.
        /// </summary>
        /// <returns>A list of items in the inventory.</returns>
        public List<Item.Item> InventoryContents()
        {
            return inventory;
        }

        /// <summary>
        /// Removes an item from the player's inventory.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(Item.Item item)
        {
            inventory.Remove(item);
            Console.WriteLine($"{item.Name} removed from inventory.");
        }

        // LINQ example: Get all weapons from inventory
        public IEnumerable<Item.Item> GetWeapons()
        {
            return inventory.Where(item => item is Item.Items.Weapon);
        }

        // LINQ example: Get all potions from inventory
        public IEnumerable<Item.Item> GetPotions()
        {
            return inventory.Where(item => item is Item.Items.HealthPotion);
        }

        public List<Item.Item> GetInventory()
        {
            return inventory;
        }

        public void UseItem(int itemIndex, Creature.Creature target)
        {
            if (itemIndex >= 0 && itemIndex < inventory.Count)
            {
                inventory[itemIndex].Use(target);
            }
        }

        public void RemoveItem(int itemIndex)
        {
            if (itemIndex >= 0 && itemIndex < inventory.Count)
            {
                inventory.RemoveAt(itemIndex);
            }
        }

        public string GetFormattedInventory()
        {
            if (!inventory.Any())
            {
                return "Your inventory is empty.";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=== Inventory ===");
            for (int i = 0; i < inventory.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {inventory[i].Name} (ID: {inventory[i].Id})");
            }
            return sb.ToString();
        }
    }
}
