using UnityEngine;

/// <summary>
/// Dict for all ToolModels based on the toolType
/// </summary>
public class ToolModelsDict : MonoBehaviour
{
    public static ToolModelsDict Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [System.Serializable]
    public struct ToolForType
    {
        public ToolInstance model;
        public ToolType type;
    }

    [SerializeField] private ToolForType[] toolForTypes;

    public ToolInstance GetModel(ToolType type)
    {
        for (int i = 0; i < toolForTypes.Length; i++)
        {
            if (toolForTypes[i].type == type)
            {
                return toolForTypes[i].model;
            }
        }
        Debug.LogError("Could not find model for " + type);
        return null;
    }

}
