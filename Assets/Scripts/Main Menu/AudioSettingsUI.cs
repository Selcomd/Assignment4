using UnityEngine;
using UnityEngine.UI;
using System;

public class AudioSettingsUI : MonoBehaviour
{
    [Serializable]
    public class AudioSliderEntry
    {
        public string label;
        public AudioCategory category;
        public Slider slider;
    }

    [Header("Audio Sliders")]
    [SerializeField] private AudioSliderEntry[] sliders;

    private void Start()
    {
        if (AudioSettingsManager.instance == null) return;

        foreach (AudioSliderEntry entry in sliders)
        {
            if (entry.slider == null) continue;

            entry.slider.value = AudioSettingsManager.instance.GetVolume(entry.category);

            AudioCategory cat = entry.category;
            entry.slider.onValueChanged.AddListener(value => { AudioSettingsManager.instance.SetVolume(cat, value); });
        }
    }
}