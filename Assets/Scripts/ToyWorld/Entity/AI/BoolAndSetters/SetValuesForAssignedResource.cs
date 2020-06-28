using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets all values for the toys assigned resourcefield
/// </summary>
public class SetValuesForAssignedResource : BoolNode
{
    [SerializeField] private DynamicValue resourceTypeValue;
    [SerializeField] private ResourceFieldValue resourceFieldValue;
    [SerializeField] private DynamicValue toolTypeValue;

    protected override bool InnerIsFulfilled()
    {
        Toy toy = tree.AttachedBrain.GetComponent<Toy>();

        // If the toy does not have a resourcefield assigned then return failure
        if (toy.AssignedResourceField == null)
            return false;

        // If the resourcefield is non existent anymore or the amount is 0 return failure
        if (!toy.AssignedResourceField || toy.AssignedResourceField.TotalRemaining <= 0)
        {
            toy.AssignedResourceField = null;
            return false;
        }

        // if the values have been set inside the treeEditor then populate them with references
        if (resourceFieldValue != null)
            resourceFieldValue.SetValue(toy.AssignedResourceField);
        if (resourceTypeValue != null)
            resourceTypeValue.SetValue(toy.AssignedResourceField.Type);
        if (toolTypeValue != null)
            toolTypeValue.SetValue(ResourceTypeForToolType.Instance.Get(toy.AssignedResourceField.Type));

        // lastly return success
        return true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetValuesForAssignedResource setValues = CreateInstance<SetValuesForAssignedResource>();

        if (resourceFieldValue != null)
            setValues.resourceFieldValue = (ResourceFieldValue)CloneValue(originalValueForClonedValue, resourceFieldValue);

        if (resourceTypeValue != null)
            setValues.resourceTypeValue = (DynamicValue)CloneValue(originalValueForClonedValue, resourceTypeValue);

        if (toolTypeValue != null)
            setValues.toolTypeValue = (DynamicValue)CloneValue(originalValueForClonedValue, toolTypeValue);

        return setValues;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (resourceFieldValue != null && originalReplace.ContainsKey(resourceFieldValue))
            resourceFieldValue = (ResourceFieldValue)originalReplace[resourceFieldValue];

        if (resourceTypeValue != null && originalReplace.ContainsKey(resourceTypeValue))
            resourceTypeValue = (DynamicValue)originalReplace[resourceTypeValue];

        if (toolTypeValue != null && originalReplace.ContainsKey(toolTypeValue))
            toolTypeValue = (DynamicValue)originalReplace[toolTypeValue];

    }
}
