using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Equips a tool for a toy
/// </summary>
public class EquipTool : DelayedTimeNode
{
    public override int MaxNumberOfChildren => 0;

    [SerializeField] private Value toolTypeValue;

    private ToolType value;

    protected override bool PreConditionCheck()
    {
        if (toolTypeValue.TryGetValue(out ToolType gottenValue) == false)
            return false;
        value = gottenValue;
        return true;
    }

    protected override DynamicValue GetDynamicValue()
        => tree.AttachedBrain.GetComponent<Toy>().QueueEquipEvent(value);

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        EquipTool equipTool = CreateInstance<EquipTool>();
        equipTool.toolTypeValue = CloneValue(originalValueForClonedValue, toolTypeValue);
        return equipTool;
    }

    public static EquipTool CreateFromToolType(ToolType toolType)
    {
        ToolTypeValue typeValue = ToolTypeValue.Create(toolType);
        EquipTool equipTool = CreateInstance<EquipTool>();
        equipTool.toolTypeValue = typeValue;
        return equipTool;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(toolTypeValue))
            toolTypeValue = originalReplace[toolTypeValue];
    }
}
