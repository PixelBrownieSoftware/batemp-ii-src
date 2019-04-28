using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_soundmanager : MonoBehaviour
{
    public static s_soundmanager sound;
    public List<AudioClip> sounds = new List<AudioClip>();
    public AudioSource src;

    void Start()
    {
        src = GetComponent<AudioSource>();
        sound = GetComponent<s_soundmanager>();
    }

    public void PlaySound(string soundName)
    {
        AudioClip soundClip = sounds.Find(x => x.name == soundName);
        if (soundClip == null)
            return;
        src.PlayOneShot(soundClip);
    }
}
