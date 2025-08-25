namespace Assets.Scripts.Server
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class Leaderboard
    {
        // leaderboard async operations from https://padj-hook-api.vercel.app/api/v2/leaderboard
        public static IEnumerator GetLeaderboard(Action<List<LeaderboardEntry>> onSuccess = null, Action<string> onError = null)
        {
            string url = "https://padj-hook-api.vercel.app/api/v2/leaderboard";
            using var webRequest = UnityEngine.Networking.UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Leaderboard data: " + jsonResponse);

                try
                {
                    // The API returns a direct array, so we need to wrap it for Unity's JsonUtility
                    string wrappedJson = "{\"entries\":" + jsonResponse + "}";
                    LeaderboardArrayWrapper wrapper = JsonUtility.FromJson<LeaderboardArrayWrapper>(wrappedJson);
                    
                    if (wrapper != null && wrapper.entries != null)
                    {
                        List<LeaderboardEntry> entries = new List<LeaderboardEntry>(wrapper.entries);
                        Debug.Log($"Successfully parsed {entries.Count} leaderboard entries");
                        onSuccess?.Invoke(entries);
                    }
                    else
                    {
                        Debug.LogError("Invalid response format - no entries found");
                        onError?.Invoke("Invalid response format");
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("JSON parsing error: " + e.Message);
                    onError?.Invoke("Failed to parse leaderboard data");
                }
            }
            else if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network error: " + webRequest.error);
                onError?.Invoke("Network error: " + webRequest.error);
            }
            else
            {
                Debug.LogError("Error fetching leaderboard: " + webRequest.error);
                onError?.Invoke("Error fetching leaderboard: " + webRequest.error);
            }
        }
    }
}
