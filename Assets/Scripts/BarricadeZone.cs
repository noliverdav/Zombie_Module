using System.Collections.Generic;
using UnityEngine;

public class BarricadeZone : MonoBehaviour
{
    [Header("Tablas de la Barricada")]
    public List<BarricadePlank> planks = new List<BarricadePlank>();

    public bool HasAvailablePlank()
    {
        return planks.Exists(plank => !plank.isDestroyed);
    }

    public BarricadePlank GetAvailablePlank()
    {
        return planks.Find(plank => !plank.isDestroyed);
    }

    public void RepairPlank()
    {
        foreach (var plank in planks)
        {
            if (plank.isDestroyed)
            {
                plank.Repair();
                break;
            }
        }
    }
}
