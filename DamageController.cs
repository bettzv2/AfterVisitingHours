using UnityEngine;

public class DamageController : MonoBehaviour
{
    [SerializeField] private int playerDamage; // default value
    [SerializeField] private HealthController _healthController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Damage();
        }
    }

    private void Damage()
    {
       
        _healthController.playerHealth -= playerDamage;
        _healthController.UpdateHealth();
        gameObject.SetActive(false);
    }
}
