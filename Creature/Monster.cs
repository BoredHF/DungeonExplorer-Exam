using System;

namespace DungeonExplorer.Creature
{
    public class Monster : Creature
    {
        public MonsterType Type { get; private set; }
        public int ExperienceValue { get; private set; }

        public Monster(string name, MonsterType type, int maxHealth, int attackPower, int experienceValue)
            : base(name, maxHealth, attackPower)
        {
            Type = type;
            ExperienceValue = experienceValue;
        }

        public override void Attack(Creature target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Cannot attack null target.");

            // Different monster types can have different attack patterns
            int damage = AttackPower;
            switch (Type)
            {
                case MonsterType.Goblin:
                    damage = AttackPower; // Basic attack
                    break;
                case MonsterType.Orc:
                    damage = (int)(AttackPower * 1.5); // Stronger attack
                    break;
                case MonsterType.Dragon:
                    damage = AttackPower * 2; // Much stronger attack
                    break;
            }
            
            target.TakeDamage(damage);
        }

        public override double GetDropRate() {

            // Monster that are higher difficulty will drop more often.
            switch (Type)
            {
                case MonsterType.Goblin:
                    return 0.2; // Low drop 
                case MonsterType.Orc:
                    return 0.4; // Sightly higher drop rate
                case MonsterType.Dragon:
                    return 0.6; // Much higher drop rate
                default:
                    return 0.2;
            }

        }

        /// <summary>
        /// Determines if the monster should attack the player based on its health and type.
        /// </summary>
        /// <param name="player">The player to consider attacking.</param>
        /// <returns>True if the monster should attack, false otherwise.</returns>
        public bool ShouldAttack(Player.Player player)
        {
            // Different monster types have different attack behaviors
            switch (Type)
            {
                case MonsterType.Goblin:
                    // Goblins are cowardly and might not attack if they're low on health
                    return Health > MaxHealth * 0.3;
                case MonsterType.Orc:
                    // Orcs are aggressive and will attack unless very low on health
                    return Health > MaxHealth * 0.2;
                case MonsterType.Dragon:
                    // Dragons are fearless and will always attack
                    return true;
                default:
                    return Health > MaxHealth * 0.3;
            }
        }

        /// <summary>
        /// Determines if the monster should flee based on its health and type.
        /// </summary>
        /// <returns>True if the monster should flee, false otherwise.</returns>
        public bool ShouldFlee()
        {
            // Different monster types have different flee behaviors
            switch (Type)
            {
                case MonsterType.Goblin:
                    // Goblins flee when below 30% health
                    return Health < MaxHealth * 0.3;
                case MonsterType.Orc:
                    // Orcs are more stubborn and only flee when below 20% health
                    return Health < MaxHealth * 0.2;
                case MonsterType.Dragon:
                    // Dragons never flee
                    return false;
                default:
                    return Health < MaxHealth * 0.3;
            }
        }
    }

    public enum MonsterType
    {
        Goblin,
        Orc,
        Dragon
    }
} 