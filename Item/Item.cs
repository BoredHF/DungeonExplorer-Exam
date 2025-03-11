

namespace DungeonExplorer.Item {
    /// <summary>
    /// Represents an abstract item in the game.
    /// </summary>
    public abstract class Item {
        private static int nextId = 1;

        public string Name { get; }
        public string Description { get; }
        public int Id { get; }
        public bool Useable { get; }

        /// <summary>
        /// Initializes a new instance of the Item class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="description">The description of the item.</param>
        /// <param name="useable">Indicates whether the item is useable.</param>
        protected Item(string name, string description, bool useable)
        {
            Name = name;
            Description = description;
            Useable = useable;
            Id = nextId++;
        }

        /// <summary>
        /// Uses the item on the player.
        /// </summary>
        /// <param name="player">The player to use the item on.</param>
        public abstract void Use(Player.Player player);
    }
}
