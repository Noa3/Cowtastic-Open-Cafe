using UnityEngine;
using UnityEngine.Audio;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    [Header("References")]
    [SerializeField]
    private AudioMixer audioMixer;

    public bool ShufflePlaylist = true;
    public MusicHolder[] MusicList;

    public AudioMixerGroup audioMixerGroup;
    private AudioClip audioClip;
    public float crossFadeTime = 4;


    public AudioSource audioSourceA, audioSourceB;
    float audioSourceAVolumeVelocity, audioSourceBVolumeVelocity;

    [Header("Debug/Info")]
    [ReadOnly]
    public float Songtime = 0;
    [ReadOnly]
    public int currentSongNumber = 0;

    private void Awake()
    {
        if (MusicList == null || MusicList.Length == 0)
        {
            Destroy(gameObject);
            return;
        }

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            DoShufflePlaylist();
            audioClip = MusicList[0].clip;

            //Load and set Sound Volume on start from saved data
            SetMusicVolume(PlayerPrefs.GetFloat(Consts.PlayerPrefSoundFx, 0.5f));
            SetEffectsVolume(PlayerPrefs.GetFloat(Consts.PlayerPrefMusic, 0.7f));
            SetTalkVolume(PlayerPrefs.GetFloat(Consts.PlayerPrefTalk, 0.7f));

            Songtime = MusicList[currentSongNumber].clip.length;
        }
        else
        {
            if (gameObject.CompareTag("EditorOnly") == false)
            {
                MusicController.instance.CrossFade(audioClip);
            }
            DestroyImmediate(this);
        }
    }

    public void DoShufflePlaylist()
    {
        if (ShufflePlaylist == true)
        {
            MusicList.Shuffle();
        }
    }

    /// <summary>
    /// Looks in the music list and plays the song
    /// </summary>
    /// <param name="audioClip"></param>
    public void CrossFade(string audioClip)
    {
        foreach (MusicHolder item in MusicList)
        {
            if (item.name == audioClip)
            {
                CrossFade(item.clip);
                break;
            }
        }
        Debug.Log("Song not Found in List!");
    }

    /// <summary>
    /// Put new Song in
    /// </summary>
    /// <param name="audioClip"></param>
    public void CrossFade(AudioClip audioClip)
    {
        if (audioSourceA.clip != audioClip)
        {
            AudioSource t = audioSourceA;
            audioSourceA = audioSourceB;
            audioSourceB = t;
            audioSourceA.clip = audioClip;
        }

        audioSourceA.Play();
    }

    void Update()
    {
        if (audioSourceA.volume > 0.98f)
        {
            audioSourceA.volume = 1;
        }
        else
        {
            audioSourceA.volume = Mathf.SmoothDamp(audioSourceA.volume, 1f, ref audioSourceAVolumeVelocity, crossFadeTime, 1);
        }

        if (audioSourceB.volume < 0.02f)
        {
            audioSourceB.volume = 0;
        }
        else
        {
            audioSourceB.volume = Mathf.SmoothDamp(audioSourceB.volume, 0f, ref audioSourceBVolumeVelocity, crossFadeTime, 1);
        }

        CheckForNextSong();
    }

    private void CheckForNextSong()
    {

        if (audioSourceA.time > (Songtime - crossFadeTime))
        {
            currentSongNumber++;
            if (currentSongNumber >= MusicList.Length)
            {
                DoShufflePlaylist();
                currentSongNumber = 0;
            }

            CrossFade(MusicList[currentSongNumber].clip);
            Songtime = MusicList[currentSongNumber].clip.length;
        }
    }

    void OnEnable()
    {
        if (audioSourceA == null)
        {
            audioSourceA = gameObject.AddComponent<AudioSource>();
        }

        audioSourceA.spatialBlend = 0;
        audioSourceA.clip = audioClip;
        audioSourceA.loop = false;
        audioSourceA.outputAudioMixerGroup = audioMixerGroup;
        audioSourceA.Play();

        if (audioSourceB == null)
        {
            audioSourceB = gameObject.AddComponent<AudioSource>();
        }

        audioSourceB.spatialBlend = 0;
        audioSourceB.loop = false;
        audioSourceB.outputAudioMixerGroup = audioMixerGroup;
    }

    public void SetMusicVolume(float slidervalue)
    {
        audioMixer.SetFloat(Consts.AudioVolumeMusic, Mathf.Log10(slidervalue) * 20);
    }

    public void SetEffectsVolume(float slidervalue)
    {
        audioMixer.SetFloat(Consts.AudioVolumeEffects, Mathf.Log10(slidervalue) * 20);
    }

    public void SetTalkVolume(float slidervalue)
    {
        audioMixer.SetFloat(Consts.AudioVolumeTalk, Mathf.Log10(slidervalue) * 20);
    }

}

[System.Serializable]
public struct MusicHolder
{
    public string name;
    public AudioClip clip;
}