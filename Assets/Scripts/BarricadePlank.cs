using UnityEngine;

public class BarricadePlank : MonoBehaviour
{
    public bool isDestroyed = false;
    private Collider plankCollider;
    private Renderer plankRenderer;

    void Start()
    {
        plankCollider = GetComponent<Collider>();
        plankRenderer = GetComponent<Renderer>();
    }

    public void TakeDamage()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        UpdatePlankState(false);
    }

    public void Repair()
    {
        isDestroyed = false;
        UpdatePlankState(true);
    }

    void UpdatePlankState(bool state)
    {
        plankCollider.enabled = state;
        plankRenderer.enabled = state;
    }
}
