using System.Threading.Tasks;
using _BaristaGame.Scripts.AddressablesScripts;
using UnityEngine;

public class AudioLoader : MonoBehaviour
{
    private LocalAudioLoader _loader;

    private async void Start()
    {
        var audioSource = GetComponent<AudioSource>();
        _loader = new LocalAudioLoader();
        var audioFile = await GetAddressablesAudioClip("Intro_NewJobNewLife_01");

        audioSource.clip = audioFile;
        audioSource.Play();
    }

    private async Task<AudioClip> GetAddressablesAudioClip(string audioName)
    {
        return await _loader.LoadAudioAsync(audioName);
    }
}
