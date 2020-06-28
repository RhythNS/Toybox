using UnityEngine;

/// <summary>
/// Building for Toys to get and put items and tools into. Can be connected to a city for toys
/// to access the town inventory
/// </summary>
[RequireComponent(typeof(Inventory))]
public class CollectionPoint : Building, IBuildingsSelectable
{
    protected override bool ShouldBlockNavMesh => false;

    // Toy needs to be inside this circle to access the collection point
    public Circle3D Circle3D => new Circle3D(transform.position, circle3DRadius);
    public float Circle3DRadius => circle3DRadius;
    [SerializeField] private float circle3DRadius;

    // If this is connected to the town then return the town inventory otherwise return the collection point inventory
    public Inventory ActiveInventory => isConnectedToTownSupply == true ? Town.Inventory : inventory;
    private Inventory inventory;

    public int MaxCapcity => isConnectedToTownSupply == true ? int.MaxValue : inventory.MaxCapcity;
    public int CurrentCapacity => isConnectedToTownSupply == true ? 0 : inventory.CurrentCapacity;
    public int CapacityLeft => isConnectedToTownSupply == true ? int.MaxValue : inventory.CapacityLeft;

    public bool IsConnectedToTownSupply { get => isConnectedToTownSupply; set => isConnectedToTownSupply = value; }
    [SerializeField] private bool isConnectedToTownSupply = false;

    private void Awake()
    {
        inventory = GetComponent<Inventory>();
    }

    public Tool Get(ToolType tool) => ActiveInventory.GetTool(tool, true);

    public bool Get(Item item) => ActiveInventory.Get(item);

    public bool Add(Item item) => ActiveInventory.Add(item);

    public void Add(Tool tool) => ActiveInventory.AddTool(tool);

    /// <summary>
    /// Transfers all items from the inventory to the collection box or from the inventory to the collection
    /// box. Returns true if one item was transfered
    /// </summary>
    public bool TransferAll(Inventory other, bool transferToBox) => ActiveInventory.Transfer(other, !transferToBox);

    public void PlaceAllTools(Inventory other) => ActiveInventory.AddAllTools(other.GetAllTools(true));

    public void Select()
    {
        InventoryPanel.Instance.Open(name, GetComponent<CollectionPoint>().ActiveInventory);
        CollectionPointPanel.Instance.Open(this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, circle3DRadius);
    }

}
