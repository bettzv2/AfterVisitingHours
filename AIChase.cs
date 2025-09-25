using UnityEngine;

public class AIChase : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float distanceBetween;
    public float distance2Stop;

    private float distance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        chase();
    }

    public void chase()
    { 
        distance = Vector2.Distance(transform.position, player.transform.position); //chase
        Vector2 direction = player.transform.position - transform.position; //chase
        direction.Normalize(); //rotating
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; //rotating

        if (distance <= distance2Stop)
        {  
            transform.rotation = Quaternion.Euler(Vector3.forward * angle); //rotating
        }
        else if (distance < distanceBetween)
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, speed * Time.deltaTime); //chase
            transform.rotation = Quaternion.Euler(Vector3.forward * angle); //rotating
        }
    }
}
