using TMPro;
using UnityEngine;

/// <summary>
/// Script for the buttons of the building panel
/// </summary>
public class BuildingElement : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private BuildingCost building;

    public BuildingCost Building
    {
        get => building;
        set
        {
            building = value;
            text.text = value.buildingPrefab.name + "\n" + value.cost.ToSmallString();
        }
    }

    public void Select()
    {
        UIManager manager = UIManager.Instance;

        // If the town does not have enough Supplies display an error and return
        if (manager.Town.Supplies.CanAdjust(Building.cost) == false)
        {
            manager.DisplayEvent(ToyEvent.NotEnoughResources);
            return;
        }

        // Create a new Building with the BuildingPlacer script and make the camera select it
        Building building = Instantiate(Building.buildingPrefab, manager.Town.transform);
        building.name = Building.buildingPrefab.name;
        building.transform.localScale.Scale(MapValuesDict.Instance.ObjectScaleVector);
        BuildingPlacer buildingPlaceHelper = building.gameObject.AddComponent<BuildingPlacer>();
        buildingPlaceHelper.Cost = Building.cost;
        CameraController.Instance.Select(buildingPlaceHelper.gameObject);
    }

}
