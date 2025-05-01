using System;
using System.Collections.Generic;
using System.Linq;

namespace DungeonExplorer.Player
{
    public class Statistics
    {
        private Dictionary<string, int> stats;
        private List<string> achievements;
        private DateTime gameStartTime;
        private int totalDamageDealt;
        private int totalDamageTaken;
        private int monstersDefeated;
        private int itemsCollected;
        private int roomsExplored;
        private int deaths;
        public int HighestLevel { get; private set; }
        public int TotalDamageDealt { get; private set; }
        public int MonstersDefeated { get; private set; }

        public Statistics()
        {
            stats = new Dictionary<string, int>();
            achievements = new List<string>();
            gameStartTime = DateTime.Now;
            HighestLevel = 1;
            TotalDamageDealt = 0;
            MonstersDefeated = 0;
            InitializeStats();
        }

        private void InitializeStats()
        {
            stats["TotalPlayTime"] = 0;
            stats["MonstersDefeated"] = 0;
            stats["ItemsCollected"] = 0;
            stats["RoomsExplored"] = 0;
            stats["Deaths"] = 0;
            stats["TotalDamageDealt"] = 0;
            stats["TotalDamageTaken"] = 0;
            stats["HighestLevelReached"] = 1;
        }

        public void UpdatePlayTime()
        {
            stats["TotalPlayTime"] = (int)(DateTime.Now - gameStartTime).TotalSeconds;
        }

        public void AddMonsterDefeated()
        {
            stats["MonstersDefeated"]++;
            monstersDefeated++;
            CheckAchievements();
        }

        public void AddItemCollected()
        {
            stats["ItemsCollected"]++;
            itemsCollected++;
            CheckAchievements();
        }

        public void AddRoomExplored()
        {
            stats["RoomsExplored"]++;
            roomsExplored++;
            CheckAchievements();
        }

        public void AddDeath()
        {
            stats["Deaths"]++;
            deaths++;
        }

        public void AddDamageDealt(int amount)
        {
            stats["TotalDamageDealt"] += amount;
            totalDamageDealt += amount;
            CheckAchievements();
        }

        public void AddDamageTaken(int amount)
        {
            stats["TotalDamageTaken"] += amount;
            totalDamageTaken += amount;
        }

        public void UpdateHighestLevel(int level)
        {
            if (level > stats["HighestLevelReached"])
            {
                stats["HighestLevelReached"] = level;
                CheckAchievements();
            }
        }

        private void CheckAchievements()
        {
            // Monster Slayer Achievement
            if (monstersDefeated >= 10 && !achievements.Contains("Monster Slayer"))
            {
                achievements.Add("Monster Slayer");
            }

            // Treasure Hunter Achievement
            if (itemsCollected >= 20 && !achievements.Contains("Treasure Hunter"))
            {
                achievements.Add("Treasure Hunter");
            }

            // Explorer Achievement
            if (roomsExplored >= 15 && !achievements.Contains("Explorer"))
            {
                achievements.Add("Explorer");
            }

            // Warrior Achievement
            if (totalDamageDealt >= 1000 && !achievements.Contains("Warrior"))
            {
                achievements.Add("Warrior");
            }

            // Survivor Achievement
            if (stats["HighestLevelReached"] >= 5 && !achievements.Contains("Survivor"))
            {
                achievements.Add("Survivor");
            }

            CheckLevelAchievements();
            CheckDamageAchievements();
        }

        private void CheckLevelAchievements()
        {
            if (stats["HighestLevelReached"] >= 5 && !achievements.Contains("Level 5 Reached"))
                achievements.Add("Level 5 Reached");
            if (stats["HighestLevelReached"] >= 10 && !achievements.Contains("Level 10 Reached"))
                achievements.Add("Level 10 Reached");
        }

        private void CheckDamageAchievements()
        {
            if (totalDamageDealt >= 1000 && !achievements.Contains("Dealt 1000 Damage"))
                achievements.Add("Dealt 1000 Damage");
            if (totalDamageDealt >= 5000 && !achievements.Contains("Dealt 5000 Damage"))
                achievements.Add("Dealt 5000 Damage");
        }

        public Dictionary<string, int> GetStats()
        {
            UpdatePlayTime();
            return new Dictionary<string, int>(stats);
        }

        public List<string> GetAchievements()
        {
            return new List<string>(achievements);
        }

        public string GetFormattedStats()
        {
            UpdatePlayTime();
            TimeSpan playTime = TimeSpan.FromSeconds(stats["TotalPlayTime"]);
            
            return $"=== Player Statistics ===\n" +
                   $"Play Time: {playTime.Hours}h {playTime.Minutes}m {playTime.Seconds}s\n" +
                   $"Monsters Defeated: {stats["MonstersDefeated"]}\n" +
                   $"Items Collected: {stats["ItemsCollected"]}\n" +
                   $"Rooms Explored: {stats["RoomsExplored"]}\n" +
                   $"Deaths: {stats["Deaths"]}\n" +
                   $"Total Damage Dealt: {stats["TotalDamageDealt"]}\n" +
                   $"Total Damage Taken: {stats["TotalDamageTaken"]}\n" +
                   $"Highest Level Reached: {stats["HighestLevelReached"]}\n" +
                   $"\n=== Achievements ===\n" +
                   $"{(achievements != null && achievements.Count > 0 ? string.Join("\n", achievements) : "No achievements yet")}";
        }
    }
}
