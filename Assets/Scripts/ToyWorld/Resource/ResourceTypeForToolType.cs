using UnityEngine;

/// <summary>
/// Gets a ResourceType for a given ToolType
/// </summary>
public class ResourceTypeForToolType : MonoBehaviour
{
    [System.Serializable]
    public struct ResourceForTool
    {
        public ResourceType resourceType;
        public ToolType toolType;
    }

    [SerializeField] private ResourceForTool[] resourceForTool;

    public static ResourceTypeForToolType Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public ToolType Get(ResourceType resourceType)
    {
        for (int i = 0; i < resourceForTool.Length; i++)
        {
            if (resourceForTool[i].resourceType == resourceType)
                return resourceForTool[i].toolType;
        }
        throw new System.Exception("ResourceType not found " + resourceType);
    }
}
