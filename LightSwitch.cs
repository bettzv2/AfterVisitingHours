using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [Header("Light")]
    [SerializeField] private Light2D roomLight;
    [SerializeField] private float onIntensity = 1f;
    [SerializeField] private float offIntensity = 0f;

    [Header("Sprites")]
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    public bool IsOn { get; private set; }
    public string SwitchID { get; private set; }

    private SpriteRenderer sr;

    void Start()
    {
        SwitchID ??= GlobalHelper.GenerateUniqueID(gameObject);
        sr = GetComponent<SpriteRenderer>();

        // Initialize state
        SetOn(IsOn);
        if (roomLight != null)
            roomLight.intensity = IsOn ? onIntensity : offIntensity;
    }

    public bool CanInteract() => true;

    public void Interact() => TurnSwitch();

    private void TurnSwitch()
    {
        SetOn(!IsOn); 
        if (roomLight != null)
            roomLight.intensity = IsOn ? onIntensity : offIntensity;
    }

    public void SetOn(bool on)
    {
        IsOn = on;
        if (sr != null)
            sr.sprite = IsOn ? onSprite : offSprite;
    }
}
