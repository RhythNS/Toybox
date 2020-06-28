using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class SetPickupJobValue : BoolNode
{
    [SerializeField] private DynamicValue location;

    protected override bool InnerIsFulfilled()
    {
        ItemInWorld item = tree.AttachedBrain.GetComponent<Toy>().AssignedItemToPickUp;

        if (item == null)
            return false;

        location.SetValue(item.transform.position);

        return true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetPickupJobValue setPickup = CreateInstance<SetPickupJobValue>();
        setPickup.location = (DynamicValue)CloneValue(originalValueForClonedValue, location);
        return setPickup;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(location))
            location = (DynamicValue)originalReplace[location];
    }
}