using System.Collections.Generic;
using UnityEngine;

public class AudioHelper : MonoBehaviour
{
    public AudioSource AS;

    public AudioClip[] clips;
    [HideInInspector]
    public List<string> ClipNames;

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    private void Awake()
    {
        for (int i = 0; i < clips.Length; i++)
        {
            ClipNames.Add(clips[i].name);
        }
    }

    public void PlayAudioOneShot(string name)
    {
        AS.PlayOneShot( clips[ClipNames.IndexOf(name)] );
    }
}
