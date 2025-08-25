using UnityEngine;
using TMPro;
using Assets.Scripts.Server;

namespace Assets.Scripts.UI
{
    public class LeaderboardEntryUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI usernameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        
        [Header("Default Values")]
        [SerializeField] private string defaultUsername = "???";
        [SerializeField] private string defaultScore = "???";
        
        public void SetEntry(LeaderboardEntry entry)
        {
            if (entry == null)
            {
                SetDefaultValues();
                return;
            }
            
            // Set username - handle null or empty username
            if (string.IsNullOrEmpty(entry.username))
            {
                usernameText.text = defaultUsername;
            }
            else
            {
                usernameText.text = entry.username;
            }
            
            // Set score
            scoreText.text = entry.score.ToString();
        }
        
        public void SetDefaultValues()
        {
            usernameText.text = defaultUsername;
            scoreText.text = defaultScore;
        }
        
        public void Clear()
        {
            SetDefaultValues();
        }
    }
}
