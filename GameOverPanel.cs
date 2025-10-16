using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class GameOverSimple : MonoBehaviour
{
    [Header("UI Reference")]
    public TMP_Text titleText; // TextMeshPro object that displays "GAME OVER"

    [Header("Typing Settings")]
    public float charsPerSecond = 12f; // Adjust typing speed
    public float pauseAfterFinish = 0.5f;

    [Header("Scene Settings")]
    public string mainMenuSceneName = "MainMenu";

    private bool isTyping = false;
    private bool typingComplete = false;

    private void Start()
    {
        if (titleText)
        {
            titleText.text = "";
            StartCoroutine(TypeGameOver());
        }
    }

    private IEnumerator TypeGameOver()
    {
        isTyping = true;
        string text = "GAME OVER";
        float delay = 1f / charsPerSecond;

        for (int i = 0; i < text.Length; i++)
        {
            titleText.text = text.Substring(0, i + 1);
            yield return new WaitForSecondsRealtime(delay);
        }

        yield return new WaitForSecondsRealtime(pauseAfterFinish);
        isTyping = false;
        typingComplete = true;
    }

    private void Update()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null && kb.spaceKey.wasPressedThisFrame)
            OnSpacePressed();
#else
        if (Input.GetKeyDown(KeyCode.Space))
            OnSpacePressed();
#endif
    }

    private void OnSpacePressed()
    {
        // Ignore presses during typing
        if (!typingComplete) return;

        // Resume time and load main menu
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
