using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public EnemyController enemyHealth;
    public int damage = 2;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enemyHealth.dead == false)
        {
            if (collision.gameObject.tag == "Player")
            {
                playerHealth.TakeDamage(damage);
            }
        }
        else
        {
            Debug.Log("dead");
        }
    }
}
