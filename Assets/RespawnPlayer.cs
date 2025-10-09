using UnityEngine;

public class RespawnPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public HealthController healthController;
    private Transform respawnPoint;
    private Transform enemies;
    void Start()
    {
        respawnPoint = GameObject.Find("RespawnPoint").transform;
        enemies = GameObject.Find("Enemies").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthController.playerHealth <= 0)
        {
            Respawn();
            RespawnEnemies();
        }
    }
    void Respawn()
    {
        transform.position = respawnPoint.position;
        healthController.playerHealth = 3;
        healthController.UpdateHealth();
    }
    void RespawnEnemies()
    {
        foreach (Transform enemy in enemies)
        {
            enemy.gameObject.SetActive(true);
        }
    }
}
