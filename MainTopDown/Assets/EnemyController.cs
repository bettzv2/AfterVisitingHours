using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject player;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float maxRange;
    [SerializeField]
    private float minRange;
    [SerializeField]
    private float maxHealth = 3f;
    private float health;
    public Transform waypointParent;
    public bool loopWaypoints = true;
    public float waitTime = 2f;
    private Transform[] waypoints;
    private int currentWaypointIndex;
    //private float lastinputX;
    //private float lastinputY;
    private bool isWaiting;
    private Animator animator;
    public bool dead = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        health = maxHealth;

        waypoints = new Transform[waypointParent.childCount];

        for (int i = 0; i < waypointParent.childCount; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
    }

    void Update()
    {
        if (MenuController.isPaused || isWaiting)
        {
            animator.SetBool("isWalking", false);
            //animator.SetFloat("LastinputX", lastinputX);
            //animator.SetFloat("LastinputY", lastinputY);
            return;
        }
        else if (animator.GetBool("Dead") == true)
        {
            animator.SetBool("isWalking", false);
        }
        else if (Vector2.Distance(transform.position, player.transform.position) <= maxRange && Vector2.Distance(player.transform.position, transform.position) >= minRange)
        {
            Chase();
        }
        else
        {
            MoveToWaypoint();
        }
    }

    public void Chase()
    {
        Vector2 direction = (player.transform.position - transform.position).normalized;

        animator.SetBool("isWalking", true);
        animator.SetFloat("InputX", direction.x);
        animator.SetFloat("InputY", direction.y);
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;

        //if (direction.magnitude > 0f)
        //{
        //    lastinputX = direction.x;
        //    lastinputY = direction.y;
        //}

        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        animator.SetFloat("InputX", direction.x);
        animator.SetFloat("InputY", direction.y);
        animator.SetBool("isWalking", direction.magnitude > 0f);


        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        animator.SetBool("isWalking", false);

        //animator.SetFloat("LastinputX", lastinputX);
        //animator.SetFloat("LastinputY", lastinputY);

        yield return new WaitForSeconds(waitTime);

        // IF looping is enabled: increment currentWaypointIndex and wrp around if needed
        // If not looping: increment currentWaypointIndex but don't exceed last waypoint
        currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length : Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);

        isWaiting = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            animator.SetBool("Dead", true);
            dead = true;
            Destroy(gameObject, 10f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Weapon")
        {
            Vector2 difference = transform.position - other.transform.position;
            transform.position = new Vector2(transform.position.x + difference.x, transform.position.y + difference.y);
        }
    }

}
