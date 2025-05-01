using System.Collections.Generic;
using System.Net;
using DungeonExplorer.Interfaces;
using DungeonExplorer.Item;
using System;

namespace DungeonExplorer.Creature
{
    public abstract class Creature : IDamageable
    {
        public string Name { get; protected set; }
        public int Health { get; set; }
        public int MaxHealth { get; protected set; }
        public int AttackPower { get; set; }
        public List<Item.Item> Inventory { get; protected set; }
        public List<Item.Item> Equipment { get; protected set; }

        protected Creature(string name, int maxHealth, int attackPower)
        {
            Name = name;
            MaxHealth = maxHealth;
            Health = MaxHealth;
            AttackPower = attackPower;
            Inventory = new List<Item.Item>();
            Equipment = new List<Item.Item>();
        }

        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health < 0) Health = 0;
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public virtual void Attack(Creature target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Cannot attack null target.");
            
            target.TakeDamage(AttackPower);
        }

        public virtual double GetDropRate() {
            return 0.1;
        }


        public virtual int Heal(int amount)
        {
            int oldHealth = Health;
            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth;
            return Health - oldHealth;
        }
    }
} 