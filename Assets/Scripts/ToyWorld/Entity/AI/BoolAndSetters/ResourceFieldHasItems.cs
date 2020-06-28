using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wheter a resource field has items or not
/// </summary>
public class ResourceFieldHasItems : BoolNode
{
    [SerializeField] private ResourceFieldValue resourceField;

    protected override bool InnerIsFulfilled()
    {
        if (resourceField.TryGetValue(out ResourceField field) == false)
            return false;

        return field.TotalRemaining > 0;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        ResourceFieldHasItems fieldHasItems = CreateInstance<ResourceFieldHasItems>();
        fieldHasItems.resourceField = (ResourceFieldValue)CloneValue(originalValueForClonedValue, resourceField);
        return fieldHasItems;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(resourceField))
            resourceField = (ResourceFieldValue)originalReplace[resourceField];
    }

}
