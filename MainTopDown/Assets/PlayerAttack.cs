using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject Melee;
    bool isAttacking = false;
    float atkDuration = 0.3f;
    float atkTimer = 0f;
    public PlayerMovement playerMovement;

    void Update()
    {
        CheckMeleeTimer();
    }

    public void OnAttack()
    {
        if (!isAttacking)
        {
            Melee.SetActive(true);
            isAttacking = true;
            playerMovement.PerformAttack();
        }
    }

    private void CheckMeleeTimer()
    {
        if (isAttacking)
        {
            atkTimer += Time.deltaTime;
            if (atkTimer >= atkDuration)
            {
                atkTimer = 0;
                isAttacking = false;
                Melee.SetActive(false);

            }
        }
    }

}
