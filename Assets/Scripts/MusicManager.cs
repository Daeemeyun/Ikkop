using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;

    void Start()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
        musicSource.Play();
    }

    void Update()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
    }
}
