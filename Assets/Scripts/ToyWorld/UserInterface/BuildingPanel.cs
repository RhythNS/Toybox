using UnityEngine;

/// <summary>
/// Panel for building Buildings
/// </summary>
public class BuildingPanel : MonoBehaviour
{
    [SerializeField] private BuildingElement elementPrefab;
    [SerializeField] private RectTransform content;

    public static BuildingPanel Instance { get; private set; }

    private void Start()
    {
        Instance = this;

        // Get all buildings and add parent them to the content transform
        BuildingCost[] buildingCosts = BuildingCostDict.Instance.Buildings;
        for (int i = 0; i < buildingCosts.Length; i++)
        {
            BuildingElement ele = Instantiate(elementPrefab, content);
            ele.Building = buildingCosts[i];
        }
    }
}
