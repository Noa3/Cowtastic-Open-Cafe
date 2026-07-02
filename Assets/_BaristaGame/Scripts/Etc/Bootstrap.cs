using System;
using System.Threading.Tasks;
using _BaristaGame.Scripts.AddressablesScripts;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bootstrap : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AssetLabelReference[] resourcesKeyName;
    [SerializeField] private string nextScene = "MainMenu";

    private const string GameVersionKey = "GameVersion";
    private LocalSceneLoader _localSceneLoader;

    private async void Awake()
    {
        try
        {
            await InitializeAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Bootstrap initialization failed: {ex.Message}");
            // Optional: Fallback-Verhalten oder Neustart
        }
    }

    private async Task InitializeAsync()
    {
        // Ensure game starts with proper time scale
        EnsureProperGameState();
        
        ValidateGameVersion();
        InitializeComponents();
        ConfigureMobileRefreshRate();
        await LoadResourcesAndScene();
    }

    /// <summary>
    /// Ensures the game starts with proper state (timeScale = 1, etc.)
    /// </summary>
    private void EnsureProperGameState()
    {
        try
        {
            // Force time scale to 1 at game start
            if (Time.timeScale != 1f)
            {
                Debug.LogWarning($"Bootstrap: Time.timeScale was {Time.timeScale} at startup, resetting to 1");
                Time.timeScale = 1f;
            }
            
            Debug.Log($"Bootstrap: Game state initialized - Time.timeScale = {Time.timeScale}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Bootstrap: Error ensuring proper game state: {ex.Message}");
            // Ensure timeScale is set even if logging fails
            Time.timeScale = 1f;
        }
    }

    private void ValidateGameVersion()
    {
#if !UNITY_EDITOR
        var currentVersion = PlayerPrefs.GetString(GameVersionKey, string.Empty);

        if (!string.Equals(currentVersion, Consts.PrefixNewGameVersion, StringComparison.Ordinal))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString(GameVersionKey, Consts.PrefixNewGameVersion);
            PlayerPrefs.Save(); // Explizit speichern

            Debug.Log($"Game version updated from '{currentVersion}' to '{Consts.PrefixNewGameVersion}'. PlayerPrefs cleared.");
        }
#endif
    }

    private void InitializeComponents()
    {
        _localSceneLoader = GetComponent<LocalSceneLoader>();

        if (_localSceneLoader == null)
        {
            // Self-heal scene/prefab mismatches after migrations.
            _localSceneLoader = gameObject.AddComponent<LocalSceneLoader>();
            Debug.LogWarning("Bootstrap: LocalSceneLoader component was missing and has been added at runtime.");
        }
    }

    /// <summary>
    /// Configures the target refresh rate for mobile devices when V-Sync is enabled.
    /// Sets the target frame rate to match the device's refresh rate for optimal performance.
    /// Also applies saved resolution and fullscreen settings on application startup.
    /// </summary>
    private void ConfigureMobileRefreshRate()
    {
        try
        {            
            // Check if we're on a mobile platform
            if (Application.isMobilePlatform)
            {
                // Get V-Sync setting from PlayerPrefs (default to disabled)
                bool vsyncEnabled = PlayerPrefs.GetInt(Consts.PlayerPrefVsync, 0) == 1;

                if (vsyncEnabled)
                {
                    // When V-Sync is enabled, set target frame rate to device refresh rate
                    int deviceRefreshRate;
                    try
                    {
                        deviceRefreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
                    }
                    catch
                    {
                        // Fallback for older Unity versions
                        deviceRefreshRate = Screen.currentResolution.refreshRate;
                    }
                    
                    Application.targetFrameRate = deviceRefreshRate;
                    
                    Debug.Log($"Mobile V-Sync enabled: Setting target frame rate to device refresh rate ({deviceRefreshRate} Hz)");
                }
                else
                {
                    // When V-Sync is disabled, allow unlimited frame rate
                    Application.targetFrameRate = -1;
                    
                    Debug.Log("Mobile V-Sync disabled: Target frame rate set to unlimited");
                }
            }
            else
            {
                // On non-mobile platforms, let the quality settings handle frame rate control
                Debug.Log("Desktop platform detected: Frame rate control handled by quality settings");
                
                // Log current resolution for debugging
                try
                {
                    int refreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
                    Debug.Log($"Bootstrap: Current resolution: {Screen.currentResolution.width}x{Screen.currentResolution.height} @ {refreshRate}Hz");
                }
                catch
                {
                    int refreshRate = Screen.currentResolution.refreshRate;
                    Debug.Log($"Bootstrap: Current resolution: {Screen.currentResolution.width}x{Screen.currentResolution.height} @ {refreshRate}Hz");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to configure mobile refresh rate and resolution: {ex.Message}");
        }
    }

    private async Task LoadResourcesAndScene()
    {
        if (resourcesKeyName == null || resourcesKeyName.Length == 0)
        {
            Debug.LogWarning("No resource keys specified. Skipping asset loading.");
            await LoadNextScene();
            return;
        }

        var assetsLoaded = await _localSceneLoader.LoadAssetsAsync(resourcesKeyName);

        if (assetsLoaded)
        {
            await LoadNextScene();
        }
        else
        {
            Debug.LogError("Failed to load required assets. Scene transition aborted.");
        }
    }

    private async Task LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextScene))
        {
            Debug.LogWarning("Next scene name is not specified.");
            return;
        }

        // Ensure proper game state before scene transition
        EnsureProperGameState();
        
        _localSceneLoader.LoadSceneAsync(nextScene);
        await Task.Yield(); // Gibt Kontrolle zur�ck an Unity
    }

    private void OnDestroy()
    {
        // Cleanup falls notwendig
    }
}