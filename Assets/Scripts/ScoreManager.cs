using TMPro;
using UnityEngine;
using Assets.Scripts.Server;

public class ScoreManager : MonoBehaviour
{
    private int currentScore = 0;
    private int highScore;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;
    [SerializeField] private ApiClient apiClient;

    void OnEnable()
    {
        Enemy.AddScore += AddScore;
    }

    void Start()
    {
        highScore = PlayerSave.GetScore();
        AudioManager.Instance.PlayMusic(8, 0.8f, true);
        
        // Get ApiClient component if not assigned
        if (apiClient == null)
        {
            apiClient = FindObjectOfType<ApiClient>();
        }
    }

    void OnDisable()
    {
        Enemy.AddScore -= AddScore;
    }

    public void AddScore()
    {
        currentScore++;
        scoreText.text = currentScore.ToString();
    }

    public void SaveScore()
    {
        bool isNewHighScore = currentScore >= highScore;
        
        if (isNewHighScore)
        {
            PlayerPrefs.SetInt("score", currentScore);
            highscoreText.text = currentScore.ToString();
            highScore = currentScore; // Update highScore for future comparisons
        }
        else
        {
            highscoreText.text = highScore.ToString();
        }
        PlayerPrefs.Save();
        
        // Save to leaderboard if username exists
        string username = PlayerSave.GetUsername();
        if (!string.IsNullOrEmpty(username) && apiClient != null)
        {
            Debug.Log($"Saving score to leaderboard: {username} with score {currentScore}");
            StartCoroutine(apiClient.PostToLeaderboard(username, currentScore, OnLeaderboardSaveComplete));
        }
        else if (string.IsNullOrEmpty(username))
        {
            Debug.Log("No username found, skipping leaderboard save");
        }
        else if (apiClient == null)
        {
            Debug.LogWarning("ApiClient not found, cannot save to leaderboard");
        }
    }
    
    private void OnLeaderboardSaveComplete(bool success, string message)
    {
        if (success)
        {
            Debug.Log($"✅ {message}");
        }
        else
        {
            Debug.LogWarning($"⚠️ {message}");
        }
    }
}
