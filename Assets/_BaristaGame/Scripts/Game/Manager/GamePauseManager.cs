using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GamePauseManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private AudioMixer audioMixer;

    public GameObject Pausepanel;
    public GameObject SettingsButton;

    public Slider SliderSoundFx;
    public Slider SliderMusic;
    public Slider SliderTalk;

    public Toggle ToggleCameraMove;
    public Toggle ToggleFullscreen;


    public Slider SliderTextureQuality;
    public Toggle ToggleAntiAlaising;
    public Toggle ToggleVsync;
    public Toggle ToggleAutoFixApron;


    public Toggle ToggleShowBestTimes;
    public GameObject TextTime;

    [Header("Settings")]

    public bool GamePaused = false;
    public string DiscordURL = "https://discord.gg/VCm2WYhG";
    [Range(0,1)]
    public float VolumeSoundFx = 1;
    [Range(0, 1)]
    public float VolumeMusic = 0.7f;
    [Range(0, 1)]
    public float VolumeTalk = 0.7f;
    [Range(0, 1)]
    public int TextureQuality = 0;

    private BaristaController barista;
    
    // Thread safety and race condition prevention
    private readonly object settingsLock = new object();
    private bool isInitialized = false;
    private bool isSavingValues = false;
    private float pendingMusicVolume = -1f;
    private float pendingEffectsVolume = -1f;
    private float pendingTalkVolume = -1f;
    
    // Flag to track if slider events are properly bound
    private bool sliderEventsInitialized = false;

    //// Start is called before the first frame update
    void Start()
    {
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        lock (settingsLock)
        {
            if (isInitialized)
                return;

            // Validate audio mixer reference
            if (audioMixer == null)
            {
                Debug.LogError("GamePauseManager: AudioMixer reference is null! Please assign the AudioMixer in the inspector.");
            }

            // Safe singleton access with retry mechanism
            InitializeBaristaReference();

            // Initialize slider events first to ensure they work properly
            InitializeSliderEvents();

            // Load all settings with error handling
            LoadVolumeSettings();
            LoadGraphicsSettings();
            LoadCameraSettings();
            LoadMiscSettings();

            // IMPORTANT: Notify MusicController that volume settings are ready
            // This ensures music volume is applied even if MusicController was initialized first
            NotifyMusicControllerVolumeReady();

            isInitialized = true;
        }
    }
    
    /// <summary>
    /// Notifies the MusicController (if it exists) that volume settings are ready to be applied.
    /// This is crucial when MusicController initializes before GamePauseManager (which happens in builds).
    /// </summary>
    private void NotifyMusicControllerVolumeReady()
    {
        try
        {
            if (MusicController.instance != null)
            {
                // Tell the MusicController to request current volume settings
                StartCoroutine(ApplyVolumeToMusicControllerDelayed());
                Debug.Log("GamePauseManager: Notified MusicController that volume settings are ready");
            }
            else
            {
                Debug.Log("GamePauseManager: MusicController instance not found - volume will be applied when it initializes");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error notifying MusicController: {e.Message}");
        }
    }
    
    /// <summary>
    /// Applies volume settings to MusicController with a small delay to ensure proper initialization
    /// </summary>
    private System.Collections.IEnumerator ApplyVolumeToMusicControllerDelayed()
    {
        // Small delay to ensure all components are properly initialized
        yield return new WaitForSeconds(0.1f);
        
        try
        {
            // Force apply current volume settings to ensure they're active
            ApplyVolumeSettingsToMixer();
            
            // Also force the MusicController to synchronize with us
            if (MusicController.instance != null)
            {
                MusicController.instance.ForceSyncWithGamePauseManager();
            }
            
            Debug.Log("GamePauseManager: Volume settings applied to MusicController after delay");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error applying delayed volume settings: {e.Message}");
        }
    }

    /// <summary>
    /// Initialize slider events to ensure they work properly after builds
    /// </summary>
    private void InitializeSliderEvents()
    {
        try
        {
            if (sliderEventsInitialized)
                return;

            // Clear any existing listeners to prevent duplicates
            if (SliderSoundFx != null)
            {
                SliderSoundFx.onValueChanged.RemoveAllListeners();
                SliderSoundFx.onValueChanged.AddListener(SetEffectsVolume);
            }

            if (SliderMusic != null)
            {
                SliderMusic.onValueChanged.RemoveAllListeners();
                SliderMusic.onValueChanged.AddListener(SetMusicVolume);
            }

            if (SliderTalk != null)
            {
                SliderTalk.onValueChanged.RemoveAllListeners();
                SliderTalk.onValueChanged.AddListener(SetTalkVolume);
            }

            if (SliderTextureQuality != null)
            {
                SliderTextureQuality.onValueChanged.RemoveAllListeners();
                SliderTextureQuality.onValueChanged.AddListener(SetTextureQuality);
            }

            sliderEventsInitialized = true;
            Debug.Log("GamePauseManager: Slider events initialized successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error initializing slider events: {e.Message}");
        }
    }

    private void InitializeBaristaReference()
    {
        // Try to get barista instance safely
        if (BaristaController.instance != null)
        {
            barista = BaristaController.instance;
        }
        else
        {
            // If not available immediately, try to find it in scene
            barista = FindObjectOfType<BaristaController>();
            
            if (barista == null)
            {
                Debug.LogWarning("GamePauseManager: BaristaController not found during initialization. Some features may not work.");
            }
        }
    }

    private void LoadVolumeSettings()
    {
        try
        {
            // Load values from PlayerPrefs
            VolumeSoundFx = PlayerPrefs.GetFloat(Consts.PlayerPrefSoundFx, VolumeSoundFx);
            VolumeMusic = PlayerPrefs.GetFloat(Consts.PlayerPrefMusic, VolumeMusic);
            VolumeTalk = PlayerPrefs.GetFloat(Consts.PlayerPrefTalk, VolumeTalk);

            // Set slider values WITHOUT triggering events (to prevent feedback loops)
            if (SliderSoundFx != null)
            {
                SliderSoundFx.SetValueWithoutNotify(VolumeSoundFx);
            }

            if (SliderMusic != null)
            {
                SliderMusic.SetValueWithoutNotify(VolumeMusic);
            }

            if (SliderTalk != null)
            {
                SliderTalk.SetValueWithoutNotify(VolumeTalk);
            }
                
            // Apply loaded volume settings to audio mixer immediately
            ApplyVolumeSettingsToMixer();
            
            Debug.Log($"GamePauseManager: Volume settings loaded - SFX: {VolumeSoundFx}, Music: {VolumeMusic}, Talk: {VolumeTalk}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error loading volume settings: {e.Message}");
        }
    }
    
    public void ApplyVolumeSettingsToMixer()
    {
        // Use the overloaded method that accepts an AudioMixer parameter
        ApplyVolumeSettingsToMixer(audioMixer);
    }

    /// <summary>
    /// Applies volume settings to a specific AudioMixer instance.
    /// This allows external systems to apply the same volume settings to their own mixers.
    /// </summary>
    /// <param name="targetAudioMixer">The AudioMixer to apply settings to</param>
    public void ApplyVolumeSettingsToMixer(AudioMixer targetAudioMixer)
    {
        if (targetAudioMixer == null)
        {
            Debug.LogWarning("GamePauseManager: Target AudioMixer is null, cannot apply volume settings");
            return;
        }

        try
        {
            // Apply effects volume
            if (VolumeSoundFx > 0.001f)
            {
                float dbValue = Mathf.Log10(VolumeSoundFx) * 20;
                dbValue = Mathf.Clamp(dbValue, -80f, 20f);
                targetAudioMixer.SetFloat(Consts.AudioVolumeEffects, dbValue);
                Debug.Log($"Applied Effects Volume to external mixer: {VolumeSoundFx} -> {dbValue}dB");
            }
            else
            {
                targetAudioMixer.SetFloat(Consts.AudioVolumeEffects, -80f);
            }
            
            // Apply music volume
            if (VolumeMusic > 0.001f)
            {
                float dbValue = Mathf.Log10(VolumeMusic) * 20;
                dbValue = Mathf.Clamp(dbValue, -80f, 20f);
                targetAudioMixer.SetFloat(Consts.AudioVolumeMusic, dbValue);
                Debug.Log($"Applied Music Volume to external mixer: {VolumeMusic} -> {dbValue}dB");
            }
            else
            {
                targetAudioMixer.SetFloat(Consts.AudioVolumeMusic, -80f);
            }
            
            // Apply talk volume
            if (VolumeTalk > 0.001f)
            {
                float dbValue = Mathf.Log10(VolumeTalk) * 20;
                dbValue = Mathf.Clamp(dbValue, -80f, 20f);
                targetAudioMixer.SetFloat(Consts.AudioVolumeTalk, dbValue);
                Debug.Log($"Applied Talk Volume to external mixer: {VolumeTalk} -> {dbValue}dB");
            }
            else
            {
                targetAudioMixer.SetFloat(Consts.AudioVolumeTalk, -80f);
            }
            
            // Force the audio mixer to update immediately
            targetAudioMixer.updateMode = AudioMixerUpdateMode.Normal;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error applying volume settings to external mixer: {e.Message}");
        }
    }

    private void LoadGraphicsSettings()
    {
        try
        {
            TextureQuality = PlayerPrefs.GetInt(Consts.PlayerPrefTextureQuality, TextureQuality);
            if (SliderTextureQuality != null)
                SliderTextureQuality.SetValueWithoutNotify(TextureQuality);

            // Set V-Sync to enabled by default on first launch (when no preference is saved)
            bool vsync = PlayerPrefs.GetInt(Consts.PlayerPrefVsync, 1) == 1; // Default to 1 (enabled)
            if (ToggleVsync != null)
                ToggleVsync.isOn = vsync;
            QualitySettings.vSyncCount = vsync ? 1 : 0;
            
            // Apply mobile refresh rate optimization on startup
            ApplyMobileRefreshRateOptimization(vsync);

            bool antiAliasing = PlayerPrefs.GetInt(Consts.PlayerPrefAntiAlaising, 0) == 1;
            if (ToggleAntiAlaising != null)
                ToggleAntiAlaising.isOn = antiAliasing;
            // Use 4x MSAA when enabled for better visual quality
            QualitySettings.antiAliasing = antiAliasing ? 4 : 0;

            // Only apply saved fullscreen preference if there's actually a saved preference
            if (PlayerPrefs.HasKey(Consts.PlayerPrefFullscreen))
            {
                bool savedFullscreen = PlayerPrefs.GetInt(Consts.PlayerPrefFullscreen, Screen.fullScreen ? 1 : 0) == 1;
                Screen.fullScreen = savedFullscreen;
                if (ToggleFullscreen != null)
                    ToggleFullscreen.isOn = savedFullscreen;
                Debug.Log($"GamePauseManager: Applied saved fullscreen setting: {savedFullscreen}");
            }
            else
            {
                // No saved preference - save current state as preference to establish it
                PlayerPrefs.SetInt(Consts.PlayerPrefFullscreen, Screen.fullScreen ? 1 : 0);
                PlayerPrefs.Save();
                
                if (ToggleFullscreen != null)
                    ToggleFullscreen.isOn = Screen.fullScreen;
                Debug.Log($"GamePauseManager: No saved fullscreen preference found, saved current state: {Screen.fullScreen}");
            }
            
            Debug.Log($"GamePauseManager: Graphics settings loaded - VSync: {vsync}, AntiAliasing: {antiAliasing}, TextureQuality: {TextureQuality}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error loading graphics settings: {e.Message}");
        }
    }

    private void LoadCameraSettings()
    {
        try
        {
#if UNITY_ANDROID
            bool defaultCameraMove = false;
#else
            bool defaultCameraMove = true;
#endif
            bool cameraMove = PlayerPrefs.GetInt(Consts.PlayerPrefCanMoveCamera, defaultCameraMove ? 1 : 0) == 1;
            if (ToggleCameraMove != null)
                ToggleCameraMove.isOn = cameraMove;
            
            // Safe camera pan access
            SetCameraPanEnabled(cameraMove);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error loading camera settings: {e.Message}");
        }
    }

    private void LoadMiscSettings()
    {
        try
        {
            bool autoFixApron = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefAutoFixClothes, false.ToString()));
            if (ToggleAutoFixApron != null)
                ToggleAutoFixApron.isOn = autoFixApron;
            
            // Safe barista auto fix setting
            SetBarsistaAutoFixOutfit(autoFixApron);

            bool showBestTimes = PlayerPrefs.GetInt(Consts.PlayerPrefShowBestTimes, 0) == 1;
            if (ToggleShowBestTimes != null)
                ToggleShowBestTimes.isOn = showBestTimes;
            
            if (TextTime != null)
                TextTime.SetActive(showBestTimes);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error loading misc settings: {e.Message}");
        }
    }

    private void SetCameraPanEnabled(bool enabled)
    {
        try
        {
            if (CameraPan.Instance != null)
            {
                CameraPan.Instance.enabled = enabled;
            }
            else
            {
                // Fallback: try to find camera pan component
                var cameraPan = FindObjectOfType<CameraPan>();
                if (cameraPan != null)
                {
                    cameraPan.enabled = enabled;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting camera pan: {e.Message}");
        }
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SetGamePause(bool Pause)
    {
        lock (settingsLock)
        {
            if (GamePaused == Pause)
                return; // Prevent redundant state changes

            GamePaused = Pause;

            try
            {
                if (GamePaused == true)
                {
                    Time.timeScale = 0;
                    if (Pausepanel != null)
                        Pausepanel.SetActive(true);
                    if (SettingsButton != null)
                        SettingsButton.SetActive(false);
                    KeyBindingManager.instance.Paused();
                }
                else
                {
                    Time.timeScale = 1;
                    if (Pausepanel != null)
                        Pausepanel.SetActive(false);
                    if (SettingsButton != null)
                        SettingsButton.SetActive(true);
                    
                    // Always save when unpausing to ensure settings are persisted
                    SaveValues();
                    KeyBindingManager.instance.UnPaused();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"GamePauseManager: Error setting game pause state: {e.Message}");
                // Restore time scale on error
                Time.timeScale = GamePaused ? 0 : 1;
            }
        }
    }

    public void SetGameFullscreen(bool b)
    {
        try
        {
            Screen.fullScreen = b;
            // Save fullscreen preference
            PlayerPrefs.SetInt(Consts.PlayerPrefFullscreen, b ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log($"GamePauseManager: Fullscreen set to {b}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting fullscreen: {e.Message}");
        }
    }

    public void SaveValues()
    {
        lock (settingsLock)
        {
            if (isSavingValues)
                return; // Prevent concurrent saves

            isSavingValues = true;
        }

        try
        {
            Debug.Log("SaveValues: " + (SliderSoundFx?.value ?? 0) + " " + (SliderMusic?.value ?? 0) + " " + (SliderTalk?.value ?? 0));
            
            // Save volume settings - ensure we save the actual current values
            if (SliderSoundFx != null)
            {
                VolumeSoundFx = SliderSoundFx.value;
                PlayerPrefs.SetFloat(Consts.PlayerPrefSoundFx, VolumeSoundFx);
            }
            if (SliderMusic != null)
            {
                VolumeMusic = SliderMusic.value;
                PlayerPrefs.SetFloat(Consts.PlayerPrefMusic, VolumeMusic);
            }
            if (SliderTalk != null)
            {
                VolumeTalk = SliderTalk.value;
                PlayerPrefs.SetFloat(Consts.PlayerPrefTalk, VolumeTalk);
            }
            
            // Save other settings
            if (ToggleAutoFixApron != null)
                PlayerPrefs.SetString(Consts.PlayerPrefAutoFixClothes, ToggleAutoFixApron.isOn.ToString());
            if (SliderTextureQuality != null)
                PlayerPrefs.SetInt(Consts.PlayerPrefTextureQuality, Mathf.RoundToInt(SliderTextureQuality.value));

            PlayerPrefs.Save();
            Debug.Log("Settings saved successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error saving values: {e.Message}");
        }
        finally
        {
            lock (settingsLock)
            {
                isSavingValues = false;
            }
        }
    }

    public void SetBarsistaAutoFixOutfit(bool value)
    {
        try
        {
            // Safe barista access
            if (barista == null)
                InitializeBaristaReference();

            if (barista != null)
            {
                barista.AutoFixOutfit = value;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting barista auto fix outfit: {e.Message}");
        }
    }

    public void ChangePauseState()
    {
        SetGamePause(!GamePaused);
    }

    public void OpenDiscordURL()
    {
        try
        {
            Application.OpenURL(DiscordURL);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error opening Discord URL: {e.Message}");
        }
    }

    public void SetMusicVolume(float slidervalue)
    {
        try
        {
            VolumeMusic = slidervalue;
            
            // Apply to GamePauseManager's own AudioMixer for any UI sounds or other audio
            if (audioMixer != null)
            {
                if (slidervalue > 0.001f) // Use small epsilon instead of direct comparison
                {
                    float dbValue = Mathf.Log10(slidervalue) * 20;
                    dbValue = Mathf.Clamp(dbValue, -80f, 20f); // Clamp to reasonable range
                    audioMixer.SetFloat(Consts.AudioVolumeMusic, dbValue);
                }
                else
                {
                    audioMixer.SetFloat(Consts.AudioVolumeMusic, -80f); // Mute when volume is 0
                }
                
                // Force the audio mixer to update immediately
                audioMixer.updateMode = AudioMixerUpdateMode.Normal;
            }
            else
            {
                Debug.LogWarning("GamePauseManager: AudioMixer is null in SetMusicVolume");
            }
            
            // IMPORTANT: Also update MusicController's AudioSources directly
            if (MusicController.instance != null)
            {
                MusicController.instance.SetMusicVolumeLevel(slidervalue);
            }
            
            // Save immediately to ensure persistence
            PlayerPrefs.SetFloat(Consts.PlayerPrefMusic, VolumeMusic);
            
            Debug.Log($"SetMusicVolume: {slidervalue} -> AudioMixer: {(slidervalue > 0.001f ? Mathf.Log10(slidervalue) * 20 : -80f)}dB, MusicController: {slidervalue}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting music volume: {e.Message}");
        }
    }

    public void SetEffectsVolume(float slidervalue)
    {
        try
        {
            VolumeSoundFx = slidervalue;
            
            if (audioMixer != null)
            {
                if (slidervalue > 0.001f) // Use small epsilon instead of direct comparison
                {
                    float dbValue = Mathf.Log10(slidervalue) * 20;
                    dbValue = Mathf.Clamp(dbValue, -80f, 20f); // Clamp to reasonable range
                    audioMixer.SetFloat(Consts.AudioVolumeEffects, dbValue);
                }
                else
                {
                    audioMixer.SetFloat(Consts.AudioVolumeEffects, -80f); // Mute when volume is 0
                }
                
                // Force the audio mixer to update immediately
                audioMixer.updateMode = AudioMixerUpdateMode.Normal;
            }
            else
            {
                Debug.LogWarning("GamePauseManager: AudioMixer is null in SetEffectsVolume");
            }
            
            // Save immediately to ensure persistence
            PlayerPrefs.SetFloat(Consts.PlayerPrefSoundFx, VolumeSoundFx);
            
            Debug.Log($"SetEffectsVolume: {slidervalue} -> {(slidervalue > 0.001f ? Mathf.Log10(slidervalue) * 20 : -80f)}dB");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting effects volume: {e.Message}");
        }
    }

    public void SetTalkVolume(float slidervalue)
    {
        try
        {
            VolumeTalk = slidervalue;
            
            if (audioMixer != null)
            {
                if (slidervalue > 0.001f) // Use small epsilon instead of direct comparison
                {
                    float dbValue = Mathf.Log10(slidervalue) * 20;
                    dbValue = Mathf.Clamp(dbValue, -80f, 20f); // Clamp to reasonable range
                    audioMixer.SetFloat(Consts.AudioVolumeTalk, dbValue);
                }
                else
                {
                    audioMixer.SetFloat(Consts.AudioVolumeTalk, -80f); // Mute when volume is 0
                }
                
                // Force the audio mixer to update immediately
                audioMixer.updateMode = AudioMixerUpdateMode.Normal;
            }
            else
            {
                Debug.LogWarning("GamePauseManager: AudioMixer is null in SetTalkVolume");
            }
            
            // Save immediately to ensure persistence
            PlayerPrefs.SetFloat(Consts.PlayerPrefTalk, VolumeTalk);
            
            Debug.Log($"SetTalkVolume: {slidervalue} -> {(slidervalue > 0.001f ? Mathf.Log10(slidervalue) * 20 : -80f)}dB");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting talk volume: {e.Message}");
        }
    }

    public void SetCameraPossibileMove(bool on)
    {
        try
        {
            PlayerPrefs.SetInt(Consts.PlayerPrefCanMoveCamera, on ? 1 : 0);
            PlayerPrefs.Save(); // Immediate save for consistency
            SetCameraPanEnabled(on);
            Debug.Log("Camera:" + on);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting camera movement: {e.Message}");
        }
    }

    public void SetShowPlayerBestTime(bool on)
    {
        try
        {
            PlayerPrefs.SetInt(Consts.PlayerPrefShowBestTimes, on ? 1 : 0);
            PlayerPrefs.Save(); // Immediate save for consistency
            if (TextTime != null)
            {
                TextTime.SetActive(on);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting show best time: {e.Message}");
        }
    }

    public void SetVsync(bool state)
    {
        try
        {
            PlayerPrefs.SetInt(Consts.PlayerPrefVsync, state ? 1 : 0);
            PlayerPrefs.Save(); // Immediate save for consistency
            QualitySettings.vSyncCount = state ? 1 : 0;
            
            // Apply mobile refresh rate optimization when V-Sync setting changes
            ApplyMobileRefreshRateOptimization(state);
            
            Debug.Log("Vsync: " + QualitySettings.vSyncCount);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting vsync: {e.Message}");
        }
    }

    /// <summary>
    /// Applies mobile-specific refresh rate optimization based on V-Sync setting.
    /// When V-Sync is enabled on mobile, sets target frame rate to device refresh rate.
    /// When disabled, allows unlimited frame rate.
    /// </summary>
    /// <param name="vsyncEnabled">Whether V-Sync is enabled</param>
    private void ApplyMobileRefreshRateOptimization(bool vsyncEnabled)
    {
        try
        {
            // Only apply optimization on mobile platforms
            if (Application.isMobilePlatform)
            {
                if (vsyncEnabled)
                {
                    // When V-Sync is enabled, set target frame rate to device refresh rate
                    // Use refreshRateRatio.value for newer Unity versions, fallback to refreshRate for compatibility
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
                // On desktop platforms, log current resolution info for debugging
                try
                {
                    int refreshRate = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
                    Debug.Log($"Desktop platform - Current resolution: {Screen.currentResolution.width}x{Screen.currentResolution.height} @ {refreshRate}Hz, VSync: {(vsyncEnabled ? "On" : "Off")}");
                }
                catch
                {
                    int refreshRate = Screen.currentResolution.refreshRate;
                    Debug.Log($"Desktop platform - Current resolution: {Screen.currentResolution.width}x{Screen.currentResolution.height} @ {refreshRate}Hz, VSync: {(vsyncEnabled ? "On" : "Off")}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error applying mobile refresh rate optimization: {e.Message}");
        }
    }

    public void SetAntialaising(bool state)
    {
        try
        {
            PlayerPrefs.SetInt(Consts.PlayerPrefAntiAlaising, state ? 1 : 0);
            PlayerPrefs.Save(); // Immediate save for consistency
            // Use 4x MSAA when enabled for better visual quality
            QualitySettings.antiAliasing = state ? 4 : 0;
            Debug.Log("AntiAliasing: " + QualitySettings.antiAliasing);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting anti-aliasing: {e.Message}");
        }
    }

    public void SetTextureQuality(float state)
    {
        try
        {
            int qualityLevel = Mathf.RoundToInt(state);
            TextureQuality = qualityLevel;
            QualitySettings.globalTextureMipmapLimit = qualityLevel;
            
            // Save immediately for consistency
            PlayerPrefs.SetInt(Consts.PlayerPrefTextureQuality, qualityLevel);
            
            Debug.Log("TextureQuality: " + QualitySettings.globalTextureMipmapLimit);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error setting texture quality: {e.Message}");
        }
    }

    public void ResetRecordTimes()
    {
        try
        {
            // Batch delete operations to reduce race condition window
            string[] keysToDelete = {
                Consts.PlayerPrefBestTimeCasual,
                Consts.PlayerPrefBestTimeNormal,
                Consts.PlayerPrefBestTimeHard,
                Consts.PlayerPrefBestTimeChaos,
                Consts.PlayerPrefBestTimeUltraChaos,
                Consts.PlayerPrefBestTimeNoasMod,
                Consts.PlayerPrefBestTimeCasual + Consts.PlayerPrefBestTimeMilkymodeSuffix,
                Consts.PlayerPrefBestTimeNormal + Consts.PlayerPrefBestTimeMilkymodeSuffix,
                Consts.PlayerPrefBestTimeHard + Consts.PlayerPrefBestTimeMilkymodeSuffix,
                Consts.PlayerPrefBestTimeChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix,
                Consts.PlayerPrefBestTimeUltraChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix,
                Consts.PlayerPrefMostEarned,
                Consts.PlayerPrefMostServed,
                Consts.PlayerPrefMostMilk
            };

            foreach (string key in keysToDelete)
            {
                PlayerPrefs.DeleteKey(key);
            }
            
            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error resetting record times: {e.Message}");
        }
    }

    // Called when component is disabled to ensure clean state
    private void OnDisable()
    {
        lock (settingsLock)
        {
            if (isInitialized && !isSavingValues)
            {
                SaveValues();
            }
        }
    }

    // Called when application is paused/resumed (mobile)
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && isInitialized && !isSavingValues)
        {
            SaveValues();
            // Reset milky mode when application is paused/minimized
            ResetMilkyModePreference();
        }
    }

    // Called when application focus is lost/gained
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && isInitialized && !isSavingValues)
        {
            SaveValues();
            // Reset milky mode when application loses focus
            ResetMilkyModePreference();
        }
    }

    /// <summary>
    /// Resets the milky mode preference to ensure it doesn't persist between sessions
    /// </summary>
    private void ResetMilkyModePreference()
    {
        try
        {
            PlayerPrefs.DeleteKey(Consts.PlayerPrefNextIsMilkyMode);
            PlayerPrefs.Save();
            Debug.Log("GamePauseManager: Milky mode preference reset");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error resetting milky mode preference: {e.Message}");
        }
    }
    
    /// <summary>
    /// Public method to force apply current settings - useful for when returning to main menu
    /// This method is modified to only apply audio settings to preserve user's graphics choices
    /// </summary>
    public static void ApplySettingsFromPlayerPrefs()
    {
        try
        {
            // When returning to main menu, only apply audio settings
            // Don't reinitialize graphics settings to preserve user's immediate choices (like fullscreen changes)
            ApplyVolumeSettingsToMusicController();
            
            Debug.Log("GamePauseManager: Applied audio settings from PlayerPrefs (preserved graphics settings)");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error applying settings from PlayerPrefs: {e.Message}");
        }
    }
    
    /// <summary>
    /// Static method to apply volume settings directly to MusicController when GamePauseManager is not available
    /// </summary>
    public static void ApplyVolumeSettingsToMusicController()
    {
        try
        {
            if (MusicController.instance != null)
            {
                // Get the saved music volume and apply it directly to MusicController
                float musicVolume = PlayerPrefs.GetFloat(Consts.PlayerPrefMusic, 0.7f);
                MusicController.instance.SetMusicVolumeLevel(musicVolume);
                
                // Also call the fallback method for other audio settings
                MusicController.instance.ApplyFallbackVolumeSettings();
                
                Debug.Log($"GamePauseManager: Applied volume settings directly to MusicController - Music: {musicVolume}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"GamePauseManager: Error applying volume settings to MusicController: {e.Message}");
        }
    }

    /// <summary>
    /// Gets the AudioMixer used by this GamePauseManager for external synchronization
    /// </summary>
    /// <returns>The AudioMixer instance used by this GamePauseManager</returns>
    public AudioMixer GetAudioMixer()
    {
        return audioMixer;
    }
}
