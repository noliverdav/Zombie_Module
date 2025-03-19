using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    [Header("Crouch Settings")]
    [Tooltip("Altura normal del Character Controller")]
    public float standingHeight = 1.8f;

    [Tooltip("Altura cuando el jugador se agacha")]
    public float crouchHeight = 1f;

    [Tooltip("Velocidad de movimiento al agacharse")]
    public float crouchSpeed = 2f;

    [Tooltip("Si es true, el jugador debe mantener presionado el boton para agacharse. Si es false, el jugador cambia de estado con un toque.")]
    public bool holdToCrouch = true;

    private CharacterController characterController;
    private PlayerMovement playerMovement;
    private float originalWalkSpeed;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerMovement = GetComponent<PlayerMovement>();
        originalWalkSpeed = playerMovement.walkSpeed;
    }

    void Update()
    {
        HandleCrouch();
    }

    void HandleCrouch()
    {
        if (holdToCrouch)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Crouch();
            }
            else
            {
                StandUp();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (playerMovement.isCrouching)
                    StandUp();
                else
                    Crouch();
            }
        }
    }

    void Crouch()
    {
        characterController.height = crouchHeight;
        playerMovement.walkSpeed = crouchSpeed;
        playerMovement.isCrouching = true;
    }

    void StandUp()
    {
        characterController.height = standingHeight;
        playerMovement.walkSpeed = originalWalkSpeed;
        playerMovement.isCrouching = false;
    }
}
