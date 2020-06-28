using UnityEngine;

public abstract class Tool : ScriptableObject
{
    public abstract ToolType ToolType { get; }
    public ToolInstance ToolModel => ToolModelsDict.Instance.GetModel(ToolType);
}
