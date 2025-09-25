using UnityEngine;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Jumping")]
    public float jumpForce = 14f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Read input (A/D or Left/Right)
        moveInput = Input.GetAxisRaw("Horizontal"); // -1, 0, 1

        // Flip sprite to face movement direction
        if (sr && Mathf.Abs(moveInput) > 0.01f)
            sr.flipX = moveInput < 0f;

        // Jump (Space) only if grounded
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            // Reset vertical speed for consistent jumps, then add impulse
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        // Variable jump height (release early = shorter jump)
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
    }

    void FixedUpdate()
    {
        // Apply horizontal movement in physics step
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    bool IsGrounded()
    {
        if (!groundCheck) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // Draw ground check gizmo in Scene view
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
