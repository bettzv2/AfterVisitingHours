using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 3;
    [SerializeField] private UnityEngine.UI.Image[] hearts;
    public PlayerMovement playerMovement;

    void Start()
    {
        health = maxHealth;
        UpdateHealth();
    }

    public void UpdateHealth()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = (i < health);
        }

        if (health <= 0)
        {
            playerMovement.enabled = false;
            Debug.Log("Player is dead!");
        }
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        UpdateHealth();
    }

    void LateUpdate()
    {
        bool show = !MenuController.isPaused;   // true when not paused
        for (int i = 0; i < hearts.Length; i++)
        {
            // Only show hearts when not paused AND i < current health
            hearts[i].enabled = show && (i < health);
        }
    }
}
