using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }
    
    private void Awake()
    {
        // Singleton pattern - ensure only one SceneManager exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep this object across scene loads
    }

    // Load scene by name
    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene name is null or empty!");
            return;
        }

        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
    
    // Load scene asynchronously by name
    public void LoadSceneAsync(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene name is null or empty!");
            return;
        }
        
        Debug.Log($"Loading scene asynchronously: {sceneName}");
        SceneManager.LoadSceneAsync(sceneName);
    }
    
    // Reload current scene
    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Reloading current scene: {currentSceneName}");
        SceneManager.LoadScene(currentSceneName);
    }
    
    // Load next scene in build index
    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"Loading next scene at index: {nextIndex}");
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("No next scene available!");
        }
    }
    
    // Load previous scene in build index
    public void LoadPreviousScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int previousIndex = currentIndex - 1;
        
        if (previousIndex >= 0)
        {
            Debug.Log($"Loading previous scene at index: {previousIndex}");
            SceneManager.LoadScene(previousIndex);
        }
        else
        {
            Debug.LogWarning("No previous scene available!");
        }
    }
    
    // Quit application
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    // Get current scene name
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
