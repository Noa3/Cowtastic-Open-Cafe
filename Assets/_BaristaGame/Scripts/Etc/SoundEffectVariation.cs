using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffectVariation : MonoBehaviour
{
    public AudioSource effectSource;

    [Range(0.9f, 1.1f)]
    public float pitchMin = 0.9f;
    [Range(0.9f, 1.1f)]
    public float pitchMax = 1.1f;
    [Range(0.8f, 1f)]
    public float volumeMin = 0.8f;
    [Range(0.8f, 1f)]
    public float volumeMax = 1f;

    public AudioClip[] clipArray;

    private bool isPlaying = false;
    private int lastClipIndex = -1;

    public void PlayRandomLoop(bool changePitch = true)
    {
        if (clipArray.Length == 0 || isPlaying)
            return;

        isPlaying = true;

        if (changePitch)
            SetRandomPitch();
        else
            ResetPitch();

        int randomIndex = GetRandomClipIndex();
        effectSource.loop = true;
        effectSource.clip = clipArray[randomIndex];
        effectSource.Play();
    }

    public void EndLoop()
    {
        isPlaying = false;
        effectSource.Stop();
    }

    public void PlayRandomOneShot(bool changePitch = true)
    {
        if (clipArray.Length == 0)
            return;

        if (changePitch)
            SetRandomPitch();
        else
            ResetPitch();

        int randomIndex = GetRandomClipIndex();
        effectSource.loop = false;
        effectSource.PlayOneShot(clipArray[randomIndex]);
    }

    public void PlayRandomOneShot(AudioClip clip)
    {
        SetRandomPitch();
        effectSource.PlayOneShot(clip);
    }

    private int GetRandomClipIndex()
    {
        int randomIndex = Random.Range(0, clipArray.Length);
        while (randomIndex == lastClipIndex && clipArray.Length > 1)
        {
            randomIndex = Random.Range(0, clipArray.Length);
        }
        lastClipIndex = randomIndex;
        return randomIndex;
    }

    public void SetRandomPitch()
    {
        effectSource.pitch = Random.Range(pitchMin, pitchMax);
        effectSource.volume = Random.Range(volumeMin, volumeMax);
    }

    public void ResetPitch()
    {
        effectSource.pitch = 1f;
        effectSource.volume = 1f;
    }
}