using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// References to all Towns
/// </summary>
public class TownDict : MonoBehaviour
{
    public static TownDict Instance { get; private set; }

    public List<Town> Towns => towns;
    [SerializeField] private List<Town> towns;

    private void Awake()
    {
        Instance = this;
    }

    public void Add(Town town)
    {
        if (Towns.Contains(town) == true)
        {
            Debug.Log("Town " + town.name + " already registerd!");
            return;
        }
        Towns.Add(town);
    }

    public void Remove(Town town)
    {
        for (int i = 0; i < Towns.Count; i++)
        {
            if (Towns[i] == town)
            {
                Towns.RemoveAt(i);
                return;
            }
        }
        Debug.Log("Town " + town.name + " not found to remove!");
    }
}
