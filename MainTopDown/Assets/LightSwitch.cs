using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [Header("Light")]
    [SerializeField] private Light2D roomLight;
    [SerializeField] private float onIntensity = 1f;
    [SerializeField] private float offIntensity = 0f;

    public bool IsOn { get; private set; }
    //public string SwitchID { get; private set; }
    private Animator animator;

    void Start()
    {
        // SwitchID ??= GlobalHelper.GenerateUniqueID(gameObject);

        animator = GetComponent<Animator>();

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
        {
            if (IsOn == true)
            {
                animator.SetBool("IsOn", true);
                roomLight.intensity = onIntensity;
            }
            else if (IsOn == false)
            {
                animator.SetBool("IsOn", false);
                roomLight.intensity = offIntensity;
            }
        }
    }

    public void SetOn(bool what)
    {
        IsOn = what;
    }
}
