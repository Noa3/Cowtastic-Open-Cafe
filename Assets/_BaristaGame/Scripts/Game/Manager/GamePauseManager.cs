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

    //// Start is called before the first frame update
    void Start()
    {
        barista = BaristaController.instance;

        VolumeSoundFx = PlayerPrefs.GetFloat(Consts.PlayerPrefSoundFx, VolumeSoundFx);
        SliderSoundFx.value = VolumeSoundFx;

        VolumeMusic = PlayerPrefs.GetFloat(Consts.PlayerPrefMusic, VolumeMusic);
        SliderMusic.value = VolumeMusic;

        VolumeTalk = PlayerPrefs.GetFloat(Consts.PlayerPrefTalk, VolumeTalk);
        SliderTalk.value = VolumeTalk;

        TextureQuality = PlayerPrefs.GetInt(Consts.PlayerPrefTextureQuality, TextureQuality);
        SliderTextureQuality.value = TextureQuality;



#if UNITY_ANDROID
        ToggleCameraMove.isOn = PlayerPrefs.GetInt(Consts.PlayerPrefCanMoveCamera, 0) == 1 ? true : false;
#else
        ToggleCameraMove.isOn = PlayerPrefs.GetInt(Consts.PlayerPrefCanMoveCamera, 1) == 1 ? true : false;
#endif

        ToggleAntiAlaising.isOn = PlayerPrefs.GetInt(Consts.PlayerPrefAntiAlaising, 0) == 1 ? true : false;
        QualitySettings.antiAliasing = ToggleAntiAlaising.isOn ? 1 : 0;

        ToggleVsync.isOn = PlayerPrefs.GetInt(Consts.PlayerPrefVsync, 0) == 1 ? true : false;
        QualitySettings.vSyncCount = ToggleVsync.isOn ? 1 : 0;

        ToggleAutoFixApron.isOn = bool.Parse(PlayerPrefs.GetString(Consts.PlayerPrefAutoFixClothes, false.ToString()));
        if (barista != null)
        {
            SetBarsistaAutoFixOutfit(ToggleAutoFixApron.isOn);
            //barista.AutoFixOutfit = ToggleAutoFixApron.isOn;
        }


        CameraPan.instance.enabled = ToggleCameraMove.isOn;
        //Debug.Log("Camera:" + ToggleCameraMove.isOn);

        ToggleFullscreen.isOn = Screen.fullScreen;


        ToggleShowBestTimes.isOn = PlayerPrefs.GetInt(Consts.PlayerPrefShowBestTimes, 0) == 1 ? true : false;
        if (TextTime != null)
        {
            TextTime?.SetActive(ToggleShowBestTimes.isOn);
        }
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SetGamePause(bool Pause)
    {
        GamePaused = Pause;

        if (GamePaused == true)
        {
            Time.timeScale = 0;
            Pausepanel.SetActive(true);
            SettingsButton.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            Pausepanel.SetActive(false);
            SettingsButton.SetActive(true);
            SaveValues();
        }
    }

    public void SetGameFullscreen(bool b)
    {
        Screen.fullScreen = b;
    }

    public void SaveValues()
    {
        Debug.Log("SaveValues: " + SliderSoundFx.value + " " + SliderMusic.value + " " + SliderTalk.value);
        PlayerPrefs.SetFloat(Consts.PlayerPrefSoundFx, SliderSoundFx.value);
        PlayerPrefs.SetFloat(Consts.PlayerPrefMusic, SliderMusic.value);
        PlayerPrefs.SetFloat(Consts.PlayerPrefTalk, SliderTalk.value);
        PlayerPrefs.SetString( Consts.PlayerPrefAutoFixClothes, ToggleAutoFixApron.isOn.ToString() );

        PlayerPrefs.SetInt( Consts.PlayerPrefTextureQuality, Mathf.RoundToInt(SliderTextureQuality.value) );

        PlayerPrefs.Save();
    }

    public void SetBarsistaAutoFixOutfit(bool value)
    {
        if (barista != null)
        {
            barista.AutoFixOutfit = value;
        }
    }

    public void ChangePauseState()
    {
        SetGamePause(!GamePaused);
    }

    public void OpenDiscordURL()
    {
        Application.OpenURL(DiscordURL);
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

    public void SetCameraPossibileMove(bool on)
    {
        PlayerPrefs.SetInt(Consts.PlayerPrefCanMoveCamera,on ? 1 : 0);
        CameraPan.instance.enabled = on;
        Debug.Log("Camera:" + on);
    }

    public void SetShowPlayerBestTime(bool on)
    {
        PlayerPrefs.SetInt(Consts.PlayerPrefShowBestTimes, on ? 1 : 0);
        if (TextTime != null)
        {
            TextTime.SetActive(on);
        }
    }

    public void SetVsync(bool state)
    {
        PlayerPrefs.SetInt(Consts.PlayerPrefVsync, state ? 1 : 0);
        QualitySettings.vSyncCount = state ? 1 : 0;
        Debug.Log("Vsync: " + QualitySettings.vSyncCount);
    }

    public void SetAntialaising(bool state)
    {
        PlayerPrefs.SetInt(Consts.PlayerPrefAntiAlaising, state ? 1 : 0);
        QualitySettings.antiAliasing = state ? 1 : 0;
        Debug.Log("AntiAlaising: " + QualitySettings.antiAliasing);
    }

    public void SetTextureQuality(float state)
    {
        QualitySettings.globalTextureMipmapLimit = Mathf.RoundToInt(state);
        Debug.Log("TextureQuality: " + QualitySettings.globalTextureMipmapLimit);
    }

    public void ResetRecordTimes()
    {
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeCasual);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeNormal);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeHard);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeChaos);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeUltraChaos);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeNoasMod);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeCasual + Consts.PlayerPrefBestTimeMilkymodeSuffix);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeNormal + Consts.PlayerPrefBestTimeMilkymodeSuffix);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeHard + Consts.PlayerPrefBestTimeMilkymodeSuffix);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefBestTimeUltraChaos + Consts.PlayerPrefBestTimeMilkymodeSuffix);


        PlayerPrefs.DeleteKey(Consts.PlayerPrefMostEarned);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefMostServed);
        PlayerPrefs.DeleteKey(Consts.PlayerPrefMostMilk);
    }
}
