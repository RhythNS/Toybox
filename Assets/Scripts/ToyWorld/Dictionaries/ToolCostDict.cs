using UnityEngine;

[System.Serializable]
public struct ToolCost
{
    public Tool tool;
    public Supplies cost;

    public ToolCost(Tool tool, Supplies cost)
    {
        this.tool = tool;
        this.cost = cost;
    }
}

/// <summary>
/// Dict for all tools with Supply cost
/// </summary>
public class ToolCostDict : MonoBehaviour
{
    public ToolCost[] Tools => tools;
    [SerializeField] private ToolCost[] tools;

    public static ToolCostDict Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
