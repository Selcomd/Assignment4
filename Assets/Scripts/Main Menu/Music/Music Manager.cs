using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Playlist")]
    [SerializeField] private AudioClip[] songs;

    [Header("Settings")]
    [SerializeField] [Range(0f, 1f)] private float defaultVolume = 1f;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool shuffle = false;

    private AudioSource audioSource;
    private int currentIndex = 0;

    private static MusicManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        float vol = AudioSettingsManager.instance != null ? AudioSettingsManager.instance.GetVolume(AudioCategory.BackgroundMusic) : defaultVolume;

        audioSource.volume = vol;
    }

    private void OnEnable()
    {
        if (AudioSettingsManager.instance != null) AudioSettingsManager.instance.OnMusicVolumeChanged += OnMusicVolumeChanged;
    }

    private void OnDisable()
    {
        if (AudioSettingsManager.instance != null) AudioSettingsManager.instance.OnMusicVolumeChanged -= OnMusicVolumeChanged;
    }

    private void OnMusicVolumeChanged(float newVolume)
    {
        if (audioSource != null) audioSource.volume = newVolume;
    }

    private void Start()
    {
        if (songs.Length == 0)
        {
            Debug.LogWarning("MusicManager: No songs assigned!");
            return;
        }

        if (playOnStart) PlayCurrentSong();
    }

    private void Update()
    {
        if (audioSource != null && !audioSource.isPlaying && songs.Length > 0) PlayNextSong();
    }

    private void PlayCurrentSong()
    {
        if (songs[currentIndex] == null)
        {
            Debug.LogWarning($"MusicManager: Song at index {currentIndex} is null, skipping.");
            PlayNextSong();
            return;
        }

        audioSource.clip = songs[currentIndex];
        audioSource.Play();

        Debug.Log($"MusicManager: Now playing ({currentIndex + 1}/{songs.Length}) - {songs[currentIndex].name}");
    }

    private void PlayNextSong()
    {
        if (shuffle)
        {
            int nextIndex = currentIndex;
            if (songs.Length > 1) while (nextIndex == currentIndex) nextIndex = Random.Range(0, songs.Length);
            currentIndex = nextIndex;
        }
        else
        {
            currentIndex = (currentIndex + 1) % songs.Length;
        }

        PlayCurrentSong();
    }

    public void SkipSong()
    {
        audioSource.Stop();
        PlayNextSong();
    }

    public void Pause() => audioSource.Pause();
    public void Resume() => audioSource.UnPause();
}