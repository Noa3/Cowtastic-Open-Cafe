using System;
using System.Threading.Tasks;
using _BaristaGame.Scripts.AddressablesScripts;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeTime = 2.0f;
    [SerializeField] private Color fadeColor = Color.black;

    [Header("Addressables Settings")]
    [SerializeField] private AssetLabelReference[] resourcesKeyName;

    private static LevelManager instance;
    private LocalSceneLoader _localSceneLoader;

    public static LevelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LevelManager>();
                if (instance == null)
                {
                    var go = new GameObject("LevelManager");
                    instance = go.AddComponent<LevelManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // Überprüfe ob bereits eine Instanz in der aktuellen Szene existiert
        LevelManager[] managers = FindObjectsOfType<LevelManager>();

        if (managers.Length > 1)
        {
            // Wenn mehrere Instanzen existieren, zerstöre diese
            Destroy(gameObject);
            return;
        }

        instance = this;
        InitializeComponents();
    }

    private void OnDestroy()
    {
        // Setze die Instanz zurück wenn dieses Objekt zerstört wird
        if (instance == this)
        {
            instance = null;
        }
    }

    private void InitializeComponents()
    {
        _localSceneLoader = GetComponent<LocalSceneLoader>();

        if (_localSceneLoader == null)
        {
            _localSceneLoader = gameObject.AddComponent<LocalSceneLoader>();
            Debug.Log("LocalSceneLoader component added to LevelManager.");
        }
    }

    /// <summary>
    /// Reloads the current scene with fade transition
    /// </summary>
    public static void ReloadScene()
    {
        if (ValidateInstance())
            Instance.ReloadSceneInternal();
    }

    /// <summary>
    /// Changes to the specified scene with fade transition
    /// </summary>
    public static void ChangeScene(string sceneName)
    {
        if (ValidateInstance())
            Instance.ChangeSceneInternal(sceneName);
    }

    /// <summary>
    /// Changes to the specified scene with addressable asset loading and fade transition
    /// </summary>
    public static async Task ChangeSceneWithResources(string sceneName)
    {
        if (ValidateInstance())
            await Instance.ChangeSceneWithResourcesInternal(sceneName);
    }

    /// <summary>
    /// Changes to the specified scene immediately without fade transition
    /// </summary>
    public static void ChangeSceneDirect(string sceneName)
    {
        if (ValidateInstance())
            Instance.ChangeSceneDirectInternal(sceneName);
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public static void QuitGame()
    {
        if (ValidateInstance())
            Instance.QuitGameInternal();
    }

    /// <summary>
    /// Validates that an instance exists, creates one if needed
    /// </summary>
    private static bool ValidateInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType<LevelManager>();

            if (instance == null)
            {
                Debug.LogWarning("LevelManager instance not found in scene. Creating temporary instance for scene transition.");
                var go = new GameObject("LevelManager_Temp");
                instance = go.AddComponent<LevelManager>();
                return true;
            }
        }
        return instance != null;
    }

    private void ReloadSceneInternal()
    {
        ForceResetGameState();

        try
        {
            // Ensure fade time is valid to prevent infinite black screen
            float safeFadeTime = Mathf.Max(fadeTime, 0.1f);
            Initiate.Fade(SceneManager.GetActiveScene().name, fadeColor, safeFadeTime);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Fade reload failed: {ex.Message}");
            ForceResetGameState(); // Reset again before fallback
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void ChangeSceneInternal(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name cannot be null or empty");
            return;
        }

        ForceResetGameState();

        try
        {
            // Ensure fade time is valid to prevent infinite black screen
            float safeFadeTime = Mathf.Max(fadeTime, 0.1f);
            Initiate.Fade(sceneName, fadeColor, safeFadeTime);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Fade transition failed: {ex.Message}");
            ChangeSceneDirectInternal(sceneName);
        }
    }

    private async Task ChangeSceneWithResourcesInternal(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name cannot be null or empty");
            return;
        }

        try
        {
            ForceResetGameState();

            // Stelle sicher dass LocalSceneLoader verfügbar ist
            if (_localSceneLoader == null)
            {
                InitializeComponents();
            }

            await LoadResourcesAndScene(sceneName);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to change scene with resources: {ex.Message}");
            // Fallback to direct scene loading
            ChangeSceneDirectInternal(sceneName);
        }
    }

    private void ChangeSceneDirectInternal(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name cannot be null or empty");
            return;
        }

        ForceResetGameState();

        try
        {
            SceneManager.LoadScene(sceneName);
            Statics.CleanUpGabarge();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Direct scene loading failed: {ex.Message}");
        }
    }

    private void QuitGameInternal()
    {
        Debug.Log("Quitting Game");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private async Task LoadResourcesAndScene(string nextScene)
    {
        if (resourcesKeyName == null || resourcesKeyName.Length == 0)
        {
            Debug.LogWarning("No resource keys specified. Skipping asset loading.");
            await LoadNextScene(nextScene);
            return;
        }

        var assetsLoaded = await _localSceneLoader.LoadAssetsAsync(resourcesKeyName);

        if (assetsLoaded)
        {
            await LoadNextScene(nextScene);
        }
        else
        {
            Debug.LogError("Failed to load required assets. Using direct scene loading as fallback.");
            ChangeSceneDirectInternal(nextScene);
        }
    }

    private async Task LoadNextScene(string nextScene)
    {
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogWarning("Next scene name is not specified.");
            return;
        }

        try
        {
            // Ensure game state is reset before loading
            ForceResetGameState();
            
            // Ensure fade time is valid to prevent infinite black screen
            float safeFadeTime = Mathf.Max(fadeTime, 0.1f);
            _localSceneLoader.LoadSceneAsync(nextScene, fadeColor, safeFadeTime);
            await Task.Yield();
        }
        catch (Exception ex)
        {
            Debug.LogError($"LoadSceneAsync failed: {ex.Message}");
            ChangeSceneDirectInternal(nextScene);
        }
    }

    /// <summary>
    /// Forces the game state to be reset - ensures timeScale is 1 and unpauzes any systems
    /// </summary>
    private void ForceResetGameState()
    {
        try
        {
            // Force time scale to 1 - this is critical for scene transitions
            Time.timeScale = 1f;
            
            // Additional safety: Force unpause any pause manager if it exists
            var pauseManager = FindObjectOfType<GamePauseManager>();
            if (pauseManager != null && pauseManager.GamePaused)
            {
                Debug.Log("LevelManager: Force unpausing game for scene transition");
                pauseManager.SetGamePause(false);
            }
            
            // Reset milky mode preference when changing scenes
            // This ensures milky mode doesn't persist when players exit levels or restart
            ResetMilkyModePreference();
            
            Debug.Log($"LevelManager: Game state reset - Time.timeScale set to {Time.timeScale}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"LevelManager: Error resetting game state: {ex.Message}");
            // Ensure timeScale is reset even if other operations fail
            Time.timeScale = 1f;
        }
    }

    /// <summary>
    /// Resets the milky mode preference to ensure it doesn't persist between levels
    /// </summary>
    private void ResetMilkyModePreference()
    {
        try
        {
            PlayerPrefs.DeleteKey(Consts.PlayerPrefNextIsMilkyMode);
            PlayerPrefs.Save();
            Debug.Log("LevelManager: Milky mode preference reset on scene change");
        }
        catch (Exception e)
        {
            Debug.LogError($"LevelManager: Error resetting milky mode preference: {e.Message}");
        }
    }

    /// <summary>
    /// Legacy method for backwards compatibility - now calls ForceResetGameState
    /// </summary>
    private void ResetTimeScale()
    {
        ForceResetGameState();
    }

    /// <summary>
    /// Configure fade settings at runtime
    /// </summary>
    public void SetFadeSettings(float time, Color color)
    {
        fadeTime = Mathf.Max(time, 0.1f); // Ensure minimum fade time
        fadeColor = color;
    }

    /// <summary>
    /// Configure addressable resources at runtime
    /// </summary>
    public void SetResourceKeys(AssetLabelReference[] resources)
    {
        resourcesKeyName = resources;
    }

    /// <summary>
    /// Manually refresh components if needed
    /// </summary>
    public void RefreshComponents()
    {
        InitializeComponents();
    }

    /// <summary>
    /// Public method to force reset game state - can be called externally if needed
    /// </summary>
    public static void ForceResetGameStateStatic()
    {
        if (ValidateInstance())
        {
            Instance.ForceResetGameState();
        }
        else
        {
            // Fallback if no instance exists
            Time.timeScale = 1f;
            Debug.Log("LevelManager: Force reset timeScale without instance");
        }
    }
}