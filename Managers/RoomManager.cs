using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer.Item.Items;
using DungeonExplorer.Player;
using DungeonExplorer.Room;

namespace DungeonExplorer.Managers {
    /// <summary>
    /// Manages the rooms in the game.
    /// </summary>
    public class RoomManager {
        private Room.Room[,] rooms;
        private bool[,] visitedRooms;
        private int currentRow;
        private int currentCol;
        private string[,] levelLayout;

        /// <summary>
        /// Initializes a new instance of the RoomManager class.
        /// </summary>
        public RoomManager()
        {
            InitializeRooms();
        }

        /// <summary>
        /// Initializes the rooms for the first level.
        /// </summary>
        private void InitializeRooms()
        {
            levelLayout = new string[,]
            {
                        { "T", "#", "B" },
                        { "N", "#", "N" },
                        { "N", "E", "N" }
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
                        if (roomType == RoomType.Event)
                        {
                            rooms[row, col].AddItem(new HealthPotion());
                        }
                    }
                }
            }

            currentRow = 0;
            currentCol = 0;
            visitedRooms[currentRow, currentCol] = true;
        }

        /// <summary>
        /// Gets the room type from a character.
        /// </summary>
        /// <param name="cell">The character representing the room type.</param>
        /// <returns>The room type.</returns>
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
                case "E":
                    return RoomType.Event;
                case "#":
                    return RoomType.None; // Wall
                default:
                    return RoomType.None;
            }
        }

        /// <summary>
        /// Gets the current room the player is in.
        /// </summary>
        /// <returns>The current room.</returns>
        public Room.Room GetCurrentRoom()
        {
            return rooms[currentRow, currentCol];
        }

        /// <summary>
        /// Moves the player in the specified direction.
        /// </summary>
        /// <param name="direction">The direction to move.</param>
        /// <param name="player">The player to move.</param>
        /// <returns>True if the move is successful, false otherwise.</returns>
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

        /// <summary>
        /// Displays the map using a 2D array and the current player position.
        /// </summary>
        public void DisplayMap()
        {
            int rows = levelLayout.GetLength(0);
            int cols = levelLayout.GetLength(1);

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

            Console.WriteLine("+" + new string('-', cols * 2) + "+");
            Console.WriteLine(" "); // Spacing
        }
    }
}