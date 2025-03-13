using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonExplorer.Player {
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player {

        public const int MAX_HEALTH = 100;
        public string Name { get; set; }
        public int Health { get; set; }
        private List<Item.Item> inventory = new List<Item.Item>();

        /// <summary>
        /// Initializes a new instance of the Player class.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="health">The initial health of the player.</param>
        public Player(string name, int health)
        {
            Name = name;
            Health = health;
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
            Console.WriteLine($"{Name} picked up {item.Name}");
        }

        /// <summary>
        /// Uses an item from the player's inventory.
        /// </summary>
        /// <param name="item">The item to use.</param>
        public void UseItem(Item.Item item)
        {
            if (InventoryContents().Contains(item))
            {
                Console.WriteLine($"{Name} uses {item.Name}");
                item.Use(this);
            }
            else
            {
                Console.WriteLine($"{Name} does not have {item.Name}");
            }
        }

        /// <summary>
        /// Reduces the player's health by the specified damage amount.
        /// </summary>
        /// <param name="damage">The amount of damage to take.</param>
        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        /// <summary>
        /// Increases the player's health by the specified amount.
        /// </summary>
        /// <param name="health">The amount of health to add.</param>
        public void AddHealth(int health)
        {
            Health += health;
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

        /// <summary>
        /// Gets the maximum health of the player.
        /// </summary>
        /// <returns>The maximum health value.</returns>
        public int getMaxHealth()
        {
            return MAX_HEALTH;
        }

    }
}
