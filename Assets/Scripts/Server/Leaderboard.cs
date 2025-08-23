namespace Assets.Scripts.Server
{
    using System.Collections;
    using UnityEngine;
    public static class Leaderboard
    {
        // leaderboard async operations from https://padj-hook-api.vercel.app/api/v2/leaderboard
        public static IEnumerator GetLeaderboard()
        {
            string url = "https://padj-hook-api.vercel.app/api/v2/leaderboard";
            using var webRequest = UnityEngine.Networking.UnityWebRequest.Get(url);
            webRequest.SetRequestHeader("Content-Type", "application/json");
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Leaderboard data: " + jsonResponse);
                // Process the JSON response as needed
            }
            else if (webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError || webRequest.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Network error: " + webRequest.error);
            }
            else
            {
                Debug.LogError("Error fetching leaderboard: " + webRequest.error);
            }
        }
    }
}
