using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource song;
    public AudioSource move;
    

    private float targetVolume = 0.20f; // Default target volume for game scene
    private float targetVolumeMenu = 0.6f; // Target volume for menu scene

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        PlayMenuMusic();
    }

    public void PlayMoveSound()
    {
        move.Play();
    }

    public void PlayMenuMusic()
    {
        song.loop = true;
        song.volume = targetVolumeMenu;
        song.Play();
    }

    public void LowerVolumeForGame()
    {
        StartCoroutine(FadeVolume(targetVolume));
    }

    private IEnumerator FadeVolume(float targetVolume)
    {
        float currentVolume = song.volume;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime / 4;
            song.volume = Mathf.Lerp(currentVolume, targetVolume, t);
            yield return null;
        }
    }
    public void IncreaseVolumeForMenu()
    {
        StartCoroutine(FadeVolume(targetVolumeMenu));
    }
}
