namespace DungeonExplorer.Interfaces
{
    public interface ICollectible
    {
        string Name { get; }
        string Description { get; }
        bool IsCollected { get; set; }
        void OnCollect();
    }
} 