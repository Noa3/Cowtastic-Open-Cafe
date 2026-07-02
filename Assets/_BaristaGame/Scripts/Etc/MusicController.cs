using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Threading.Tasks;
using _BaristaGame.Scripts.AddressablesScripts;
using System.Collections.Generic;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    [Header("References")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup audioMixerGroup;

    [Header("Playlist Settings")]
    public bool ShufflePlaylist = true;
    public MusicHolder[] MusicList;

    [Header("Addressables Settings")]
    [SerializeField] private bool useAddressables = false;
    [SerializeField] private string[] addressableAudioKeys; // Audio asset keys for Addressables

    [Header("Crossfade Settings")]
    public float crossFadeTime = 4f;

    public AudioSource audioSourceA, audioSourceB;
    private float audioSourceAVolumeVelocity, audioSourceBVolumeVelocity;
    private AudioClip currentAudioClip;
    private bool isInitialized = false;

    [Header("Debug/Info")]
    [ReadOnly] public float Songtime = 0;
    [ReadOnly] public int currentSongNumber = 0;
    [ReadOnly] public bool IsCrossfading => Mathf.Abs(audioSourceA.volume - 1f) > 0.02f || audioSourceB.volume > 0.02f;
    [ReadOnly] public bool IsLoadingAddressableAudio = false;

    // Addressables loading
    private Dictionary<string, LocalAudioLoader> audioLoaders = new Dictionary<string, LocalAudioLoader>();
    private List<AudioClip> loadedAddressableAudioClips = new List<AudioClip>();
    
    // Volume control
    private float baseAudioVolume = 1f; // The base volume multiplier for music

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
            InitializeMusicController();
        }
        else
        {
            // If another instance exists, crossfade to current audio if not EditorOnly
            if (!gameObject.CompareTag("EditorOnly") && currentAudioClip != null)
            {
                instance.CrossFade(currentAudioClip);
            }
            Destroy(gameObject);
        }
    }

    private async void InitializeMusicController()
    {
        // Validate essential components
        if (audioMixer == null)
        {
            Debug.LogError("MusicController: AudioMixer not assigned!");
            return;
        }

        // Load audio assets based on mode
        if (useAddressables)
        {
            await LoadAddressableAudio();
        }
        else
        {
            if (MusicList == null || MusicList.Length == 0)
            {
                Debug.LogError("MusicController: No music tracks assigned!");
                return;
            }
        }

        // Initialize playlist
        DoShufflePlaylist();

        if (GetCurrentAudioClip() != null)
        {
            currentAudioClip = GetCurrentAudioClip();
            Songtime = currentAudioClip.length;
        }

        // REMOVED: Don't set volume here anymore - let GamePauseManager handle it
        // The GamePauseManager will apply the correct volume settings
        // SetMusicVolume(PlayerPrefs.GetFloat(Consts.PlayerPrefMusic, 0.7f));
        // SetEffectsVolume(PlayerPrefs.GetFloat(Consts.PlayerPrefSoundFx, 0.5f));
        // SetTalkVolume(PlayerPrefs.GetFloat(Consts.PlayerPrefTalk, 0.7f));

        isInitialized = true;
        
        // Request volume settings from GamePauseManager if it exists, or wait for it to be created
        StartCoroutine(RequestVolumeSettingsFromGamePauseManager());
        
        Debug.Log("MusicController: Initialized successfully. Volume will be controlled by GamePauseManager.");
    }
    
    /// <summary>
    /// Requests volume settings from GamePauseManager. 
    /// This handles cases where MusicController initializes before GamePauseManager exists.
    /// </summary>
    private System.Collections.IEnumerator RequestVolumeSettingsFromGamePauseManager()
    {
        const float maxWaitTime = 10f; // Maximum time to wait for GamePauseManager
        const float checkInterval = 0.5f; // How often to check for GamePauseManager
        float elapsedTime = 0f;
        
        while (elapsedTime < maxWaitTime)
        {
            var gamePauseManager = FindObjectOfType<GamePauseManager>();
            if (gamePauseManager != null)
            {
                // GamePauseManager found, request it to apply volume settings
                yield return new WaitForSeconds(0.2f); // Small delay to ensure it's fully initialized
                
                try
                {
                    // IMPORTANT: Sync AudioMixer references to ensure we're using the same mixer
                    SyncAudioMixerWithGamePauseManager(gamePauseManager);
                    
                    // Force the GamePauseManager to apply its volume settings
                    gamePauseManager.ApplyVolumeSettingsToMixer();
                    Debug.Log("MusicController: Successfully requested volume settings from GamePauseManager");
                    yield break;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"MusicController: Error requesting volume settings: {e.Message}");
                }
            }
            
            yield return new WaitForSeconds(checkInterval);
            elapsedTime += checkInterval;
        }
        
        // If we reach here, GamePauseManager wasn't found within the time limit
        // Apply fallback volume settings directly
        Debug.LogWarning("MusicController: GamePauseManager not found within time limit, applying fallback volume settings");
        ApplyFallbackVolumeSettings();
    }
    
    /// <summary>
    /// Syncs the AudioMixer reference with GamePauseManager to ensure both use the same mixer instance
    /// </summary>
    private void SyncAudioMixerWithGamePauseManager(GamePauseManager gamePauseManager)
    {
        try
        {
            // Instead of syncing AudioMixers, apply volume settings directly to AudioSources
            ApplyVolumeSettingsToAudioSources(gamePauseManager);
            
            Debug.Log("MusicController: Successfully applied volume settings to AudioSources from GamePauseManager");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MusicController: Error syncing volume with GamePauseManager: {e.Message}");
        }
    }
    
    /// <summary>
    /// Applies volume settings from GamePauseManager directly to our AudioSources
    /// </summary>
    private void ApplyVolumeSettingsToAudioSources(GamePauseManager gamePauseManager)
    {
        try
        {
            // Get the current music volume from GamePauseManager's VolumeMusic property
            // Use reflection to access the private field, or use PlayerPrefs as fallback
            float musicVolume;
            try
            {
                var volumeMusicField = typeof(GamePauseManager).GetField("VolumeMusic", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (volumeMusicField != null)
                {
                    musicVolume = (float)volumeMusicField.GetValue(gamePauseManager);
                }
                else
                {
                    // Fallback to PlayerPrefs
                    musicVolume = PlayerPrefs.GetFloat(Consts.PlayerPrefMusic, 0.7f);
                }
            }
            catch
            {
                // Fallback to PlayerPrefs
                musicVolume = PlayerPrefs.GetFloat(Consts.PlayerPrefMusic, 0.7f);
            }
            
            // Apply the volume directly to our AudioSources
            if (audioSourceA != null)
            {
                audioSourceA.volume = musicVolume;
                Debug.Log($"MusicController: Set audioSourceA volume to {musicVolume}");
            }
            
            if (audioSourceB != null)
            {
                audioSourceB.volume = 0f; // audioSourceB should start at 0 for crossfading
                Debug.Log($"MusicController: Set audioSourceB volume to 0 (for crossfading)");
            }
            
            // Store the base volume for crossfading calculations
            baseAudioVolume = musicVolume;
            
            Debug.Log($"MusicController: Applied music volume {musicVolume} to AudioSources");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MusicController: Error applying volume settings to AudioSources: {e.Message}");
        }
    }
    
    /// <summary>
    /// Applies fallback volume settings when GamePauseManager is not available.
    /// Can be called externally to force volume application.
    /// </summary>
    public void ApplyFallbackVolumeSettings()
    {
        try
        {
            // Load saved volume settings directly
            float musicVolume = PlayerPrefs.GetFloat(Consts.PlayerPrefMusic, 0.7f);
            
            // Apply music volume directly to AudioSources instead of AudioMixer
            if (audioSourceA != null)
            {
                audioSourceA.volume = musicVolume;
            }
            
            if (audioSourceB != null)
            {
                audioSourceB.volume = 0f; // audioSourceB should start at 0 for crossfading
            }
            
            // Store the base volume
            baseAudioVolume = musicVolume;
            
            // Still apply other volume settings to AudioMixer if available (for effects and talk)
            if (audioMixer != null)
            {
                float effectsVolume = PlayerPrefs.GetFloat(Consts.PlayerPrefSoundFx, 1f);
                float talkVolume = PlayerPrefs.GetFloat(Consts.PlayerPrefTalk, 0.7f);
                
                // Apply effects volume
                if (effectsVolume > 0.001f)
                {
                    float dbValue = Mathf.Log10(effectsVolume) * 20;
                    dbValue = Mathf.Clamp(dbValue, -80f, 20f);
                    audioMixer.SetFloat(Consts.AudioVolumeEffects, dbValue);
                }
                else
                {
                    audioMixer.SetFloat(Consts.AudioVolumeEffects, -80f);
                }
                
                // Apply talk volume
                if (talkVolume > 0.001f)
                {
                    float dbValue = Mathf.Log10(talkVolume) * 20;
                    dbValue = Mathf.Clamp(dbValue, -80f, 20f);
                    audioMixer.SetFloat(Consts.AudioVolumeTalk, dbValue);
                }
                else
                {
                    audioMixer.SetFloat(Consts.AudioVolumeTalk, -80f);
                }
                
                // Force audio mixer to update
                audioMixer.updateMode = AudioMixerUpdateMode.Normal;
            }
            
            Debug.Log($"MusicController: Applied fallback volume settings - Music: {musicVolume} (AudioSource), Effects & Talk: AudioMixer");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MusicController: Error applying fallback volume settings: {e.Message}");
        }
    }

    #region Addressables Support

    /// <summary>
    /// Loads audio clips from Addressables system
    /// </summary>
    private async Task LoadAddressableAudio()
    {
        if (addressableAudioKeys == null || addressableAudioKeys.Length == 0)
        {
            Debug.LogWarning("MusicController: No Addressable audio keys specified!");
            return;
        }

        IsLoadingAddressableAudio = true;
        loadedAddressableAudioClips.Clear();

        try
        {
            for (int i = 0; i < addressableAudioKeys.Length; i++)
            {
                string key = addressableAudioKeys[i];
                LocalAudioLoader loader = new LocalAudioLoader();

                AudioClip clip = await loader.LoadAudioAsync(key);
                if (clip != null)
                {
                    loadedAddressableAudioClips.Add(clip);
                    audioLoaders[key] = loader;
                    Debug.Log($"MusicController: Successfully loaded audio '{key}' from Addressables");
                }
                else
                {
                    Debug.LogWarning($"MusicController: Failed to load audio '{key}' from Addressables");
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"MusicController: Error loading Addressable audio: {ex.Message}");
        }
        finally
        {
            IsLoadingAddressableAudio = false;
        }

        if (loadedAddressableAudioClips.Count > 0)
        {
            Debug.Log($"MusicController: Successfully loaded {loadedAddressableAudioClips.Count} audio clips from Addressables");
        }
    }

    /// <summary>
    /// Gets the current audio clip based on the active mode (Addressables or MusicList)
    /// </summary>
    private AudioClip GetCurrentAudioClip()
    {
        if (useAddressables)
        {
            return loadedAddressableAudioClips.Count > currentSongNumber ?
                   loadedAddressableAudioClips[currentSongNumber] : null;
        }
        else
        {
            return MusicList?.Length > currentSongNumber ?
                   MusicList[currentSongNumber].clip : null;
        }
    }

    /// <summary>
    /// Gets the total number of available audio clips
    /// </summary>
    private int GetTotalAudioClips()
    {
        return useAddressables ? loadedAddressableAudioClips.Count :
               (MusicList?.Length ?? 0);
    }

    #endregion

    /// <summary>
    /// Shuffles the playlist if shuffle is enabled
    /// </summary>
    public void DoShufflePlaylist()
    {
        if (!ShufflePlaylist) return;

        if (useAddressables && loadedAddressableAudioClips.Count > 0)
        {
            loadedAddressableAudioClips.Shuffle();
        }
        else if (MusicList != null && MusicList.Length > 0)
        {
            MusicList.Shuffle();
        }
    }

    /// <summary>
    /// Searches the music list and plays the specified song by name
    /// </summary>
    /// <param name="audioClipName">Name of the audio clip to play</param>
    public void CrossFade(string audioClipName)
    {
        if (string.IsNullOrEmpty(audioClipName)) return;

        if (useAddressables)
        {
            // For Addressables, we use the clip directly since we can't search by name easily
            Debug.LogWarning("MusicController: CrossFade by name not fully supported with Addressables. Use CrossFade(AudioClip) instead.");
            return;
        }

        if (MusicList == null) return;

        foreach (MusicHolder item in MusicList)
        {
            if (item.name == audioClipName && item.clip != null)
            {
                CrossFade(item.clip);
                return;
            }
        }

        Debug.LogWarning($"MusicController: Song '{audioClipName}' not found in playlist!");
    }

    /// <summary>
    /// Plays the specified AudioClip with crossfade effect
    /// </summary>
    /// <param name="audioClip">The AudioClip to play</param>
    public void CrossFade(AudioClip audioClip)
    {
        if (audioClip == null || !isInitialized) return;

        // Only crossfade if a different clip should be played
        if (audioSourceA.clip != audioClip)
        {
            // Swap audio sources
            (audioSourceA, audioSourceB) = (audioSourceB, audioSourceA);
            audioSourceA.clip = audioClip;
        }

        audioSourceA.Play();
        currentAudioClip = audioClip;
    }

    private void Update()
    {
        if (!isInitialized || IsLoadingAddressableAudio) return;

        HandleCrossfade();
        CheckForNextSong();
    }

    /// <summary>
    /// Handles the crossfade volume interpolation between audio sources
    /// </summary>
    private void HandleCrossfade()
    {
        // Calculate target volumes based on base volume setting
        float targetVolumeA = baseAudioVolume;
        float targetVolumeB = 0f;
        
        // Fade in active audio source to the base volume level
        if (audioSourceA.volume < (targetVolumeA - 0.02f))
        {
            audioSourceA.volume = Mathf.SmoothDamp(audioSourceA.volume, targetVolumeA,
                ref audioSourceAVolumeVelocity, crossFadeTime);
        }
        else if (audioSourceA.volume != targetVolumeA)
        {
            audioSourceA.volume = targetVolumeA;
        }

        // Fade out inactive audio source
        if (audioSourceB.volume > 0.02f)
        {
            audioSourceB.volume = Mathf.SmoothDamp(audioSourceB.volume, targetVolumeB,
                ref audioSourceBVolumeVelocity, crossFadeTime);
        }
        else if (audioSourceB.volume != targetVolumeB)
        {
            audioSourceB.volume = targetVolumeB;
            audioSourceB.Stop(); // Stop to save resources
        }
    }

    /// <summary>
    /// Checks if it's time to transition to the next song
    /// </summary>
    private void CheckForNextSong()
    {
        if (audioSourceA.isPlaying && audioSourceA.time > (Songtime - crossFadeTime))
        {
            PlayNextSong();
        }
    }

    /// <summary>
    /// Advances to the next song in the playlist
    /// </summary>
    private void PlayNextSong()
    {
        currentSongNumber++;
        int totalClips = GetTotalAudioClips();

        if (currentSongNumber >= totalClips)
        {
            DoShufflePlaylist();
            currentSongNumber = 0;
        }

        AudioClip nextClip = GetCurrentAudioClip();
        if (nextClip != null)
        {
            CrossFade(nextClip);
            Songtime = nextClip.length;
        }
    }

    private void OnEnable()
    {
        InitializeAudioSources();
    }

    /// <summary>
    /// Initializes and configures the audio sources
    /// </summary>
    private void InitializeAudioSources()
    {
        // Initialize Audio Source A
        if (audioSourceA == null)
        {
            audioSourceA = gameObject.AddComponent<AudioSource>();
        }
        ConfigureAudioSource(audioSourceA);

        // Initialize Audio Source B
        if (audioSourceB == null)
        {
            audioSourceB = gameObject.AddComponent<AudioSource>();
        }
        ConfigureAudioSource(audioSourceB);

        // Set initial volumes based on saved preferences
        float initialMusicVolume = PlayerPrefs.GetFloat(Consts.PlayerPrefMusic, 0.7f);
        baseAudioVolume = initialMusicVolume;
        
        if (audioSourceA != null)
        {
            audioSourceA.volume = initialMusicVolume;
        }
        
        if (audioSourceB != null)
        {
            audioSourceB.volume = 0f; // Start at 0 for crossfading
        }

        // Start playing if we have audio and are initialized
        if (currentAudioClip != null && isInitialized)
        {
            audioSourceA.clip = currentAudioClip;
            audioSourceA.Play();
        }
        
        Debug.Log($"MusicController: AudioSources initialized with music volume: {initialMusicVolume}");
    }

    /// <summary>
    /// Configures an AudioSource with standard settings
    /// </summary>
    private void ConfigureAudioSource(AudioSource source)
    {
        if (source == null) return;

        source.spatialBlend = 0f; // 2D audio
        source.loop = false;
        source.outputAudioMixerGroup = audioMixerGroup;
        source.playOnAwake = false;
    }

    #region Volume Controls - DEPRECATED: Use GamePauseManager for volume control

    /// <summary>
    /// DEPRECATED: This method is kept for compatibility but should not be used.
    /// Use GamePauseManager.SetMusicVolume() instead for consistent volume control.
    /// </summary>
    [System.Obsolete("Use GamePauseManager.SetMusicVolume() instead")]
    public void SetMusicVolume(float sliderValue)
    {
        Debug.LogWarning("MusicController.SetMusicVolume() is deprecated. Use GamePauseManager.SetMusicVolume() instead.");
        
        if (audioMixer == null) return;

        if (sliderValue > 0.001f)
        {
            float dbValue = Mathf.Log10(sliderValue) * 20;
            dbValue = Mathf.Clamp(dbValue, -80f, 20f);
            audioMixer.SetFloat(Consts.AudioVolumeMusic, dbValue);
        }
        else
        {
            audioMixer.SetFloat(Consts.AudioVolumeMusic, -80f); // Mute
        }
    }

    /// <summary>
    /// DEPRECATED: This method is kept for compatibility but should not be used.
    /// Use GamePauseManager.SetEffectsVolume() instead for consistent volume control.
    /// </summary>
    [System.Obsolete("Use GamePauseManager.SetEffectsVolume() instead")]
    public void SetEffectsVolume(float sliderValue)
    {
        Debug.LogWarning("MusicController.SetEffectsVolume() is deprecated. Use GamePauseManager.SetEffectsVolume() instead.");
        
        if (audioMixer == null) return;

        if (sliderValue > 0.001f)
        {
            float dbValue = Mathf.Log10(sliderValue) * 20;
            dbValue = Mathf.Clamp(dbValue, -80f, 20f);
            audioMixer.SetFloat(Consts.AudioVolumeEffects, dbValue);
        }
        else
        {
            audioMixer.SetFloat(Consts.AudioVolumeEffects, -80f);
        }
    }

    /// <summary>
    /// DEPRECATED: This method is kept for compatibility but should not be used.
    /// Use GamePauseManager.SetTalkVolume() instead for consistent volume control.
    /// </summary>
    [System.Obsolete("Use GamePauseManager.SetTalkVolume() instead")]
    public void SetTalkVolume(float sliderValue)
    {
        Debug.LogWarning("MusicController.SetTalkVolume() is deprecated. Use GamePauseManager.SetTalkVolume() instead.");
        
        if (audioMixer == null) return;

        if (sliderValue > 0.001f)
        {
            float dbValue = Mathf.Log10(sliderValue) * 20;
            dbValue = Mathf.Clamp(dbValue, -80f, 20f);
            audioMixer.SetFloat(Consts.AudioVolumeTalk, dbValue);
        }
        else
        {
            audioMixer.SetFloat(Consts.AudioVolumeTalk, -80f);
        }
    }

    #endregion

    #region Public API

    /// <summary>
    /// Pauses music playback
    /// </summary>
    public void PauseMusic()
    {
        audioSourceA?.Pause();
        audioSourceB?.Pause();
    }

    /// <summary>
    /// Resumes music playback
    /// </summary>
    public void ResumeMusic()
    {
        audioSourceA?.UnPause();
        audioSourceB?.UnPause();
    }

    /// <summary>
    /// Stops all music playback
    /// </summary>
    public void StopMusic()
    {
        audioSourceA?.Stop();
        audioSourceB?.Stop();
    }

    /// <summary>
    /// Skips to the next song immediately
    /// </summary>
    public void SkipToNext()
    {
        if (!isInitialized) return;
        PlayNextSong();
    }

    /// <summary>
    /// Forces the MusicController to synchronize with any existing GamePauseManager and apply volume settings.
    /// Can be called externally to ensure proper volume application.
    /// </summary>
    public void ForceSyncWithGamePauseManager()
    {
        try
        {
            var gamePauseManager = FindObjectOfType<GamePauseManager>();
            if (gamePauseManager != null)
            {
                SyncAudioMixerWithGamePauseManager(gamePauseManager);
                Debug.Log("MusicController: Force sync with GamePauseManager completed");
            }
            else
            {
                Debug.LogWarning("MusicController: No GamePauseManager found for force sync");
                ApplyFallbackVolumeSettings();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MusicController: Error during force sync: {e.Message}");
        }
    }
    
    /// <summary>
    /// Sets the music volume directly on the AudioSources.
    /// This is called by GamePauseManager when the music slider changes.
    /// </summary>
    /// <param name="volume">Volume level (0.0 to 1.0)</param>
    public void SetMusicVolumeLevel(float volume)
    {
        try
        {
            // Clamp volume to valid range
            volume = Mathf.Clamp01(volume);
            
            // Update base volume
            baseAudioVolume = volume;
            
            // Apply to current active audio source (audioSourceA is the active one during crossfading)
            if (audioSourceA != null)
            {
                audioSourceA.volume = volume;
            }
            
            // audioSourceB should remain at 0 or fade out naturally during crossfading
            // The crossfading logic will handle its volume appropriately
            
            Debug.Log($"MusicController: Set music volume level to {volume}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"MusicController: Error setting music volume level: {e.Message}");
        }
    }

    #endregion

    #region Cleanup

    private void OnDestroy()
    {
        // Cleanup Addressable audio loaders
        if (useAddressables && audioLoaders != null)
        {
            foreach (var loader in audioLoaders.Values)
            {
                loader?.UnloadAudio();
            }
            audioLoaders.Clear();
            loadedAddressableAudioClips.Clear();
        }
    }

    #endregion
}

[System.Serializable]
public struct MusicHolder
{
    public string name;
    public AudioClip clip;
}