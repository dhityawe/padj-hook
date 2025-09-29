using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameOver : MonoBehaviour
{
    public void RestartGame()
    {
        GameSceneManager.Instance.ReloadCurrentScene();
    }

    public void QuitToMainMenu()
    {
        GameSceneManager.Instance.LoadScene("MenuScene");
    }
}
