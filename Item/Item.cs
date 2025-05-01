using System;
using DungeonExplorer.Creature;

namespace DungeonExplorer.Item
{
    /// <summary>
    /// Represents an abstract item in the game.
    /// </summary>
    public abstract class Item
    {
        private static int nextId = 1;

        public int Id { get; }
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public bool Useable { get; protected set; }
        public int SlotWeight { get; protected set; }
        public bool IsStackable { get; protected set; }
        public int MaxStackSize { get; protected set; }
        public int CurrentStackSize { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the Item class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="description">The description of the item.</param>
        /// <param name="slotWeight">The weight of the item in inventory slots.</param>
        /// <param name="isStackable">Whether the item can be stacked with other items.</param>
        /// <param name="maxStackSize">The maximum stack size of the item.</param>
        /// <exception cref="ArgumentException">Thrown when name is null or empty, or when slotWeight or maxStackSize is less than 1.</exception>
        protected Item(string name, string description = "", int slotWeight = 1, bool isStackable = false, int maxStackSize = 1)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Item name cannot be null or empty.", nameof(name));
            
            if (slotWeight < 1)
                throw new ArgumentException("Slot weight must be at least 1.", nameof(slotWeight));
            
            if (maxStackSize < 1)
                throw new ArgumentException("Maximum stack size must be at least 1.", nameof(maxStackSize));

            Id = nextId++;
            Name = name;
            Description = description ?? "";
            Useable = false;  // Default to false, derived classes can change this
            SlotWeight = slotWeight;
            IsStackable = isStackable;
            MaxStackSize = maxStackSize;
            CurrentStackSize = 1;
        }

        /// <summary>
        /// Uses the item on the target creature.
        /// </summary>
        /// <param name="target">The creature to use the item on.</param>
        /// <exception cref="ItemException">Thrown when the item cannot be used or when the target is invalid.</exception>
        public abstract void Use(Creature.Creature target);

        /// <summary>
        /// Checks if this item can be stacked with another item.
        /// </summary>
        /// <param name="other">The other item to check stacking compatibility with.</param>
        /// <returns>True if the items can be stacked, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        public virtual bool CanStack(Item other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Cannot check stacking with null item.");

            if (!IsStackable || !other.IsStackable) return false;
            if (GetType() != other.GetType()) return false;
            if (CurrentStackSize >= MaxStackSize) return false;
            return Name == other.Name;
        }

        /// <summary>
        /// Attempts to stack another item with this one.
        /// </summary>
        /// <param name="other">The item to stack with this one.</param>
        /// <returns>True if stacking was successful, false otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when other is null.</exception>
        /// <exception cref="ItemException">Thrown when stacking operation fails due to invalid state.</exception>
        public virtual bool TryStack(Item other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other), "Cannot stack with null item.");

            if (!CanStack(other))
                return false;

            try
            {
                int spaceLeft = MaxStackSize - CurrentStackSize;
                int amountToAdd = Math.Min(spaceLeft, other.CurrentStackSize);
                
                CurrentStackSize += amountToAdd;
                other.CurrentStackSize -= amountToAdd;

                return true;
            }
            catch (Exception ex)
            {
                throw new ItemException($"Failed to stack items: {ex.Message}", Id, Name);
            }
        }

        /// <summary>
        /// Splits the current stack into two stacks.
        /// </summary>
        /// <param name="amount">The amount to split off into a new stack.</param>
        /// <returns>A new Item instance with the split amount, or null if splitting is not possible.</returns>
        /// <exception cref="ItemException">Thrown when the split amount is invalid or the operation fails.</exception>
        public virtual Item Split(int amount)
        {
            if (!IsStackable)
                throw new ItemException("Cannot split non-stackable item.", Id, Name);

            if (amount <= 0)
                throw new ItemException($"Split amount must be positive. Received: {amount}", Id, Name);

            if (amount >= CurrentStackSize)
                throw new ItemException($"Split amount ({amount}) must be less than current stack size ({CurrentStackSize}).", Id, Name);

            try
            {
                var newItem = (Item)MemberwiseClone();
                newItem.CurrentStackSize = amount;
                CurrentStackSize -= amount;
                return newItem;
            }
            catch (Exception ex)
            {
                throw new ItemException($"Failed to split item: {ex.Message}", Id, Name);
            }
        }

        /// <summary>
        /// Validates that the current stack size is within valid bounds.
        /// </summary>
        /// <exception cref="ItemException">Thrown when the stack size is invalid.</exception>
        protected void ValidateStackSize()
        {
            if (CurrentStackSize < 0)
                throw new ItemException("Stack size cannot be negative.", Id, Name);
            
            if (CurrentStackSize > MaxStackSize)
                throw new ItemException($"Stack size ({CurrentStackSize}) exceeds maximum ({MaxStackSize}).", Id, Name);
        }
    }
}
