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
        
        [Header("Loading Animation")]
        [SerializeField] private TextAnimation textAnimation;
        [SerializeField] private string loadingUsernameText = "Loading...";
        [SerializeField] private string loadingScoreText = "...";
        
        private bool isLoading = false;
        
        private void Awake()
        {
            // Get TextAnimation component if not assigned
            if (textAnimation == null)
            {
                textAnimation = GetComponent<TextAnimation>();
            }
        }
        
        public void SetEntry(LeaderboardEntry entry)
        {
            // Stop loading animation if running
            StopLoadingAnimation();
            
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
            StopLoadingAnimation();
            usernameText.text = defaultUsername;
            scoreText.text = defaultScore;
        }
        
        public void Clear()
        {
            SetDefaultValues();
        }
        
        public void StartLoadingAnimation()
        {
            if (isLoading) return;
            
            isLoading = true;
            
            // Start text animation for both username and score
            if (textAnimation != null)
            {
                textAnimation.StartAnimation(usernameText, loadingUsernameText);
                textAnimation.StartAnimation(scoreText, loadingScoreText);
            }
            else
            {
                // Fallback: just set loading text without animation
                usernameText.text = loadingUsernameText;
                scoreText.text = loadingScoreText;
            }
        }
        
        public void StopLoadingAnimation()
        {
            if (!isLoading) return;
            
            isLoading = false;
            
            if (textAnimation != null)
            {
                textAnimation.StopAnimation();
            }
        }
        
        public bool IsLoading()
        {
            return isLoading;
        }
    }
}
