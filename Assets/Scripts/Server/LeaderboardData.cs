using System;
using System.Collections.Generic;

namespace Assets.Scripts.Server
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string id;
        public string username;
        public int score;
    }

    // Wrapper class for Unity's JsonUtility to parse arrays
    [Serializable]
    public class LeaderboardArrayWrapper
    {
        public LeaderboardEntry[] entries;
    }
}
