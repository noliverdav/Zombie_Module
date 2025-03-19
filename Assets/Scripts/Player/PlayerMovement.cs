using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("Velocidad al caminar")]
    public float walkSpeed = 5f;

    [Tooltip("Velocidad al sprintar")]
    public float sprintSpeed = 8f;

    [Header("Salto y Gravedad")]
    [Tooltip("Fuerza del salto")]
    public float jumpForce = 7f;

    [Tooltip("Fuerza de gravedad aplicada al jugador")]
    public float gravity = 9.81f;

    [Tooltip("Tiempo de espera entre saltos")]
    public float jumpCooldown = 0.5f;

    [Header("Deteccion de Suelo")]
    [Tooltip("Capas que se consideran como suelo")]
    public LayerMask groundMask;

    [Tooltip("Distancia del raycast para detectar si el jugador esta en el suelo")]
    public float groundCheckDistance = 0.2f;

    public bool isCrouching = false;

    private CharacterController characterController;

    private Vector3 velocity;
    private bool isGrounded;
    private bool canJump = true;

    private PlayerStamina stamina;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        stamina = GetComponent<PlayerStamina>();
    }

    void Update()
    {
        CheckGround();
        HandleMovement();
    }

    void CheckGround()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;
    }

    public void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        bool isSprinting = !isCrouching && Input.GetKey(KeyCode.LeftShift) && stamina.CanSprint() && moveZ > 0;
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        stamina.HandleStamina(isSprinting);

        characterController.Move(move * currentSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded && canJump && stamina.HasEnoughStamina(stamina.jumpStaminaCost))
        {
            velocity.y = jumpForce;
            stamina.ConsumeStamina(stamina.jumpStaminaCost);
            canJump = false;
            StartCoroutine(JumpCooldown());
        }

        velocity.y -= gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }
}
