using System.IO;
using UnityEngine;

namespace Scripts.Track.Results
{
    public static class LeaderboardManager
    {
        private const string FileName = "save.dat";
        
        public static void Save(Leaderboard leaderboard)
        {
            var destination = $"{Application.persistentDataPath}/{FileName}";
            File.WriteAllText(destination, JsonUtility.ToJson(leaderboard));
        }

        public static Leaderboard Load()
        {
            var destination = $"{Application.persistentDataPath}/{FileName}";
            return !File.Exists(destination) ? null : JsonUtility.FromJson<Leaderboard>(File.ReadAllText(destination));
        }
    }
}