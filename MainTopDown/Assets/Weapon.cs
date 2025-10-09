using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage = 1f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

}
