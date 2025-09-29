using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField]
    private GameObject _loginPanel;
    [SerializeField]
    private GameObject _loginQuestionPanel;
    [SerializeField]
    private GameObject _leaderboardPanel;
    [SerializeField]
    private GameObject _blackOverlay;

    [SerializeField]
    private string _gameplayScene;

    public void OnYesClicked()
    {
        _loginQuestionPanel.SetActive(false);
        _loginPanel.SetActive(true);
    }
    public void OnNoClicekd()
    {
        _leaderboardPanel.SetActive(false);
    }

    private void OnEnable()
    {
        SavePlayerUI.OnRegisterPlayer += HandleRegister;
    }

    private void OnDisable()
    {
        SavePlayerUI.OnRegisterPlayer -= HandleRegister;
    }

    public void OnBackClicked()
    {
        _loginPanel.SetActive(false);
        _loginQuestionPanel.SetActive(true);
    }

    public void OnExitClicked() => Application.Quit();

    public void OnStartClicked() => SceneManager.LoadScene(_gameplayScene);

    public void OnLeaderboardClicked()
    {
        _leaderboardPanel.SetActive(true);
        _blackOverlay.SetActive(true);
        if (!PlayerPrefs.HasKey("username"))
        {
            _loginPanel.SetActive(false);
            _loginQuestionPanel.SetActive(true);
        }
        else
        {
            _loginPanel.SetActive(false);
            _blackOverlay.SetActive(false);
            _loginQuestionPanel.SetActive(false);
            _leaderboardPanel.SetActive(true);
        }
    }

    private void Start()
    {
        AudioManager.Instance.PlayMusic(9, 0.8f, true);
    }

    private void HandleRegister()
    {
        _leaderboardPanel.SetActive(false);
    }
}
