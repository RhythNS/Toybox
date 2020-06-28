using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gets a the next Resource from a given resoureField
/// </summary>
public class GetNextResourceField : BoolNode
{
    [SerializeField] private ResourceTypeValue resourceTypeValue;
    [SerializeField] private ResourceFieldValue resourceFieldValue;

    public override int MaxNumberOfChildren => 0;

    protected override bool InnerIsFulfilled()
    {
        if (!resourceTypeValue.TryGetValue(out ResourceType type))
            return false;

        if (!ResourceManager.Instance.TryGetNextResourceField(type, tree.AttachedBrain.transform.position, out ResourceField field))
            return false;

        resourceFieldValue.SetValue(field);
        return true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        GetNextResourceField nextResource = CreateInstance<GetNextResourceField>();

        nextResource.resourceFieldValue = (ResourceFieldValue)CloneValue(originalValueForClonedValue, resourceFieldValue);
        nextResource.resourceTypeValue = (ResourceTypeValue)CloneValue(originalValueForClonedValue, resourceTypeValue);
        return nextResource;
    }


    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(resourceTypeValue))
            resourceTypeValue = (ResourceTypeValue)originalReplace[resourceTypeValue];
        if (originalReplace.ContainsKey(resourceFieldValue))
            resourceFieldValue = (ResourceFieldValue)originalReplace[resourceFieldValue];
    }
}
