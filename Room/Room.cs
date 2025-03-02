using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer.Item.Items;

namespace DungeonExplorer.Room
{
    public class Room
    {
        private string description;
        private RoomType roomType;

        public Room(string description, RoomType roomType) {
            // Implement additional text formatting for the type of room
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

                case RoomType.Shop:
                    description += " | SHOP ROOM |";
                    break;

                case RoomType.Event:
                    description += " | EVENT ROOM |";
                    break;
                default:
                    break;

            }

            this.description = description;

            this.roomType = roomType;
        }


        public string GetDescription() {

            return description;
        }

        public RoomType getRoomType() {
            return roomType;
        }

        public void EnterRoom(Player.Player player)
        {
            switch (roomType)
            {
                case RoomType.Event:
                    player.PickUpItem(new HealthPotion());
                    Console.WriteLine("You found a treasure room! You receive a health potion.");
                    break;
                case RoomType.None:
                    Console.WriteLine("You hit a wall.");
                    break;
                default:
                    Console.WriteLine("You entered a " + roomType.ToString().ToLower() + " room.");
                    break;
            }
        }
    }
}
