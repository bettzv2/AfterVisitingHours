using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartMenuController : MonoBehaviour
{
    [Header("Buttons")]
    public Button startGameButton;
    public Button optionsButton;
    public Button howToPlayButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Panels")]
    public GameObject startMenuPanel;   // parent of Title + ButtonColumn
    public GameObject optionsPanel;
    public GameObject howToPlayPanel;
    public GameObject creditsPanel;

    [Header("Config")]
    public string firstLevelSceneName = "Level1";

    [Header("Focus")]
    public Selectable defaultSelected;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (EventSystem.current && defaultSelected)
            EventSystem.current.SetSelectedGameObject(defaultSelected.gameObject);

        if (startGameButton) startGameButton.onClick.AddListener(OnStartGame);
        if (optionsButton) optionsButton.onClick.AddListener(() => { Debug.Log("[Menu] Options clicked"); ShowPanel(optionsPanel, true); });
        if (howToPlayButton) howToPlayButton.onClick.AddListener(() => { Debug.Log("[Menu] HowTo clicked"); ShowPanel(howToPlayPanel, true); });
        if (creditsButton) creditsButton.onClick.AddListener(() => { Debug.Log("[Menu] Credits clicked"); ShowPanel(creditsPanel, true); });
        if (quitButton) quitButton.onClick.AddListener(OnQuit);

        // Auto-wire Back buttons inside each panel (if present)
        WireBackButton(optionsPanel);
        WireBackButton(howToPlayPanel);
        WireBackButton(creditsPanel);

        // Only the start menu is visible at boot
        SetActiveSafe(startMenuPanel, true);
        SetActiveSafe(optionsPanel, false);
        SetActiveSafe(howToPlayPanel, false);
        SetActiveSafe(creditsPanel, false);

        Debug.Log($"[Menu] Init | startMenu={Has(startMenuPanel)} options={Has(optionsPanel)} howTo={Has(howToPlayPanel)} credits={Has(creditsPanel)}");
    }

    void WireBackButton(GameObject panel)
    {
        if (!panel) return;
        var backTf = panel.transform.Find("BackButton");
        if (!backTf) { Debug.Log($"[Menu] No BackButton under {panel.name}"); return; }

        var backButton = backTf.GetComponent<Button>();
        if (!backButton) { Debug.Log($"[Menu] BackButton has no Button on {panel.name}"); return; }

        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(OnBackFromPanel);
    }

    public void OnStartGame() => SceneManager.LoadScene("Level1");

    public void OnBackFromPanel()
    {
        Debug.Log("[Menu] Back pressed");
        ShowPanel(null, false);
    }

    void ShowPanel(GameObject panelToShow, bool show)
    {
        Debug.Log($"[Menu] ShowPanel -> {(panelToShow ? panelToShow.name : "null")} show={show}");

        // Hide all subpanels first
        SetActiveSafe(optionsPanel, false);
        SetActiveSafe(howToPlayPanel, false);
        SetActiveSafe(creditsPanel, false);

        if (show)
        {
            if (!panelToShow) { Debug.LogWarning("[Menu] ShowPanel called with null panel"); return; }

            // Hide main menu, show requested panel
            SetActiveSafe(startMenuPanel, false);
            panelToShow.SetActive(true);

            var back = panelToShow.transform.Find("BackButton");
            if (back && EventSystem.current)
                EventSystem.current.SetSelectedGameObject(back.gameObject);
        }
        else
        {
            // Return to main menu
            SetActiveSafe(startMenuPanel, true);

            if (EventSystem.current && defaultSelected)
                EventSystem.current.SetSelectedGameObject(defaultSelected.gameObject);
        }
    }

    static void SetActiveSafe(GameObject go, bool active)
    {
        if (go && go.activeSelf != active) go.SetActive(active);
    }

    static string Has(GameObject go) => go ? "OK" : "NULL";

    public void OnQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

