using UnityEngine;
using TMPro;

namespace Assets.Scripts.UI
{
    /// <summary>
    /// Helper script to assist with setting up leaderboard UI components
    /// This script can be used to automatically find and assign TextMeshPro components
    /// </summary>
    public class LeaderboardSetupHelper : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private bool autoSetupOnStart = false;
        [SerializeField] private string usernameTextPrefix = "Username";
        [SerializeField] private string scoreTextPrefix = "Score";
        
        [Header("Manual Assignment")]
        [SerializeField] private TextMeshProUGUI[] usernameTexts;
        [SerializeField] private TextMeshProUGUI[] scoreTexts;
        
        void Start()
        {
            if (autoSetupOnStart)
            {
                AutoSetupLeaderboardEntries();
            }
        }
        
        [ContextMenu("Auto Setup Leaderboard Entries")]
        public void AutoSetupLeaderboardEntries()
        {
            // Find all child objects with TextMeshProUGUI components
            TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>();
            
            if (allTexts.Length == 0)
            {
                Debug.LogWarning("No TextMeshProUGUI components found in children");
                return;
            }
            
            // Separate username and score texts based on naming convention
            var usernames = new System.Collections.Generic.List<TextMeshProUGUI>();
            var scores = new System.Collections.Generic.List<TextMeshProUGUI>();
            
            foreach (var text in allTexts)
            {
                if (text.name.ToLower().Contains(usernameTextPrefix.ToLower()))
                {
                    usernames.Add(text);
                }
                else if (text.name.ToLower().Contains(scoreTextPrefix.ToLower()))
                {
                    scores.Add(text);
                }
            }
            
            // Create LeaderboardEntryUI components
            int entryCount = Mathf.Min(usernames.Count, scores.Count);
            
            for (int i = 0; i < entryCount; i++)
            {
                // Create a new GameObject for this entry
                GameObject entryGO = new GameObject($"LeaderboardEntry_{i + 1}");
                entryGO.transform.SetParent(transform);
                
                // Add LeaderboardEntryUI component
                LeaderboardEntryUI entryUI = entryGO.AddComponent<LeaderboardEntryUI>();
                
                // Assign the text components (this would need to be done manually in the inspector)
                Debug.Log($"Created LeaderboardEntry_{i + 1} - Please assign username and score TextMeshProUGUI components in the inspector");
            }
            
            Debug.Log($"Auto setup complete. Created {entryCount} leaderboard entries. Please assign the TextMeshProUGUI components in the inspector.");
        }
        
        [ContextMenu("Clear All Leaderboard Entries")]
        public void ClearAllLeaderboardEntries()
        {
            LeaderboardEntryUI[] entries = GetComponentsInChildren<LeaderboardEntryUI>();
            foreach (var entry in entries)
            {
                if (entry != null)
                {
                    DestroyImmediate(entry.gameObject);
                }
            }
            Debug.Log("Cleared all leaderboard entries");
        }
    }
}
