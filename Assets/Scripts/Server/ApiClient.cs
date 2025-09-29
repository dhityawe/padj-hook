using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Server
{
    [Serializable]
    public class PlayerRequest
    {
        public string username;
        public int score;
    }
    
    public class ApiClient : MonoBehaviour
    {
        private const string BASE_URL = "https://padj-hook-api.vercel.app/api/v2";

        public IEnumerator PostPlayer(string username, int score, Button button)
        {
            PlayerRequest playerRequest = new()
            {
                username = username,
                score = score
            };
            Debug.Log($"Posting player data: {username} with score {score}");
            string json = JsonUtility.ToJson(playerRequest);
            Debug.Log("Request JSON: " + json);
            button.interactable = false; // Disable button to prevent multiple submissions

            using UnityWebRequest webRequest = new($"{BASE_URL}/player", "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    Debug.Log("Player data posted successfully: " + webRequest.downloadHandler.text);
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("Connection error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("Error posting player data: " + webRequest.error);
                    break;
                default:
                    Debug.LogError("Unexpected error: " + webRequest.error);
                    break;
            }
            button.interactable = true; // Re-enable button after request completes
        }
        
        public IEnumerator PostToLeaderboard(string username, int score, Action<bool, string> callback = null)
        {
            PlayerRequest playerRequest = new()
            {
                username = username,
                score = score
            };
            
            Debug.Log($"Posting to leaderboard: {username} with score {score}");
            string json = JsonUtility.ToJson(playerRequest);
            Debug.Log("Leaderboard Request JSON: " + json);

            using UnityWebRequest webRequest = new($"{BASE_URL}/leaderboard", "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            bool success = false;
            string message = "";

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    Debug.Log("Score posted to leaderboard successfully: " + webRequest.downloadHandler.text);
                    success = true;
                    message = "Score updated in leaderboard!";
                    break;
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("Connection error posting to leaderboard: " + webRequest.error);
                    message = "Connection error. Please check your internet connection.";
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("Error posting to leaderboard: " + webRequest.error);
                    message = "Error updating leaderboard. Please try again.";
                    break;
                default:
                    Debug.LogError("Unexpected error posting to leaderboard: " + webRequest.error);
                    message = "Unexpected error occurred.";
                    break;
            }
            
            callback?.Invoke(success, message);
        }
    }

}
