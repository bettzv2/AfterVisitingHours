using UnityEngine;
using UnityEngine.UI;

public class SoundEffectManager : MonoBehaviour
{
    private static SoundEffectManager Instance;

    private AudioSource audioSource;
    private SoundEffectLibrary soundEffectLibrary;

    [SerializeField] private Slider sfxSlider;

    private const string SfxVolumeKey = "SFXVolume";
    private const float DefaultSfxVolume = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
            soundEffectLibrary = GetComponent<SoundEffectLibrary>();

            audioSource.volume = PlayerPrefs.GetFloat(SfxVolumeKey, DefaultSfxVolume);
            // NOTE: No DontDestroyOnLoad — per-scene lifetime.
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        if (sfxSlider)
        {
            sfxSlider.SetValueWithoutNotify(audioSource ? audioSource.volume : DefaultSfxVolume);
            sfxSlider.onValueChanged.AddListener(SetVolume);
        }
    }

    private void OnDisable()
    {
        if (sfxSlider)
        {
            sfxSlider.onValueChanged.RemoveListener(SetVolume);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public static void Play(string soundName)
    {
        if (Instance == null || Instance.soundEffectLibrary == null) return;
        var clip = Instance.soundEffectLibrary.GetRandomClip(soundName);
        if (clip != null) Instance.audioSource.PlayOneShot(clip);
    }

    public static void SetVolume(float volume)
    {
        if (Instance == null) return;
        Instance.audioSource.volume = volume;
        PlayerPrefs.SetFloat(SfxVolumeKey, volume);
        PlayerPrefs.Save();
    }
}
