using System;
using DungeonExplorer.Player;

namespace DungeonExplorer.Item.Items {
    public class HealthPotion : Item {
        private const int HealthRegen = 2;

        public HealthPotion() : base("Health Potion", "Restores a small amount of health.", true)
        {
            // ID is automatically assigned in the base class constructor
        }

        // Method to use the item on a player.
        public override void Use(Player.Player player)
        {

            if(!(player.Health >= player.getMaxHealth())) {
                Console.WriteLine("You drink the health potion and feel rejuvenated!");
                Console.WriteLine("[+2 HP]");
                player.AddHealth(HealthRegen);

                // remove the item from the player's inventory
                player.RemoveItem(this);


            }
            else
            {
                Console.WriteLine("You are already at full health!");
            }
        }
    }
}