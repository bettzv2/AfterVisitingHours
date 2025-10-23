using UnityEngine;
using UnityEngine.Video;

public class ElevatorController : MonoBehaviour, IInteractable
{
    [Header("References")]
    [SerializeField] private LightSwitch lightSwitch;   // assign in Inspector
    [SerializeField] private VideoPlayer videoPlayer;   // assign in Inspector

    public bool IsOn { get; private set; }
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnDisable()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }

    void Start()
    {
        SetOn(IsOn);
        if (videoPlayer != null)
        {
            videoPlayer.Stop();
            // Start hidden so it doesn't linger on scene load
            videoPlayer.gameObject.SetActive(false);
        }
    }

    public bool CanInteract()
    {
        return lightSwitch != null && lightSwitch.IsOn;
    }

    public void Interact() => Entering();

    private void Entering()
    {
        if (!CanInteract() || videoPlayer == null) return;

        // Show video surface and play
        if (!videoPlayer.gameObject.activeSelf)
            videoPlayer.gameObject.SetActive(true);

        videoPlayer.Play();

        // If you have an animation:
        // animator?.SetTrigger("Enter");
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        // Stop and hide the video surface so it "disappears"
        vp.Stop();
        vp.gameObject.SetActive(false);
    }

    public void SetOn(bool what)
    {
        IsOn = what;
    }
}
