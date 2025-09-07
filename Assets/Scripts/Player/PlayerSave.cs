using UnityEngine;

public static class PlayerSave
{
    private const string UsernameKey = "username";
    private const string ScoreKey = "score";

    public static void SavePlayerData(string username, int score)
    {
        PlayerPrefs.SetString(UsernameKey, username);
        PlayerPrefs.SetInt(ScoreKey, score);
        PlayerPrefs.Save();
        Debug.Log($"Player data saved: {username} with score {score}");
    }

    public static string GetUsername() => PlayerPrefs.GetString(UsernameKey, "");
    public static int GetScore() => PlayerPrefs.GetInt(ScoreKey, 0);
}
