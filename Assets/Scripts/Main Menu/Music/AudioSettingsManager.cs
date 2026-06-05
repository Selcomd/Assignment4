using UnityEngine;
using System;

public enum AudioCategory
{
    GuardCaught,
    GuardSpotted,
    GuardLost,
    BackgroundMusic,
    KeycardPickup
}

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager instance;

    private float guardCaughtVolume  = 1f;
    private float guardSpottedVolume = 1f;
    private float guardLostVolume = 1f;
    private float musicVolume = 1f;
    private float keycardVolume = 1f;

    public event Action<float> OnMusicVolumeChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadSettings();
    }

    public float GetVolume(AudioCategory category)
    {
        switch (category)
        {
            case AudioCategory.GuardCaught: return guardCaughtVolume;
            case AudioCategory.GuardSpotted: return guardSpottedVolume;
            case AudioCategory.GuardLost: return guardLostVolume;
            case AudioCategory.BackgroundMusic: return musicVolume;
            case AudioCategory.KeycardPickup: return keycardVolume;
            default: return 1f;
        }
    }

    public void SetVolume(AudioCategory category, float value)
    {
        value = Mathf.Clamp01(value);

        switch (category)
        {
            case AudioCategory.GuardCaught: guardCaughtVolume = value; break;
            case AudioCategory.GuardSpotted: guardSpottedVolume = value; break;
            case AudioCategory.GuardLost: guardLostVolume = value; break;
            case AudioCategory.BackgroundMusic: musicVolume = value;
                OnMusicVolumeChanged?.Invoke(value);
                break;
            case AudioCategory.KeycardPickup: keycardVolume = value; break;
        }

        SaveSettings();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("Vol_GuardCaught", guardCaughtVolume);
        PlayerPrefs.SetFloat("Vol_GuardSpotted", guardSpottedVolume);
        PlayerPrefs.SetFloat("Vol_GuardLost", guardLostVolume);
        PlayerPrefs.SetFloat("Vol_Music", musicVolume);
        PlayerPrefs.SetFloat("Vol_Keycard", keycardVolume);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        guardCaughtVolume  = PlayerPrefs.GetFloat("Vol_GuardCaught", 1f);
        guardSpottedVolume = PlayerPrefs.GetFloat("Vol_GuardSpotted", 1f);
        guardLostVolume = PlayerPrefs.GetFloat("Vol_GuardLost", 1f);
        musicVolume = PlayerPrefs.GetFloat("Vol_Music", 1f);
        keycardVolume = PlayerPrefs.GetFloat("Vol_Keycard", 1f);
    }
}