using UnityEngine;

public class AimDirection : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Player's Rigidbody2D (read movement direction from here).")]
    public Rigidbody2D rb;                 // drag Player's Rigidbody2D
    [Tooltip("Child melee (triangle) under Aim.")]
    public Transform melee;                // drag your 'melee' child

    [Header("Facing / Rotation")]
    [Range(0f, 50f)] public float turnSmoothing = 20f;
    [Tooltip("Velocity magnitude below which we keep last facing.")]
    public float stopThreshold = 0.01f;
    [Tooltip("If your triangle sprite's 'zero' points UP, set 90. If it points RIGHT, leave 0.")]
    public float triangleZeroIsUpOffset = 0f;

    [Header("Melee Placement")]
    [Tooltip("How far in front of the player to place the triangle.")]
    public float meleeDistance = 0.35f;

    Vector2 _lastDir = Vector2.down;

    void Awake()
    {
        if (!rb) rb = GetComponentInParent<Rigidbody2D>();
        if (!melee && transform.childCount > 0)
            melee = transform.GetChild(0);
    }

    void OnEnable()
    {
        // Snap immediately when Aim/melee gets enabled.
        UpdateAimAndMelee(snap:true);
    }

    void Update()
    {
        // Smooth rotation here so it looks nice.
        UpdateAimAndMelee(snap:false);
    }

    void LateUpdate()
    {
        // Ensure melee position wins over any Animator/other scripts.
        PlaceMeleeWorld();
    }

    void UpdateAimAndMelee(bool snap)
    {
        if (!rb) return;

        // 1) Determine facing using velocity; keep last if nearly stopped.
        Vector2 v = rb.linearVelocity;
        if (v.sqrMagnitude > stopThreshold * stopThreshold)
            _lastDir = v.normalized;

        // 2) Compute Aim's target angle (Aim's +X is forward)
        float targetAngle = Mathf.Atan2(_lastDir.y, _lastDir.x) * Mathf.Rad2Deg + triangleZeroIsUpOffset;

        // 3) Smoothly rotate Aim
        float z = snap
            ? targetAngle
            : Mathf.LerpAngle(transform.eulerAngles.z, targetAngle,
                              1f - Mathf.Exp(-turnSmoothing * Time.deltaTime));
        transform.rotation = Quaternion.Euler(0f, 0f, z);
    }

    void PlaceMeleeWorld()
    {
        if (!melee) return;

        // Place melee in FRONT of Aim/player using the world-facing direction.
        Vector2 worldPos = (Vector2)transform.position + _lastDir * meleeDistance;
        melee.position = worldPos;
    }
}
