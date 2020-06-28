using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Returns success if the right tool is being used to gather a resource
/// </summary>
public class HasToolForResource : BoolNode
{
    [SerializeField] private Value resource;

    protected override bool InnerIsFulfilled()
    {
        if (resource.TryGetValue(out ResourceType resourceType) == false)
            return false;

        Toy toy = tree.AttachedBrain.GetComponent<Toy>();

        return toy.Inventory.GetTool(ResourceTypeForToolType.Instance.Get(resourceType)) != null;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        HasToolForResource toolForResource = CreateInstance<HasToolForResource>();
        toolForResource.resource = CloneValue(originalValueForClonedValue, resource);
        return toolForResource;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(resource))
            resource = originalReplace[resource];
    }
}