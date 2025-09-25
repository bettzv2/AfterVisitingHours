using UnityEngine;
using UnityEngine.UI; 

public class HealthController : MonoBehaviour
{
    public int playerHealth;
    [SerializeField] private Image[] hearts;
     void Start()
    {
        UpdateHealth();
    }
    public void UpdateHealth()
    {
        for(int i = 0; i < hearts.Length; i++)
        {
            if(i < playerHealth)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
