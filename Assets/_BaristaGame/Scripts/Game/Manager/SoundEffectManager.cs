using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip levelUp;
    [SerializeField] private AudioClip newOrder;
    [SerializeField] private AudioClip mouseClick;
    [SerializeField] private AudioClip menuSelection;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float masterVolume = 1f;

    public static SoundEffectManager instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern mit DontDestroyOnLoad
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);

            // AudioSource validieren oder erstellen
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
                if (audioSource == null)
                {
                    audioSource = gameObject.AddComponent<AudioSource>();
                }
            }
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    #region Public Sound Methods
    public void PlayLevelUpEffect()
    {
        PlaySoundOneShot(levelUp);
    }

    public void PlayNewOrderEffect()
    {
        PlaySoundOneShot(newOrder);
    }

    public void PlayMenuSelection()
    {
        PlaySoundOneShot(menuSelection);
    }

    public void PlayMouseClick()
    {
        PlaySoundOneShot(mouseClick);
    }
    #endregion

    #region Core Sound Playing Methods
    public void PlaySoundOneShot(AudioClip clip)
    {
        PlaySoundOneShot(clip, masterVolume);
    }

    public void PlaySoundOneShot(AudioClip clip, float volume)
    {
        PlaySoundOneShot(clip, volume, 1f);
    }

    public void PlaySoundOneShot(AudioClip clip, float volume, float pitch)
    {
        if (clip == null || audioSource == null)
        {
            return;
        }

        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, volume * masterVolume);
    }
    #endregion

    #region Volume Control
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }
    #endregion

    #region Validation
    private void OnValidate()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    #endregion
}