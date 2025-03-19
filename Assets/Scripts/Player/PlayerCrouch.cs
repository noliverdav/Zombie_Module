using UnityEngine;
using System.Collections;

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

    [Tooltip("Duracion de la transicion de agacharse y levantarse")]
    public float crouchTransitionTime = 0.2f;

    private CharacterController characterController;
    private PlayerMovement playerMovement;
    private float originalWalkSpeed;
    private Coroutine crouchCoroutine;

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
        if (crouchCoroutine != null) StopCoroutine(crouchCoroutine);
        crouchCoroutine = StartCoroutine(TransitionCrouch(crouchHeight, crouchSpeed));
        playerMovement.isCrouching = true;
    }

    void StandUp()
    {
        if (crouchCoroutine != null) StopCoroutine(crouchCoroutine);
        crouchCoroutine = StartCoroutine(TransitionCrouch(standingHeight, originalWalkSpeed));
        playerMovement.isCrouching = false;
    }

    IEnumerator TransitionCrouch(float targetHeight, float targetSpeed)
    {
        float startHeight = characterController.height;
        float startSpeed = playerMovement.walkSpeed;
        float elapsedTime = 0f;

        while (elapsedTime < crouchTransitionTime)
        {
            elapsedTime += Time.deltaTime;
            characterController.height = Mathf.Lerp(startHeight, targetHeight, elapsedTime / crouchTransitionTime);
            playerMovement.walkSpeed = Mathf.Lerp(startSpeed, targetSpeed, elapsedTime / crouchTransitionTime);
            yield return null;
        }

        characterController.height = targetHeight;
        playerMovement.walkSpeed = targetSpeed;
    }
}