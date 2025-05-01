using System;
using DungeonExplorer.Creature;
using DungeonExplorer.Item;

namespace DungeonExplorer.Item.Items {
    /// <summary>
    /// Represents a health potion item.
    /// </summary>
    public class HealthPotion : Item {
        public enum PotionRarity
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary
        }

        private readonly int healAmount;
        private readonly PotionRarity rarity;
        private readonly string lore;
        private readonly bool hasInstantEffect;
        private readonly int healOverTimeAmount;

        /// <summary>
        /// Initializes a new instance of the HealthPotion class.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when healAmount is negative or when healOverTimeAmount is negative.</exception>
        public HealthPotion(
            string name,
            string description,
            int healAmount,
            PotionRarity rarity,
            string lore,
            bool hasInstantEffect = true,
            int healOverTimeAmount = 0,
            bool isStackable = true,
            int maxStackSize = 5
        ) : base(name, description, 1, isStackable, maxStackSize)
        {
            if (healAmount < 0)
                throw new ArgumentException("Heal amount cannot be negative.", nameof(healAmount));
            
            if (healOverTimeAmount < 0)
                throw new ArgumentException("Heal over time amount cannot be negative.", nameof(healOverTimeAmount));

            if (!hasInstantEffect && healOverTimeAmount == 0)
                throw new ArgumentException("Potion must have either instant effect or heal over time effect.");

            this.healAmount = healAmount;
            this.rarity = rarity;
            this.lore = lore ?? throw new ArgumentNullException(nameof(lore));
            this.hasInstantEffect = hasInstantEffect;
            this.healOverTimeAmount = healOverTimeAmount;
            Useable = true;
        }

        /// <summary>
        /// Uses the health potion on the creature.
        /// </summary>
        /// <param name="target">The creature to use the potion on.</param>
        /// <exception cref="ItemException">Thrown when the potion cannot be used or when the target is invalid.</exception>
        public override void Use(Creature.Creature target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Cannot use potion on null target.");

            if (CurrentStackSize <= 0)
                throw new ItemException("Cannot use potion with empty stack.", Id, Name);

            try
            {
                if (hasInstantEffect)
                {
                    target.Heal(healAmount);
                }

                if (healOverTimeAmount > 0)
                {
                    // TODO: Implement heal over time effect
                    target.Heal(healOverTimeAmount);
                }

                CurrentStackSize--;
                ValidateStackSize();

                if (CurrentStackSize <= 0)
                {
                    // Remove from inventory if stack is empty
                    target.Inventory.Remove(this);
                }
            }
            catch (Exception ex)
            {
                if (ex is ItemException)
                    throw;
                throw new ItemException($"Failed to use potion: {ex.Message}", Id, Name);
            }
        }

        public override string ToString()
        {
            string effectDesc = hasInstantEffect ? $"Instantly heals for {healAmount}" : "";
            if (healOverTimeAmount > 0)
            {
                effectDesc += hasInstantEffect ? " and " : "";
                effectDesc += $"heals for {healOverTimeAmount} over time";
            }

            string stackInfo = IsStackable ? $" (Stack: {CurrentStackSize}/{MaxStackSize})" : "";
            
            return $"{Name} - {rarity} Potion{stackInfo}\n" +
                   $"Effect: {effectDesc}\n" +
                   $"Lore: {lore}";
        }

        /// <summary>
        /// Creates a minor healing potion.
        /// </summary>
        /// <returns>A new minor healing potion instance.</returns>
        public static HealthPotion CreateMinorHealingPotion()
        {
            return new HealthPotion(
                "Minor Healing Potion",
                "A small vial containing a red liquid that glows faintly.",
                20,
                PotionRarity.Common,
                "A common remedy brewed by apprentice alchemists, popular among novice adventurers.",
                maxStackSize: 10
            );
        }

        /// <summary>
        /// Creates a greater healing potion.
        /// </summary>
        /// <returns>A new greater healing potion instance.</returns>
        public static HealthPotion CreateGreaterHealingPotion()
        {
            return new HealthPotion(
                "Greater Healing Potion",
                "A crystal vial containing a bright crimson liquid that pulses with energy.",
                50,
                PotionRarity.Uncommon,
                "Mastered by skilled alchemists, these potions are favored by seasoned warriors.",
                maxStackSize: 5
            );
        }

        /// <summary>
        /// Creates a regeneration potion.
        /// </summary>
        /// <returns>A new regeneration potion instance.</returns>
        public static HealthPotion CreateRegenPotion()
        {
            return new HealthPotion(
                "Regeneration Potion",
                "An ornate flask containing a swirling golden liquid.",
                30,
                PotionRarity.Rare,
                "Ancient elven recipe that promotes natural healing abilities.",
                hasInstantEffect: true,
                healOverTimeAmount: 20,
                maxStackSize: 3
            );
        }

        /// <summary>
        /// Creates a divine elixir.
        /// </summary>
        /// <returns>A new divine elixir instance.</returns>
        public static HealthPotion CreateDivineElixir()
        {
            return new HealthPotion(
                "Divine Elixir",
                "A radiant vial that seems to contain pure light itself.",
                100,
                PotionRarity.Legendary,
                "Blessed by the gods themselves, these legendary elixirs are said to be able to save even those on death's door.",
                hasInstantEffect: true,
                healOverTimeAmount: 50,
                maxStackSize: 1
            );
        }
    }
}