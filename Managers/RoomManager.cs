using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer.Player;
using DungeonExplorer.Room;

namespace DungeonExplorer.Managers {
    public class RoomManager {
        private Room.Room[,] rooms;
        private bool[,] visitedRooms;
        private int currentRow;
        private int currentCol;
        private string[,] levelLayout;

        // Creating the first level of the dungeon
        public RoomManager()
        {
            InitializeRooms();
        }

        private void InitializeRooms()
        {
            // Initialize rooms for the first level using a 2D array
            levelLayout = new string[,]
            {
                    { "S", "B", "N" },
                    { "N", "#", "N" },
                    { "N", "T", "N" }
            };

            int rows = levelLayout.GetLength(0);
            int cols = levelLayout.GetLength(1);
            rooms = new Room.Room[rows, cols];
            visitedRooms = new bool[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    string cell = levelLayout[row, col];
                    RoomType roomType = GetRoomTypeFromChar(cell);
                    if (roomType != RoomType.None)
                    {
                        rooms[row, col] = new Room.Room($"Room ({row},{col})", roomType);
                    }
                }
            }

            // Start the player in the first room (0,0)
            currentRow = 0;
            currentCol = 0;
            visitedRooms[currentRow, currentCol] = true;
        }

        private RoomType GetRoomTypeFromChar(string cell)
        {
            switch (cell)
            {
                case "B":
                    return RoomType.Boss;
                case "N":
                    return RoomType.Normal;
                case "T":
                    return RoomType.Safe;
                case "S":
                    return RoomType.Shop;
                case "E":
                    return RoomType.Event;
                case "#":
                    return RoomType.None; // Wall
                default:
                    return RoomType.None;
            }
        }

        public Room.Room GetCurrentRoom()
        {
            return rooms[currentRow, currentCol];
        }

        public bool MovePlayer(string direction, Player.Player player)
        {
            int newRow = currentRow;
            int newCol = currentCol;

            switch (direction.ToLower())
            {
                case "up":
                    newRow--;
                    break;
                case "down":
                    newRow++;
                    break;
                case "left":
                    newCol--;
                    break;
                case "right":
                    newCol++;
                    break;
                default:
                    Console.WriteLine("Invalid direction. Use 'up', 'down', 'left', or 'right'.");
                    return false;
            }

            if (newRow >= 0 && newRow < rooms.GetLength(0) && newCol >= 0 && newCol < rooms.GetLength(1) && rooms[newRow, newCol] != null)
            {
                currentRow = newRow;
                currentCol = newCol;
                visitedRooms[currentRow, currentCol] = true;
                rooms[currentRow, currentCol].EnterRoom(player); // Trigger room behavior
                return true;
            }
            else
            {
                Console.WriteLine("You can't move in that direction.");
                return false;
            }
        }

        // Display map using 2D array and current player position (P)
        public void DisplayMap()
        {
            int rows = levelLayout.GetLength(0);
            int cols = levelLayout.GetLength(1);

            // Print top border
            Console.WriteLine(" "); // Spacing
            Console.WriteLine("+" + new string('-', cols * 2) + "+");

            for (int row = 0; row < rows; row++)
            {
                Console.Write("|"); // Left border
                for (int col = 0; col < cols; col++)
                {
                    if (row == currentRow && col == currentCol)
                    {
                        Console.Write("P "); // Player's current position
                    }
                    else if (levelLayout[row, col] == "#")
                    {
                        Console.Write("# "); // Wall
                    }
                    else if (visitedRooms[row, col])
                    {
                        Console.Write(levelLayout[row, col] + " ");
                    }
                    else
                    {
                        Console.Write("? ");
                    }
                }
                Console.WriteLine("|"); // Right border
            }

            // Print bottom border
            Console.WriteLine("+" + new string('-', cols * 2) + "+");
            Console.WriteLine(" "); // Spacing
        }
    }
}