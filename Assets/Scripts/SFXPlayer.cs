using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioClip clickSound;
    public AudioClip saveSound;
    public AudioClip doorSound;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void PlayClick()
    {
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.6f);
        sfxSource.PlayOneShot(clickSound);
    }

    public void PlaySave()
    {
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.6f);
        sfxSource.PlayOneShot(saveSound);
    }

    public void PlayDoor()
    {
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.6f);
        sfxSource.PlayOneShot(doorSound);
    }
}
