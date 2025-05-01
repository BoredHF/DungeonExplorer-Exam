using System;
using System.Collections.Generic;
using System.Linq;
using DungeonExplorer.Room;

namespace DungeonExplorer.GameMap
{
    /// <summary>
    /// Represents the game map that manages the dungeon layout and room connections.
    /// </summary>
    public class GameMap
    {
        private Room.Room[,] rooms;
        private bool[,] visitedRooms;
        private int currentRow;
        private int currentCol;
        private int mapWidth;
        private int mapHeight;
        private Random random;
        private int currentLevel;
        private bool isBossDefeated;
        private Dictionary<int, string[,]> levelLayouts;

        /// <summary>
        /// Gets the current room the player is in.
        /// </summary>
        public Room.Room CurrentRoom => rooms[currentRow, currentCol];

        /// <summary>
        /// Gets the current row position.
        /// </summary>
        public int CurrentRow => currentRow;

        /// <summary>
        /// Gets the current column position.
        /// </summary>
        public int CurrentCol => currentCol;

        /// <summary>
        /// Gets the width of the map.
        /// </summary>
        public int Width => mapWidth;

        /// <summary>
        /// Gets the height of the map.
        /// </summary>
        public int Height => mapHeight;

        /// <summary>
        /// Gets the current level number.
        /// </summary>
        public int CurrentLevel => currentLevel;

        /// <summary>
        /// Gets whether the boss of the current level has been defeated.
        /// </summary>
        public bool IsBossDefeated => isBossDefeated;

        /// <summary>
        /// Initializes a new instance of the GameMap class with a default layout.
        /// </summary>
        public GameMap()
        {
            random = new Random();
            currentLevel = 1;
            isBossDefeated = false;
            InitializeLevelLayouts();
            InitializeDefaultMap();
        }

        /// <summary>
        /// Initializes the level layouts for different dungeon levels.
        /// </summary>
        private void InitializeLevelLayouts()
        {
            levelLayouts = new Dictionary<int, string[,]>
            {
                { 1, new string[,] {
                    { "T", "#", "B" },
                    { "N", "#", "N" },
                    { "N", "E", "N" }
                }},
                { 2, new string[,] {
                    { "T", "N", "N", "B" },
                    { "N", "#", "#", "N" },
                    { "N", "E", "N", "N" },
                    { "N", "N", "N", "N" }
                }},
                { 3, new string[,] {
                    { "T", "N", "N", "N", "B" },
                    { "N", "#", "#", "#", "N" },
                    { "N", "E", "N", "E", "N" },
                    { "N", "#", "#", "#", "N" },
                    { "N", "N", "N", "N", "N" }
                }}
            };
        }

        /// <summary>
        /// Initializes the map with the current level's layout.
        /// </summary>
        private void InitializeDefaultMap()
        {
            if (levelLayouts.TryGetValue(currentLevel, out string[,] layout))
            {
                InitializeMap(layout);
            }
            else
            {
                throw new InvalidOperationException($"No layout defined for level {currentLevel}");
            }
        }

        /// <summary>
        /// Initializes the map with a custom layout.
        /// </summary>
        /// <param name="layout">The 2D array representing the map layout.</param>
        private void InitializeMap(string[,] layout)
        {
            mapHeight = layout.GetLength(0);
            mapWidth = layout.GetLength(1);
            rooms = new Room.Room[mapHeight, mapWidth];
            visitedRooms = new bool[mapHeight, mapWidth];

            for (int row = 0; row < mapHeight; row++)
            {
                for (int col = 0; col < mapWidth; col++)
                {
                    string cell = layout[row, col];
                    RoomType roomType = GetRoomTypeFromChar(cell);
                    if (roomType != RoomType.None)
                    {
                        rooms[row, col] = new Room.Room($"Level {currentLevel} - Room ({row},{col})", roomType);
                        InitializeRoomContents(rooms[row, col], roomType);
                    }
                }
            }

            // Set starting position (top-left corner)
            currentRow = 0;
            currentCol = 0;
            visitedRooms[currentRow, currentCol] = true;
        }

        /// <summary>
        /// Initializes the contents of a room based on its type and level.
        /// </summary>
        private void InitializeRoomContents(Room.Room room, RoomType roomType)
        {
            switch (roomType)
            {
                case RoomType.Event:
                    // Add special items or events to event rooms
                    room.AddItem(Item.Items.HealthPotion.CreateGreaterHealingPotion());
                    // Add level-specific items
                    if (currentLevel > 1)
                    {
                        room.AddItem(Item.Items.Weapon.CreateIronSword());
                    }
                    break;
                case RoomType.Boss:
                    // Add boss monster and special items based on level
                    // TODO: Implement boss monster creation with level scaling
                    break;
                case RoomType.Normal:
                    // Add random monsters and items with level scaling
                    if (random.NextDouble() < 0.7) // 70% chance for monsters
                    {
                        // TODO: Add random monster generation with level scaling
                    }
                    if (random.NextDouble() < 0.3) // 30% chance for items
                    {
                        // TODO: Add random item generation with level scaling
                    }
                    break;
            }
        }

        /// <summary>
        /// Moves to the next level if the current level is completed.
        /// </summary>
        /// <returns>True if the level was advanced, false otherwise.</returns>
        public bool AdvanceLevel()
        {
            if (isBossDefeated && levelLayouts.ContainsKey(currentLevel + 1))
            {
                currentLevel++;
                isBossDefeated = false;
                InitializeDefaultMap();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Marks the boss as defeated.
        /// </summary>
        public void DefeatBoss()
        {
            isBossDefeated = true;
        }

        /// <summary>
        /// Gets the room type from a character.
        /// </summary>
        private RoomType GetRoomTypeFromChar(string cell)
        {
            switch (cell)
            {
                case "B": return RoomType.Boss;
                case "N": return RoomType.Normal;
                case "T": return RoomType.Safe;
                case "E": return RoomType.Event;
                case "#": return RoomType.None; // Wall
                default: return RoomType.None;
            }
        }

        /// <summary>
        /// Moves the player to a new position on the map.
        /// </summary>
        /// <param name="direction">The direction to move (up, down, left, right).</param>
        /// <returns>True if the move was successful, false otherwise.</returns>
        public bool MovePlayer(string direction)
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
                    return false;
            }

            // Check if the new position is valid
            if (IsValidPosition(newRow, newCol))
            {
                currentRow = newRow;
                currentCol = newCol;
                visitedRooms[currentRow, currentCol] = true;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a position is valid on the map.
        /// </summary>
        private bool IsValidPosition(int row, int col)
        {
            return row >= 0 && row < mapHeight &&
                   col >= 0 && col < mapWidth &&
                   rooms[row, col] != null;
        }

        /// <summary>
        /// Gets the room at the specified position.
        /// </summary>
        public Room.Room GetRoom(int row, int col)
        {
            if (IsValidPosition(row, col))
            {
                return rooms[row, col];
            }
            return null;
        }

        /// <summary>
        /// Checks if a room has been visited.
        /// </summary>
        public bool IsRoomVisited(int row, int col)
        {
            return IsValidPosition(row, col) && visitedRooms[row, col];
        }

        /// <summary>
        /// Displays the map with the current player position.
        /// </summary>
        public void DisplayMap()
        {
            Console.WriteLine();
            Console.WriteLine($"Level {currentLevel}");
            Console.WriteLine("+" + new string('-', mapWidth * 2) + "+");

            for (int row = 0; row < mapHeight; row++)
            {
                Console.Write("|");
                for (int col = 0; col < mapWidth; col++)
                {
                    if (row == currentRow && col == currentCol)
                    {
                        Console.Write("P "); // Player's current position
                    }
                    else if (rooms[row, col] == null)
                    {
                        Console.Write("# "); // Wall
                    }
                    else if (visitedRooms[row, col])
                    {
                        Console.Write(GetRoomSymbol(rooms[row, col].GetRoomType()) + " ");
                    }
                    else
                    {
                        Console.Write("? "); // Unexplored room
                    }
                }
                Console.WriteLine("|");
            }

            Console.WriteLine("+" + new string('-', mapWidth * 2) + "+");
            Console.WriteLine();
            DisplayMapLegend();
        }

        /// <summary>
        /// Gets the symbol for a room type.
        /// </summary>
        private string GetRoomSymbol(RoomType roomType)
        {
            switch (roomType)
            {
                case RoomType.Boss: return "B";
                case RoomType.Normal: return "N";
                case RoomType.Safe: return "T";
                case RoomType.Event: return "E";
                default: return "?";
            }
        }

        /// <summary>
        /// Displays the map legend.
        /// </summary>
        private void DisplayMapLegend()
        {
            Console.WriteLine("Map Legend:");
            Console.WriteLine("P - Your position");
            Console.WriteLine("B - Boss Room");
            Console.WriteLine("N - Normal Room");
            Console.WriteLine("T - Safe Room");
            Console.WriteLine("E - Event Room");
            Console.WriteLine("# - Wall");
            Console.WriteLine("? - Unexplored");
            Console.WriteLine($"Level: {currentLevel}");
            if (isBossDefeated)
            {
                Console.WriteLine("Boss Defeated!");
            }
        }
    }
} 