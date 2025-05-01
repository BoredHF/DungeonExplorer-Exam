using System;

namespace DungeonExplorer.Item
{
    /// <summary>
    /// Custom exception class for handling item-related errors in the game.
    /// </summary>
    public class ItemException : Exception
    {
        /// <summary>
        /// Gets the ID of the item that caused the exception.
        /// </summary>
        public int? ItemId { get; }

        /// <summary>
        /// Gets the name of the item that caused the exception.
        /// </summary>
        public string ItemName { get; }

        /// <summary>
        /// Initializes a new instance of the ItemException class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ItemException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ItemException class with item details.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="itemId">The ID of the item that caused the exception.</param>
        /// <param name="itemName">The name of the item that caused the exception.</param>
        public ItemException(string message, int itemId, string itemName) 
            : base($"{message} (Item: {itemName}, ID: {itemId})")
        {
            ItemId = itemId;
            ItemName = itemName;
        }

        /// <summary>
        /// Initializes a new instance of the ItemException class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ItemException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
} 