using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TypewriterIntro : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text titleText;
    public TMP_Text bodyText;
    public TMP_Text continueHint;
    public Button skipButton;
    public Button pauseButton;
    public Button mainMenuButton;

    [Header("Typing Settings")]
    public float charsPerSecond = 20f;
    public float punctuationPause = 0.20f;
    public float ellipsisPause = 0.40f;
    public float pageEndDelay = 0.50f;

    [Header("Audio Settings")]
    public AudioClip typingSound;
    [Range(0f, 1f)] public float typingVolume = 0.6f; // lower to avoid clipping
    [Tooltip("Do not change unless you really need pitch variation.")]
    public float minPitch = 1.0f;
    public float maxPitch = 1.0f;
    private AudioSource audioSource;

    [Header("Audio Timing")]
    [Tooltip("Play SFX on every audible char (still gated). If off, uses everyNChars.")]
    public bool playEveryCharacter = false;
    [Tooltip("If not every char, play once per N audible chars.")]
    public int everyNChars = 3;
    [Tooltip("Absolute minimum time between SFX (realtime).")]
    public float minSfxInterval = 0.08f; // ~12 clicks/sec max

    // Internals for audio gate
    private float _lastSfxTimeUnscaled = -999f;
    private int _playedCharCount = 0;
    private float _clipGateInterval = 0.08f; // will be set from clip length

    [Header("Scenes")]
    public string gameSceneName = "GameScene";
    public string mainMenuSceneName = "MainMenu";

    // Intro copy
    private readonly string[] pages =
    {
        "You're trapped inside the hospital’s security room...\n\n" +
        "A terrible outbreak has spread through the building.\n" +
        "Power is gone... systems offline... communications dead.\n\n" +
        "You must restore power through the generator room.\n" +
        "Maybe then, you can escape.\n\n" +
        "The only way out is through the elevators...\n" +
        "But they’ve stopped working.\n" +
        "Restore power... and try to stay alive."
    };

    private const string HintText = "Press Space to Play";
    private int currentPageIndex = 0;
    private Coroutine typingRoutine;
    private bool isTyping = false;
    private bool isPaused = false;

    private void Awake()
    {
        if (titleText) titleText.text = "AFTER VISITING HOURS";
        if (continueHint) continueHint.gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f; // 2D
        audioSource.volume = 1f;       // we control loudness via PlayOneShot volume

        // If clip assigned, gate by its length so we don't stack/clip
        if (typingSound)
        {
            // Let clicks ring out ~70% of their length before next one
            _clipGateInterval = Mathf.Max(0.02f, typingSound.length * 0.70f);
        }

        if (skipButton) skipButton.onClick.AddListener(OnSkipPressed);
        if (pauseButton) pauseButton.onClick.AddListener(OnPausePressed);
        if (mainMenuButton) mainMenuButton.onClick.AddListener(OnMainMenuPressed);
    }

    private void Start()
    {
        StartPage(currentPageIndex);
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.spaceKey.wasPressedThisFrame) OnSkipPressed();
            if (kb.pKey.wasPressedThisFrame) OnPausePressed();
            if (kb.escapeKey.wasPressedThisFrame) OnMainMenuPressed();
        }
#else
        if (Input.GetKeyDown(KeyCode.Space))  OnSkipPressed();
        if (Input.GetKeyDown(KeyCode.P))      OnPausePressed();
        if (Input.GetKeyDown(KeyCode.Escape)) OnMainMenuPressed();
#endif
    }

    private void StartPage(int index)
    {
        if (index < 0 || index >= pages.Length) return;

        if (typingRoutine != null) StopCoroutine(typingRoutine);

        if (continueHint) continueHint.gameObject.SetActive(false);
        if (bodyText) bodyText.text = string.Empty;

        _lastSfxTimeUnscaled = -999f;
        _playedCharCount = 0;

        typingRoutine = StartCoroutine(TypeTextRealtime(pages[index]));
    }

    private IEnumerator TypeTextRealtime(string fullText)
    {
        isTyping = true;
        int visibleCount = 0;
        float tPerChar = 1f / Mathf.Max(1f, charsPerSecond);

        while (visibleCount < fullText.Length)
        {
            if (!isPaused)
            {
                visibleCount++;
                if (bodyText) bodyText.text = fullText.Substring(0, visibleCount);

                char c = fullText[visibleCount - 1];
                bool isAudibleChar = c != ' ' && c != '\n' && c != '\r' && c != '\t';
                if (isAudibleChar) _playedCharCount++;

                // Gate: time AND count, and never overlap (skip if source is already playing)
                if (typingSound && audioSource && isAudibleChar)
                {
                    bool dueByCount = playEveryCharacter || (_playedCharCount % Mathf.Max(1, everyNChars) == 0);

                    // Effective min interval is the max of user min, clip gate, and 60% of char spacing
                    float effectiveMinInterval = Mathf.Max(minSfxInterval, _clipGateInterval, tPerChar * 0.6f);
                    bool dueByTime = (Time.unscaledTime - _lastSfxTimeUnscaled) >= effectiveMinInterval;

                    // Also skip if the source is still playing to avoid harsh overlap
                    bool sourceFree = !audioSource.isPlaying;

                    if (dueByCount && dueByTime && sourceFree)
                    {
                        audioSource.pitch = Mathf.Clamp(Random.Range(minPitch, maxPitch), 0.5f, 2f);
                        audioSource.PlayOneShot(typingSound, typingVolume);
                        _lastSfxTimeUnscaled = Time.unscaledTime;
                    }
                }

                float wait = tPerChar;
                if (c == '.' || c == ',' || c == '!' || c == '?' || c == ';' || c == ':')
                    wait += punctuationPause;

                if (visibleCount >= 3 &&
                    fullText[visibleCount - 1] == '.' &&
                    fullText[visibleCount - 2] == '.' &&
                    fullText[visibleCount - 3] == '.')
                    wait += ellipsisPause;

                yield return new WaitForSecondsRealtime(wait);
            }
            else
            {
                yield return null;
            }
        }

        isTyping = false;
        yield return new WaitForSecondsRealtime(pageEndDelay);

        if (continueHint)
        {
            continueHint.text = HintText;
            continueHint.gameObject.SetActive(true);
        }
    }

    private void OnSkipPressed()
    {
        if (isTyping)
        {
            RevealCurrentPageInstantly();
            return;
        }

        if (currentPageIndex < pages.Length - 1)
        {
            currentPageIndex++;
            StartPage(currentPageIndex);
        }
        else
        {
            LoadGameScene();
        }
    }

    private void OnPausePressed()
    {
        isPaused = !isPaused;
        if (pauseButton)
        {
            var label = pauseButton.GetComponentInChildren<TMP_Text>();
            if (label) label.text = isPaused ? "Resume" : "Pause";
        }
    }

    private void OnMainMenuPressed()
    {
        if (string.IsNullOrEmpty(mainMenuSceneName))
        {
            Debug.LogWarning("Main Menu scene name is empty. Set it in the Inspector.");
            return;
        }

        if (isTyping) RevealCurrentPageInstantly();
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void RevealCurrentPageInstantly()
    {
        if (typingRoutine != null) StopCoroutine(typingRoutine);
        isTyping = false;

        if (bodyText && currentPageIndex >= 0 && currentPageIndex < pages.Length)
            bodyText.text = pages[currentPageIndex];

        if (continueHint)
        {
            continueHint.text = HintText;
            continueHint.gameObject.SetActive(true);
        }
    }

    private void LoadGameScene()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("Game scene name is empty. Set 'gameSceneName' in the Inspector and add it to Build Settings.");
            return;
        }
        SceneManager.LoadScene(gameSceneName);
    }

    // Optional slider hook
    public void SetTypingVolume(float v) => typingVolume = Mathf.Clamp01(v);
}
