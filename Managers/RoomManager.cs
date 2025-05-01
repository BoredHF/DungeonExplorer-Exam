using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonExplorer.Creature;
using DungeonExplorer.Item.Items;
using DungeonExplorer.Player;
using DungeonExplorer.Room;
using DungeonExplorer.GameMap;

namespace DungeonExplorer.Managers {
    /// <summary>
    /// Manages the rooms in the game.
    /// </summary>
    public class RoomManager {
        private GameMap.GameMap gameMap;

        /// <summary>
        /// Gets the current level number.
        /// </summary>
        public int CurrentLevel => gameMap.CurrentLevel;

        /// <summary>
        /// Gets whether the boss of the current level has been defeated.
        /// </summary>
        public bool IsBossDefeated => gameMap.IsBossDefeated;

        /// <summary>
        /// Initializes a new instance of the RoomManager class.
        /// </summary>
        public RoomManager()
        {
            gameMap = new GameMap.GameMap();
        }

        /// <summary>
        /// Gets the current room the player is in.
        /// </summary>
        /// <returns>The current room.</returns>
        public Room.Room GetCurrentRoom()
        {
            return gameMap.CurrentRoom;
        }

        /// <summary>
        /// Moves the player in the specified direction.
        /// </summary>
        /// <param name="direction">The direction to move.</param>
        /// <param name="player">The player to move.</param>
        /// <returns>True if the move was successful, false otherwise.</returns>
        public bool MovePlayer(string direction, DungeonExplorer.Player.Player player)
        {
            return gameMap.MovePlayer(direction);
        }

        /// <summary>
        /// Displays the map.
        /// </summary>
        public void DisplayMap()
        {
            gameMap.DisplayMap();
        }

        /// <summary>
        /// Gets the room at the specified position.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="col">The column index.</param>
        /// <returns>The room at the specified position, or null if the position is invalid.</returns>
        public Room.Room GetRoom(int row, int col)
        {
            return gameMap.GetRoom(row, col);
        }

        /// <summary>
        /// Checks if a room has been visited.
        /// </summary>
        /// <param name="row">The row index.</param>
        /// <param name="col">The column index.</param>
        /// <returns>True if the room has been visited, false otherwise.</returns>
        public bool IsRoomVisited(int row, int col)
        {
            return gameMap.IsRoomVisited(row, col);
        }

        /// <summary>
        /// Marks the boss as defeated and advances to the next level if possible.
        /// </summary>
        /// <returns>True if the level was advanced, false otherwise.</returns>
        public bool DefeatBossAndAdvance()
        {
            gameMap.DefeatBoss();
            return gameMap.AdvanceLevel();
        }
    }
}