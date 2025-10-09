using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class FlashlightFollowMove : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Player's Rigidbody2D.")]
    public Rigidbody2D rb;
    [Tooltip("Player Transform so we can position the flashlight near the 'hand'.")]
    public Transform playerTransform;
    [Tooltip("Spot Light2D on this object.")]
    public Light2D flashlight;

    [Header("Aim / Rotation")]
    [Tooltip("If your cone/light's 'zero' points UP, set this to 90. If it points RIGHT, leave 0.")]
    public float spriteZeroIsUpOffset = -90f;
    [Range(0f, 50f)] public float turnSmoothing = 20f;
    public float stopThreshold = 0.01f;

    [Header("Offset")]
    [Tooltip("Local offset (relative to player facing RIGHT). This will rotate with aim.")]
    public Vector2 localOffset = new Vector2(0.17f, 0.15f);

    [Header("Toggle")]
    public Key toggleKey = Key.F;
    public bool startOn = true;

    public bool IsOn { get; private set; }

    Vector2 _lastDir = Vector2.down;

    void Awake()
    {
        if (!flashlight) flashlight = GetComponent<Light2D>();
        if (!rb) rb = GetComponentInParent<Rigidbody2D>();
        if (!playerTransform && rb) playerTransform = rb.transform;
    }

    void OnEnable() => SetEnabled(startOn);

    void Update()
    {
        if (toggleKey != Key.None && Keyboard.current != null && Keyboard.current[toggleKey].wasPressedThisFrame)
            SetEnabled(!IsOn);

        if (!IsOn || flashlight == null || rb == null || playerTransform == null) return;

        // 1) Direction from current velocity (keeps last if stopped)
        Vector2 v = rb.linearVelocity;
        if (v.sqrMagnitude > stopThreshold * stopThreshold)
            _lastDir = v.normalized;

        // 2) Target angle for the cone
        float targetAngle = Mathf.Atan2(_lastDir.y, _lastDir.x) * Mathf.Rad2Deg + spriteZeroIsUpOffset;

        // 3) Smooth rotation
        float smoothZ = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, 1f - Mathf.Exp(-turnSmoothing * Time.deltaTime));
        transform.rotation = Quaternion.Euler(0f, 0f, smoothZ);

        // 4) Position: rotate the local offset by the SAME angle the cone faces (minus the sprite offset part)
        //    The offset is defined assuming "facing RIGHT" (0 degrees). So rotate by (targetAngle - spriteZeroIsUpOffset).
        Quaternion rot = Quaternion.Euler(0f, 0f, targetAngle - spriteZeroIsUpOffset);
        Vector2 rotatedOffset = rot * localOffset;

        transform.position = (Vector2)playerTransform.position + rotatedOffset;
    }

    public void SetEnabled(bool enabled)
    {
        IsOn = enabled;
        if (flashlight) flashlight.enabled = enabled;
        // Optional: hide any sprite/mesh here too
    }
}
