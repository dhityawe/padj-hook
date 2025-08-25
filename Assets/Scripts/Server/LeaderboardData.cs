using System;
using System.Collections.Generic;

namespace Assets.Scripts.Server
{
    [Serializable]
    public class LeaderboardEntry
    {
        public string username;
        public int score;
    }

    [Serializable]
    public class LeaderboardResponse
    {
        public List<LeaderboardEntry> data;
        public string message;
        public bool success;
    }
}
