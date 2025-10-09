using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointMover : MonoBehaviour
{
    public Transform waypointParent;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public bool loopWaypoints = true;

    private Transform[] waypoints;
    private int currentWaypointIndex;
    private bool isWaiting;
    private Animator animator;

    private float lastinputX;
    private float lastinputY;


    void Start()
    {
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
            animator.SetFloat("LastinputX", lastinputX);
            animator.SetFloat("LastinputY", lastinputY);
            return;
        }

        MoveToWaypoint();
    }

    void MoveToWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];
        Vector2 direction = (target.position - transform.position).normalized;

        if (direction.magnitude > 0f)
        {
            lastinputX = direction.x;
            lastinputY = direction.y;   
        }

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
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

        animator.SetFloat("LastinputX", lastinputX);
        animator.SetFloat("LastinputY", lastinputY);

        yield return new WaitForSeconds(waitTime);

        // IF looping is enabled: increment currentWaypointIndex and wrp around if needed
        // If not looping: increment currentWaypointIndex but don't exceed last waypoint
        currentWaypointIndex = loopWaypoints ? (currentWaypointIndex + 1) % waypoints.Length : Mathf.Min(currentWaypointIndex + 1, waypoints.Length - 1);

        isWaiting = false;
    }
}
