using System;
using Assets.Scripts.Server;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavePlayerUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField _usernameInputField;
    [SerializeField]
    private Button _saveButton;
    [SerializeField]
    private int _randomScore = 0;
    [SerializeField]
    private ApiClient _apiClient;
    public static event Action OnRegisterPlayer;

    private void Start()
    {
        _randomScore = UnityEngine.Random.Range(0, 1000); // Example score generation
    }

    public void HandleSaveButtonClickedAsync()
    {
        string username = _usernameInputField.text;
        if (string.IsNullOrWhiteSpace(username))
        {
            Debug.LogWarning("Username cannot be empty.");
            return;
        }

        PlayerSave.SavePlayerData(username, PlayerPrefs.HasKey("score") ? PlayerPrefs.GetInt("score") : 0);
        Debug.Log($"Player data saved: {username} with score {_randomScore}");
        StartCoroutine(_apiClient.PostPlayer(username, PlayerPrefs.GetInt("score"), _saveButton));
        OnRegisterPlayer?.Invoke();
    }
}
