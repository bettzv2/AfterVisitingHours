using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl2D : MonoBehaviour
{
    [SerializeField] float defaultSpeed = 100; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalMovement = 0f;
        float verticalMovement = 0f;

        if (Keyboard.current.wKey.isPressed)
        {
            verticalMovement = 1f;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            verticalMovement = -1f;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            horizontalMovement = -1f;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            horizontalMovement = 1f;
        }

        float horizontalAmount = horizontalMovement * defaultSpeed * Time.deltaTime;
        float verticalAmount = verticalMovement * defaultSpeed * Time.deltaTime;

        transform.Translate(0, verticalAmount, 0);
        transform.Translate(horizontalAmount, 0, 0);
    }
}
