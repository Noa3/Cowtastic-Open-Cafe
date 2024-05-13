using UnityEngine;

public class AudioEffectHelper : MonoBehaviour
{
    public AudioClip Effect;


    private AudioSource AS;

    private void Awake()
    {
        AS = GetComponent<AudioSource>();
    }

    public void PlayAudioEffect()
    {
        AS.PlayOneShot(Effect);
    }
}
