using UnityEngine;
using System.Collections;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina General")]
    [Tooltip("Cantidad maxima de stamina del jugador")]
    public float maxStamina = 100f;

    [SerializeField]
    [Tooltip("Stamina actual del jugador, se actualiza en tiempo real")]
    private float stamina;

    [Header("Sprint")]
    [Tooltip("Cuanta stamina se gasta por segundo al sprintar")]
    public float sprintStaminaDrain = 25f;

    [Tooltip("Stamina minima requerida para poder sprintar")]
    public float minStaminaToSprint = 20f;

    [Header("Salto")]
    [Tooltip("Cuanta stamina se gasta por salto")]
    public float jumpStaminaCost = 15f;

    [Header("Regeneracion")]
    [Tooltip("Velocidad de regeneracion normal de stamina por segundo")]
    public float normalStaminaRegen = 15f;

    [Tooltip("Velocidad de regeneracion cuando la stamina esta por debajo del umbral minimo")]
    public float lowStaminaRegen = 5f;

    [Tooltip("Tiempo de espera antes de que comience la regeneracion de stamina despues de dejar de sprintar o saltar")]
    public float staminaRegenDelay = 2f;

    private bool isRegenerating = false;

    private Coroutine regenCoroutine;

    void Start()
    {
        stamina = maxStamina;
    }

    public void HandleStamina(bool isSprinting)
    {
        if (isSprinting && stamina > 0)
        {
            stamina -= sprintStaminaDrain * Time.deltaTime;
            if (stamina < minStaminaToSprint) stamina = minStaminaToSprint;
            StopRegen();
        }
        else if (!isRegenerating)
        {
            StartRegen();
        }

        stamina = Mathf.Clamp(stamina, 0f, maxStamina);
    }

    public bool CanSprint()
    {
        return stamina > minStaminaToSprint;
    }

    public bool HasEnoughStamina(float amount)
    {
        return stamina >= amount;
    }

    public void ConsumeStamina(float amount)
    {
        stamina -= amount;
        stamina = Mathf.Clamp(stamina, 0f, maxStamina);
        StopRegen();
    }

    private void StartRegen()
    {
        if (regenCoroutine != null) StopCoroutine(regenCoroutine);
        regenCoroutine = StartCoroutine(RegenerateStamina());
    }

    private void StopRegen()
    {
        if (regenCoroutine != null)
        {
            StopCoroutine(regenCoroutine);
            regenCoroutine = null;
        }
        isRegenerating = false;
    }

    private IEnumerator RegenerateStamina()
    {
        isRegenerating = true;
        yield return new WaitForSeconds(staminaRegenDelay);

        while (stamina < maxStamina)
        {
            float regenRate = (stamina <= minStaminaToSprint) ? lowStaminaRegen : normalStaminaRegen;
            stamina += regenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, maxStamina);
            yield return null;
        }

        isRegenerating = false;
    }
}
