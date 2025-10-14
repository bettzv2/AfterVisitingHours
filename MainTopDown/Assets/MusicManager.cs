using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    private static MusicManager Instance;

    private AudioSource audioSource;
    public AudioClip backgroundMusic;
    [SerializeField] private Slider musicSlider;

    private const string MusicVolumeKey = "MusicVolume";
    private const float DefaultMusicVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = PlayerPrefs.GetFloat(MusicVolumeKey, DefaultMusicVolume);

            // NOTE: No DontDestroyOnLoad here — this manager is per-scene.
        }
        else
        {
            // Prevent duplicates within the same scene.
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            PlayBackgroundMusic(false, backgroundMusic);
        }
    }

    private void OnEnable()
    {
        if (musicSlider)
        {
            musicSlider.SetValueWithoutNotify(audioSource ? audioSource.volume : DefaultMusicVolume);
            musicSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void OnDisable()
    {
        if (musicSlider)
        {
            musicSlider.onValueChanged.RemoveListener(SetVolume);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public static void SetVolume(float volume)
    {
        if (Instance == null) return;
        Instance.audioSource.volume = volume;
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        PlayerPrefs.Save();
    }

    public static void PlayBackgroundMusic(bool resetSong, AudioClip audioClip = null)
    {
        if (Instance == null) return;

        if (audioClip != null)
            Instance.audioSource.clip = audioClip;

        if (Instance.audioSource.clip != null)
        {
            if (resetSong) Instance.audioSource.Stop();
            Instance.audioSource.Play();
        }
    }

    public static void PauseBackgroundMusic()
    {
        if (Instance == null) return;
        Instance.audioSource.Pause();
    }
}
