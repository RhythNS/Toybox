using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gathers a resource
/// </summary>
public class GatherResource : DelayedTimeNode
{
    [SerializeField] private ResourceValue resourceValue;

    private bool success;
    private Toy toy;
    private Resource resource;

    public override int MaxNumberOfChildren => 1;

    public override void InnerBeginn()
    {
        // If values are correct
        if (tree.AttachedBrain.TryGetComponent(out toy) == false
            || resourceValue.TryGetValue(out resource) == false)
            return;

        success = resource.CheckCollectionRequirements(toy);
    }

    public override void InnerRestart()
    {
        base.InnerRestart();
        success = false;
    }

    protected override bool PreConditionCheck()
    {
        if (success == false)
        {
            CurrentStatus = Status.Failure;
            return false;
        }
        return true;
    }

    protected override DynamicValue GetDynamicValue()
        => tree.AttachedBrain.GetComponent<Toy>().Gather(resource);

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        GatherResource gatherResource = CreateInstance<GatherResource>();
        gatherResource.resourceValue = (ResourceValue)CloneValue(originalValueForClonedValue, resourceValue);
        return gatherResource;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(resourceValue))
            resourceValue = (ResourceValue)originalReplace[resourceValue];
    }
}
