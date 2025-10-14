using Unity.VisualScripting;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public EnemyController enemyHealth;
    public int damage = 2;
    [SerializeField] private string playAttack;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enemyHealth.dead == false)
        {
            if (collision.gameObject.tag == "Player")
            {
                playerHealth.TakeDamage(damage);
                SoundEffectManager.Play(playAttack);
            }
        }
        else
        {
            Debug.Log("dead");
        }
    }
}
