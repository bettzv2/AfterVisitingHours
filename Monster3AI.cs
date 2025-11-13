using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.AI;

public class Monster3AI : MonoBehaviour
{
    public Transform player;
    public Transform fakeGeneratorSpawnPoint;

    [Header("Speeds")]
    public float stalkSpeed = 1.5f;   // slow stalking
    public float fleeSpeed = 4f;      // fast retreat

    [Header("Behavior Distances")]
    public float warningDistance = 8f;     // distance to play warning sound

    [Header("Retreat Settings")]
    public float retreatDuration = 3f;     // how long it retreats after light hit

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip warningClip;
    public AudioClip screamClip;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Optional Pathfinding")]
    public bool useNavMesh = false;
    private NavMeshAgent agent;

    private bool hasPlayedWarning = false;
    private bool isInLight = false;

    private enum State { Stalking, Fleeing, Idle }
    private State currentState = State.Stalking;

    private Coroutine retreatRoutine;

    void Awake()
    {
        if (useNavMesh)
        {
            agent = GetComponent<NavMeshAgent>();
        }
    }

    void Start()
    {
        // Only active on Level 3 (optional safety)
        if (SceneManager.GetActiveScene().name != "Level3")
        {
            gameObject.SetActive(false);
            return;
        }

        // Spawn at fake generator
        if (fakeGeneratorSpawnPoint != null)
        {
            transform.position = fakeGeneratorSpawnPoint.position;
        }

        // Start mostly hidden
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case State.Stalking:
                HandleStalking();
                break;
            case State.Fleeing:
                HandleFleeing();
                break;
        }

        UpdateVisibility();
    }

    // STALKING 

    void HandleStalking()
    {
        // Move toward player (simple chase; replace with your pathfinding if needed)
        Vector3 dir = (player.position - transform.position).normalized;

        if (useNavMesh && agent != null)
        {
            agent.speed = stalkSpeed;
            agent.SetDestination(player.position);  
        }
        else
        {
            transform.position += dir * stalkSpeed * Time.deltaTime;
        }

        float dist = Vector3.Distance(transform.position, player.position);

        // Play warning sound once when close enough
        if (dist <= warningDistance && !hasPlayedWarning)
        {
            if (audioSource && warningClip)
                audioSource.PlayOneShot(warningClip);

            hasPlayedWarning = true;
        }

        // Optional: stalking animation
        if (animator)
            animator.SetBool("IsStalking", true);
    }

    // ---------------- FLEEING (RETREAT AFTER LIGHT) ----------------

    void HandleFleeing()
    {
        // Movement is handled in the coroutine, but if you want,
        // you can still update animations here.
        if (animator)
        {
            animator.SetBool("IsStalking", false);
            animator.SetBool("IsRetreating", true);
        }
    }

    IEnumerator RetreatCoroutine()
    {
        currentState = State.Fleeing;

        // Reset warning so it can play again next time it comes close
        hasPlayedWarning = false;

        // Scream SFX
        if (audioSource && screamClip)
            audioSource.PlayOneShot(screamClip);

        // Trigger scream/retreat animation
        if (animator)
            animator.SetTrigger("Scream");

        float t = 0f;

        while (t < retreatDuration)
        {
            t += Time.deltaTime;

            if (player != null)
            {
                Vector3 awayDir = (transform.position - player.position).normalized;

                if (useNavMesh && agent != null)
                {
                    agent.speed = fleeSpeed;
                    agent.SetDestination(transform.position + awayDir * 5f);
                }
                else
                {
                    transform.position += awayDir * fleeSpeed * Time.deltaTime;
                }
            }

            yield return null;
        }

        // Go back to stalking after retreat
        if (animator)
            animator.SetBool("IsRetreating", false);

        currentState = State.Stalking;
    }

    // ---------------- LIGHT DETECTION ----------------

    // Call this when the monster is hit by the player's flashlight light
    public void OnLightHit()
    {
        isInLight = true;

        // Ensure visible when screaming
        if (spriteRenderer)
            spriteRenderer.color = Color.white;

        if (retreatRoutine != null)
            StopCoroutine(retreatRoutine);

        retreatRoutine = StartCoroutine(RetreatCoroutine());
    }

    // Using a trigger collider for the light cone
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Light"))
        {
            OnLightHit();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Light"))
        {
            isInLight = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Light"))
        {
            isInLight = false;
        }
    }

    // Visitibility

    void UpdateVisibility()
    {
        // Hidden / mostly invisible while stalking in the dark
        if (!isInLight && currentState == State.Stalking)
        {
            if (spriteRenderer)
                spriteRenderer.color = new Color(1f, 1f, 1f, 0.1f); // transparent
        }
        else
        {
            if (spriteRenderer)
                spriteRenderer.color = Color.white;
        }
    }
}
