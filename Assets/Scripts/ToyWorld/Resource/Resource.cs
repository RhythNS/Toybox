using UnityEngine;

/// <summary>
/// A point where a toy can gather resources from
/// </summary>
public abstract class Resource : MonoBehaviour
{
    /// <summary>
    /// For dynamicly changing the Mesh when the amount changes
    /// </summary>
    public MeshFilter MeshFilter { get; private set; }

    public enum GatheringDrop
    {
        SpawnInWorld, DirectlyIntoInventory
    }

    /// <summary>
    /// Wheter the model should change when the amount changes
    /// </summary>
    protected virtual bool ChangeModelOnAmountChange => true;

    [SerializeField] private int amountLeft;
    public int AmountLeft
    {
        get => amountLeft;
        protected set
        {
            // When the value changes deciede if we should display a new model
            if (ChangeModelOnAmountChange == true &&
                ResourceItemDict.Instance.GetNewModel(ResourceType, amountLeft, value, out Mesh newMesh))
                MeshFilter.mesh = newMesh;
            amountLeft = value;
            if (amountLeft <= 0) // if the amount is 0 then call OnNoAmountLeft()
                OnNoAmountLeft();
        }
    }

    /// <summary>
    /// The parent field of this resource
    /// </summary>
    public ResourceField ParentField { get; set; }

    /// <summary>
    /// The resourceType that is gained when a toy gathers this resource
    /// </summary>
    public abstract ResourceType ResourceType { get; }

    /// <summary>
    /// The required ToolType for gathering this resource
    /// </summary>
    public abstract ToolType RequiredToolType { get; }

    /// <summary>
    /// Wheter the given position is in range for the resource to be gathered
    /// </summary>
    public abstract bool AtCollectionPoint(Vector3 position);

    /// <summary>
    /// Wheter the tool can harvest the resource
    /// </summary>
    public abstract bool TryCollecting(Tool tool);

    /// <summary>
    /// Gets a ResourceItem when gathering from this resource
    /// </summary>
    public abstract GatheringDrop Gather(out ResourceItem item);

    /// <summary>
    /// The next point for this resource to be collected from.
    /// </summary>
    public abstract Circle3D NextCollectionPoint { get; }

    private void Awake()
    {
        MeshFilter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        if (ResourceItemDict.Instance.GetNewModel(ResourceType, int.MaxValue, amountLeft, out Mesh newMesh))
            MeshFilter.mesh = newMesh;
    }

    /// <summary>
    /// Called when no resources are left to be gatherd
    /// </summary>
    protected virtual void OnNoAmountLeft()
    {
        ParentField.Remove(this);
        Destroy(gameObject);
    }

    /// <summary>
    /// Check it the given Toy can gather the resouce. 
    /// </summary>
    public bool CheckCollectionRequirements(Toy gatherer)
    {
        // If there are no resources left return false
        if (AmountLeft <= 0)
            return false;

        // Get the Current Tool
        ToolInstance toolInstance = gatherer.ActiveTool;
        if (toolInstance == null) // Does not have a tool
        {
            if (RequiredToolType != ToolType.None)
                return false;
        }
        else // Does have a tool
        {
            if (RequiredToolType != toolInstance.Tool.ToolType)
                return false;
        }

        // Is the toy at the collection point
        if (AtCollectionPoint(gatherer.transform.position) == false)
            return false;

        return true;
    }

}
