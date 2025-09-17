using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;

    // Start is called before the first frame update
    void Start()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
    }
}
