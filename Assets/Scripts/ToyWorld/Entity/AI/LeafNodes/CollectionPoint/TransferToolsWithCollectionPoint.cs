using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transfers tools with a collection point
/// </summary>
public class TransferToolsWithCollectionPoint : InteractWithCollectionPoint
{
    [SerializeField] private Interaction interaction;
    [SerializeField] private Value toolTypeValue;

    private enum Interaction
    {
        GetSingleTool, PlaceSingleTool, PlaceAllTools
    }

    protected override Status Interact()
    {
        bool success = false;

        switch (interaction)
        {
            case Interaction.GetSingleTool:
                if (toolTypeValue.TryGetValue(out ToolType getToolType))
                {
                    Tool tool = collectionPoint.Get(getToolType);
                    if (tool != null)
                    {
                        toy.Inventory.AddTool(tool);
                        success = true;
                    }
                }
                break;

            case Interaction.PlaceSingleTool:
                if (toolTypeValue.TryGetValue(out ToolType placeToolType))
                {
                    Tool tool = toy.Inventory.GetTool(placeToolType, true);
                    if (tool == null)
                        break;

                    collectionPoint.Add(tool);
                    success = true;
                }
                break;

            case Interaction.PlaceAllTools:
                collectionPoint.PlaceAllTools(toy.Inventory);
                success = true;
                break;

            default:
                Debug.Log("Unimplemented case " + interaction);
                return Status.Failure;
        }

        return success ? Status.Success : Status.Failure;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        TransferToolsWithCollectionPoint interactCollection = CreateInstance<TransferToolsWithCollectionPoint>();
        interactCollection.collectionPointValue = (CollectionPointValue)(CloneValue(originalValueForClonedValue, collectionPointValue));
        interactCollection.interaction = interaction;
        if (toolTypeValue != null)
            interactCollection.toolTypeValue = CloneValue(originalValueForClonedValue, toolTypeValue);
        return interactCollection;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        base.InnerReplaceValues(originalReplace);
        if (toolTypeValue != null && originalReplace.ContainsKey(toolTypeValue))
            toolTypeValue = originalReplace[toolTypeValue];
    }

}
