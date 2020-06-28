using UnityEngine;

[System.Serializable]
public struct BuildingCost
{
    public Building buildingPrefab;
    public Supplies cost;

    public BuildingCost(Building buildingPrefab, Supplies cost)
    {
        this.buildingPrefab = buildingPrefab;
        this.cost = cost;
    }
}

/// <summary>
/// Dict for all Buildings which can be built
/// </summary>
public class BuildingCostDict : MonoBehaviour
{
    public BuildingCost[] Buildings => buildings;
    [SerializeField] private BuildingCost[] buildings;

    public static BuildingCostDict Instance;

    private void Awake()
    {
        Instance = this;
    }
}
