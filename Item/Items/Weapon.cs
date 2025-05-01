using System;
using DungeonExplorer.Creature;
using DungeonExplorer.Item;


namespace DungeonExplorer.Item.Items
{
    public class Weapon : Item
    {
        private static readonly Random random = new Random();

        public enum WeaponType
        {
            Sword,
            Axe,
            Mace,
            Dagger,
            Bow,
            Staff,
            Wand
        }

        public enum WeaponRarity
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary
        }

        public int BaseDamage { get; }
        private readonly WeaponType type;
        private readonly WeaponRarity rarity;
        private readonly string lore;
        private readonly bool isTwoHanded;
        private readonly float criticalChance;
        private readonly float criticalMultiplier;

        /// <summary>
        /// Initializes a new instance of the Weapon class.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown when:
        /// - baseDamage is negative
        /// - criticalChance is not between 0 and 1
        /// - criticalMultiplier is less than 1
        /// </exception>
        public Weapon(
            string name,
            string description,
            int baseDamage,
            WeaponType type,
            WeaponRarity rarity,
            string lore,
            bool isTwoHanded = false,
            float criticalChance = 0.05f,
            float criticalMultiplier = 2.0f
        ) : base(name, description, isTwoHanded ? 2 : 1, false, 1)
        {
            if (baseDamage < 0)
                throw new ArgumentException("Base damage cannot be negative.", nameof(baseDamage));
            
            if (criticalChance < 0 || criticalChance > 1)
                throw new ArgumentException("Critical chance must be between 0 and 1.", nameof(criticalChance));
            
            if (criticalMultiplier < 1)
                throw new ArgumentException("Critical multiplier must be at least 1.", nameof(criticalMultiplier));

            BaseDamage = baseDamage;
            this.type = type;
            this.rarity = rarity;
            this.lore = lore ?? throw new ArgumentNullException(nameof(lore));
            this.isTwoHanded = isTwoHanded;
            this.criticalChance = criticalChance;
            this.criticalMultiplier = criticalMultiplier;
            Useable = true;
        }

        /// <summary>
        /// Uses the weapon to attack a target.
        /// </summary>
        /// <param name="target">The creature to attack.</param>
        /// <exception cref="ItemException">Thrown when the weapon cannot be used or when the target is invalid.</exception>
        public override void Use(Creature.Creature target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Cannot attack null target.");

            try
            {
                int damage = BaseDamage;
                bool isCritical = random.NextDouble() <= criticalChance;
                
                if (isCritical)
                {
                    damage = (int)(damage * criticalMultiplier);
                }

                target.TakeDamage(damage);
            }
            catch (Exception ex)
            {
                if (ex is ItemException)
                    throw;
                throw new ItemException($"Failed to use weapon: {ex.Message}", Id, Name);
            }
        }

        public override string ToString()
        {
            string critInfo = criticalChance > 0 ? 
                $"\nCritical: {criticalChance * 100}% chance for x{criticalMultiplier} damage" : "";
            
            return $"{Name} - {rarity} {type}\n" +
                   $"Damage: {BaseDamage}\n" +
                   $"Type: {(isTwoHanded ? "Two-handed" : "One-handed")}" +
                   critInfo + "\n" +
                   $"Lore: {lore}";
        }

        /// <summary>
        /// Creates a rusty dagger weapon.
        /// </summary>
        /// <returns>A new rusty dagger instance.</returns>
        public static Weapon CreateRustyDagger()
        {
            return new Weapon(
                "Rusty Dagger",
                "A worn dagger showing signs of neglect and age.",
                5,
                WeaponType.Dagger,
                WeaponRarity.Common,
                "A simple blade that has seen better days, but it still holds an edge.",
                false,
                0.1f,
                1.5f
            );
        }

        /// <summary>
        /// Creates an iron sword weapon.
        /// </summary>
        /// <returns>A new iron sword instance.</returns>
        public static Weapon CreateIronSword()
        {
            return new Weapon(
                "Iron Sword",
                "A standard iron sword with decent balance.",
                10,
                WeaponType.Sword,
                WeaponRarity.Common,
                "A reliable weapon favored by town guards and mercenaries alike.",
                false,
                0.05f,
                2.0f
            );
        }

        /// <summary>
        /// Creates a battle axe weapon.
        /// </summary>
        /// <returns>A new battle axe instance.</returns>
        public static Weapon CreateBattleAxe()
        {
            return new Weapon(
                "Battle Axe",
                "A heavy two-handed axe designed for devastating strikes.",
                15,
                WeaponType.Axe,
                WeaponRarity.Uncommon,
                "This fearsome weapon can cleave through armor with ease.",
                true,
                0.15f,
                2.5f
            );
        }

        /// <summary>
        /// Creates an enchanted staff weapon.
        /// </summary>
        /// <returns>A new enchanted staff instance.</returns>
        public static Weapon CreateEnchantedStaff()
        {
            return new Weapon(
                "Enchanted Staff",
                "A wooden staff humming with magical energy.",
                8,
                WeaponType.Staff,
                WeaponRarity.Rare,
                "Ancient runes carved into the wood pulse with an inner light.",
                true,
                0.2f,
                3.0f
            );
        }

        /// <summary>
        /// Creates a legendary dragonslayer weapon.
        /// </summary>
        /// <returns>A new dragonslayer instance.</returns>
        public static Weapon CreateDragonslayer()
        {
            return new Weapon(
                "Dragonslayer",
                "A legendary greatsword said to have been forged in dragon's breath.",
                25,
                WeaponType.Sword,
                WeaponRarity.Legendary,
                "This massive blade bears scorch marks that never fade, a testament to its creation.",
                true,
                0.25f,
                3.5f
            );
        }
    }
} 