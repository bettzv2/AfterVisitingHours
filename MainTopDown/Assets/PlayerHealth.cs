using System;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 3;
    [SerializeField] private UnityEngine.UI.Image[] hearts;
    public GameObject gameOverScreen;

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
            MenuController.isPaused = true;
            Debug.Log("Player is dead!");
            gameOverScreen.SetActive(true);
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
