using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Representation of a Town
/// </summary>
[RequireComponent(typeof(TownInventory))]
public class Town : MonoBehaviour
{
    [SerializeField] private Supplies supplies;

    public TownInventory Inventory { get; private set; }
    public Supplies Supplies { get => supplies; private set => supplies = value; }
    public List<Building> Buildings { get; private set; }
    public List<Toy> Toys { get; private set; }
    public List<CollectionPoint> CollectionPoints { get; private set; }
    public Toori Toori { get; private set; }

    private void Awake()
    {
        supplies = ConstantsDict.Instance.StartingSupplies.Clone();
        Buildings = new List<Building>();
        Toys = new List<Toy>();
        CollectionPoints = new List<CollectionPoint>();
        Inventory = GetComponent<TownInventory>();
        Inventory.Town = this;
    }

    private void Start()
    {
        if (TownDict.Instance.Towns.Contains(this) == false)
            TownDict.Instance.Add(this);
    }

    /// <summary>
    /// Places a spawn Toori on the given position
    /// </summary>
    public void PlaceToori(Vector3 position)
    {
        Toori = Instantiate(NonBuildableBuildingsDict.Instance.Toori, transform);
        Toori.Town = this;
        Toori.transform.localScale.Scale(MapValuesDict.Instance.ObjectScaleVector);
        Toori.transform.position = position;
    }

    /// <summary>
    /// Spawn a toy on the toori. Returns wheter it successeded or not
    /// </summary>
    public bool SpawnToy(string name = null)
    {
        // Get the cost to spawn a toy and try to adjust the supplies on the cost
        int cost = ConstantsDict.Instance.FoodCostPerToyCreation;
        if (Supplies.AdjustAmount(ResourceType.Food, -cost) == false) // if there are not enough supplies, give an error
        {
            UIManager.Instance.DisplayEvent(ToyEvent.NotEnoughResources);
            return false;
        }

        // Try to spawn a toy
        if (Toori.SpawnToy(name) == false)
        {
            // If for some reason it did not work then readd the supplies
            if (Supplies.AdjustAmount(ResourceType.Food, cost) == false) // display error if the readding did also not successed
                Debug.LogWarning("Could not spawn toy but already payed for its cost. For some reason the food can not be readded!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Adds a building to the town and assign its Town value to this town
    /// </summary>
    public void AddBuilding(Building building)
    {
        building.Town = this;
        Buildings.Add(building);

        // If the building is a collection point then add it to the collection point list
        if (building is CollectionPoint col)
            CollectionPoints.Add(col);
    }

    /// <summary>
    /// Removes a building from the town
    /// </summary>
    /// <param name="building"></param>
    public void RemoveBuilding(Building building)
    {
        Buildings.Remove(building);

        // If the building is a collection point then remove it from the collection point list
        if (building is CollectionPoint col)
            CollectionPoints.Remove(col);
    }

    /// <summary>
    /// Tries to craft the given tool
    /// </summary>
    public void CraftTool(ToolCost toolCost)
    {
        // If there are not enough supplies then display an error
        if (supplies.AdjustAmount(toolCost.cost) == false)
        {
            UIManager.Instance.DisplayEvent(ToyEvent.NotEnoughResources);
            return;
        }

        Inventory.AddTool(toolCost.tool);
    }

    /// <summary>
    /// Update the city connection on the collection point
    /// </summary>
    public void ConnectCollectionPointToCity(CollectionPoint collectionPoint, bool establishConnection)
    {
        // if it wants to disconnect than just disconnet it
        if (establishConnection == false)
        {
            collectionPoint.IsConnectedToTownSupply = false;
            return;
        }

        // Otherwise if it is not in range of the toori then display an error
        if ((collectionPoint.transform.position - Toori.transform.position).sqrMagnitude
            > ConstantsDict.Instance.SquaredDistancePickupBoxToToori)
        {
            UIManager.Instance.DisplayEvent(ToyEvent.CollBoxTooFar);
            return;
        }

        // Everything successeded so set the connection 
        collectionPoint.IsConnectedToTownSupply = establishConnection;
    }
}
