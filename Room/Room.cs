using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer.Item.Items;

namespace DungeonExplorer.Room
{
    /// <summary>
    /// Represents a room in the game.
    /// </summary>
    public class Room {
        private string description;
        private RoomType roomType;
        private List<Item.Item> items; // List of items in the room

        /// <summary>
        /// Initializes a new instance of the Room class.
        /// </summary>
        /// <param name="description">The description of the room.</param>
        /// <param name="roomType">The type of the room.</param>
        public Room(string description, RoomType roomType)
        {
            switch (roomType)
            {
                case RoomType.Safe:
                    description += " | SAFE ZONE |";
                    break;
                case RoomType.Normal:
                    description += " | NORMAL ROOM |";
                    break;
                case RoomType.Boss:
                    description += " | BOSS ROOM |";
                    break;
                case RoomType.Event:
                    description += " | EVENT ROOM |";
                    break;
                default:
                    break;
            }

            this.description = description;
            this.roomType = roomType;
            this.items = new List<Item.Item>(); // Initialize the list of items
        }

        /// <summary>
        /// Gets the description of the room.
        /// </summary>
        /// <returns>The room description.</returns>
        public string GetDescription()
        {
            return description;
        }

        /// <summary>
        /// Gets the type of the room.
        /// </summary>
        /// <returns>The room type.</returns>
        public RoomType getRoomType()
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

        /// <summary>
        /// Adds an item to the room.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(Item.Item item)
        {
            items.Add(item);
        }

        /// <summary>
        /// Handles the player entering the room.
        /// </summary>
        /// <param name="player">The player entering the room.</param>
        public void EnterRoom(Player.Player player)
        {
            Console.WriteLine("You entered a " + roomType.ToString().ToLower() + " room.");
            if (items.Count > 0)
            {
                Console.WriteLine("You found the following items:");
                foreach (var item in items)
                {
                    Console.WriteLine($"- {item.Name} (ID: {item.Id})");
                    player.PickUpItem(item); // Player picks up the item
                }
                items.Clear(); // Clear items after they are picked up
            }
        }
    }
}
