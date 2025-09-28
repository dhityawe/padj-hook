using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int currentScore = 0;
    private int highScore;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highscoreText;

    void OnEnable()
    {
        Enemy.AddScore += AddScore;
    }

    void Start()
    {
        highScore = PlayerPrefs.HasKey("score") ? PlayerPrefs.GetInt("score") : 0;
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
        if (currentScore >= highScore)
        {
            PlayerPrefs.SetInt("score", currentScore);
            highscoreText.text = currentScore.ToString();
        }
        else
        {
            highscoreText.text = highScore.ToString();
        }
    }
}
