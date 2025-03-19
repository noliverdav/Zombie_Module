using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    private BarricadeZone targetBarricade;
    private BarricadePlank targetPlank;
    private Transform targetEntry;
    private Transform player;
    private NavMeshAgent agent;
    private bool isBreakingBarricade = false;
    private Vector3 targetPosition;

    [Header("Barricade Breaking Settings")]
    public float baseBreakTime = 1.5f;
    public float breakTimeVariation = 0.3f;
    public float postBreakDelay = 1.5f;
    public int maxAttackers = 2; // Máximo de enemigos que pueden atacar la barricada

    [Header("Entry Point Positioning")]
    public float minSeparationDistance = 1.0f; // Distancia mínima entre enemigos en la barricada
    public float waitingDistanceOffset = 2.0f; // Distancia extra para los que esperan

    private static HashSet<BarricadePlank> assignedPlanks = new HashSet<BarricadePlank>(); // Planks asignadas
    private static int currentAttackers = 0; // Contador de enemigos atacando la barricada
    private bool isWaitingForTurn = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        FindNearestEntry();
    }

    void Update()
    {
        if (isBreakingBarricade) return;

        if (targetBarricade != null && targetPlank != null && !targetPlank.isDestroyed)
        {
            if (Vector3.Distance(transform.position, targetPosition) < 1.5f)
            {
                agent.isStopped = true;
                StartCoroutine(BreakPlank());
            }
            else
            {
                agent.SetDestination(targetPosition);
            }
        }
        else if (targetBarricade != null && targetBarricade.HasAvailablePlank())
        {
            if (currentAttackers < maxAttackers)
            {
                AssignUniquePlank();
            }
            else
            {
                SetWaitingPosition();
                StartCoroutine(WaitForTurn());
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    void FindNearestEntry()
    {
        GameObject[] entries = GameObject.FindGameObjectsWithTag("EntryPoint");
        float minDistance = float.MaxValue;

        foreach (var entry in entries)
        {
            float distance = Vector3.Distance(transform.position, entry.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                targetEntry = entry.transform;
            }
        }

        if (targetEntry != null)
        {
            FindBarricade(targetEntry);
            SetEntryPosition(targetEntry);
        }
    }

    void FindBarricade(Transform entry)
    {
        BarricadeZone[] barricades = FindObjectsOfType<BarricadeZone>();
        foreach (var barricade in barricades)
        {
            if (Vector3.Distance(barricade.transform.position, entry.position) < 2f && barricade.HasAvailablePlank())
            {
                targetBarricade = barricade;
                AssignUniquePlank();
                return;
            }
        }
    }

    void AssignUniquePlank()
    {
        if (currentAttackers >= maxAttackers) return;

        foreach (var plank in targetBarricade.planks)
        {
            if (!plank.isDestroyed && !assignedPlanks.Contains(plank))
            {
                targetPlank = plank;
                assignedPlanks.Add(plank);
                currentAttackers++;
                Debug.Log($"{gameObject.name} asignado a plank: {targetPlank.gameObject.name}");
                return;
            }
        }
    }

    void SetEntryPosition(Transform entry)
    {
        Vector3 offset = new Vector3(Random.Range(-minSeparationDistance, minSeparationDistance), 0, Random.Range(-minSeparationDistance, minSeparationDistance));
        targetPosition = entry.position + offset;
    }

    void SetWaitingPosition()
    {
        Vector3 offset = new Vector3(Random.Range(-waitingDistanceOffset, waitingDistanceOffset), 0, Random.Range(-waitingDistanceOffset, waitingDistanceOffset));
        targetPosition = targetEntry.position + offset;
        agent.SetDestination(targetPosition);
    }

    IEnumerator BreakPlank()
    {
        isBreakingBarricade = true;

        while (targetPlank != null && !targetPlank.isDestroyed)
        {
            float randomBreakTime = baseBreakTime + Random.Range(-breakTimeVariation, breakTimeVariation);
            yield return new WaitForSeconds(randomBreakTime);
            targetPlank.TakeDamage();
            Debug.Log($"{gameObject.name} destruyó plank: {targetPlank.gameObject.name}");
        }

        assignedPlanks.Remove(targetPlank); // Liberar la tabla para otros enemigos
        targetPlank = null;
        currentAttackers--;

        if (targetBarricade != null && targetBarricade.HasAvailablePlank() && currentAttackers < maxAttackers)
        {
            AssignUniquePlank(); // Asignar una nueva tabla y seguir destruyendo
        }
        else
        {
            yield return new WaitForSeconds(postBreakDelay);
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        isBreakingBarricade = false;
    }

    IEnumerator WaitForTurn()
    {
        isWaitingForTurn = true;
        while (currentAttackers >= maxAttackers)
        {
            yield return new WaitForSeconds(0.5f); // Esperar medio segundo antes de revisar de nuevo
        }
        isWaitingForTurn = false;
        AssignUniquePlank();
    }
}
