using Rhyth.BTree;
using UnityEngine;

public class ToolTypeValue : Value
{
    [SerializeField] private ToolType toolType;

    public override Value Clone()
    {
        ToolTypeValue toolTypeValue = CreateInstance<ToolTypeValue>();
        toolTypeValue.toolType = toolType;
        return toolTypeValue;
    }

    public override object GetValue() => toolType;

    public static ToolTypeValue Create(ToolType toolType)
    {
        ToolTypeValue toolTypeValue = CreateInstance<ToolTypeValue>();
        toolTypeValue.toolType = toolType;
        return toolTypeValue;
    }
}
