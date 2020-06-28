using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets a random resourceField
/// </summary>
public class SetRandomResourceFromField : BoolNode
{
    [SerializeField] private ResourceFieldValue resourceField;
    [SerializeField] private ResourceValue resource;

    protected override bool InnerIsFulfilled()
    {
        if (resourceField.TryGetValue(out ResourceField field) == false)
            return false;

        resource.SetValue(field.RandomResource);

        return true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetRandomResourceFromField setRandomResource = CreateInstance<SetRandomResourceFromField>();
        setRandomResource.resourceField = (ResourceFieldValue)CloneValue(originalValueForClonedValue, resourceField);
        setRandomResource.resource = (ResourceValue)CloneValue(originalValueForClonedValue, resource);
        return setRandomResource;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(resourceField))
            resourceField = (ResourceFieldValue)originalReplace[resourceField];
        if (originalReplace.ContainsKey(resource))
            resource = (ResourceValue)originalReplace[resource];
    }
}
