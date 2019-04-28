using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class misc_intro : MonoBehaviour
{
    AudioSource aud;
    private void Start()
    {
        aud = GetComponent<AudioSource>();
    }

    public void Transition()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    public void Jingle()
    {
        aud.Play();
    }
}
