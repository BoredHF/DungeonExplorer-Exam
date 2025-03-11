using System;
using DungeonExplorer.Player;

namespace DungeonExplorer.Item.Items {
    /// <summary>
    /// Represents a health potion item.
    /// </summary>
    public class HealthPotion : Item {
        private const int HealthRegen = 2;

        /// <summary>
        /// Initializes a new instance of the HealthPotion class.
        /// </summary>
        public HealthPotion() : base("Health Potion", "Restores a small amount of health.", true)
        {
            // ID is automatically assigned in the base class constructor
        }

        /// <summary>
        /// Uses the health potion on the player.
        /// </summary>
        /// <param name="player">The player to use the potion on.</param>
        public override void Use(Player.Player player)
        {
            if (!(player.Health >= player.getMaxHealth()))
            {
                Console.WriteLine("You drink the health potion and feel rejuvenated!");
                Console.WriteLine("[+2 HP]");
                player.AddHealth(HealthRegen);
                player.RemoveItem(this);
            }
            else
            {
                Console.WriteLine("You are already at full health!");
            }
        }
    }
}