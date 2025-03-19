using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement movement;
    private PlayerLook look;
    private PlayerStamina stamina;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        look = GetComponent<PlayerLook>();
        stamina = GetComponent<PlayerStamina>();
    }

    void Update()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && stamina.CanSprint();
        stamina.HandleStamina(isSprinting);

        movement.HandleMovement();
        look.HandleLook();
    }
}
