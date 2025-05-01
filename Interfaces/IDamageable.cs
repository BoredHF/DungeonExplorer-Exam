namespace DungeonExplorer.Interfaces
{
    public interface IDamageable
    {
        int Health { get; set; }
        int MaxHealth { get; }
        void TakeDamage(int damage);
        bool IsAlive();
    }
} 