using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Node for wheter the inventory of the toy is full or not
/// </summary>
public class IsInventoryFull : BoolNode
{
    [SerializeField] private int bufferWeight;
    [SerializeField] private float maxCapicityInPercent;

    [SerializeField] private bool usePercent;

    protected override bool InnerIsFulfilled()
    {
        Inventory inventory = tree.AttachedBrain.GetComponent<Toy>().Inventory;

        if (usePercent == true)
            return inventory.CapacityLeft / inventory.MaxCapcity < maxCapicityInPercent;
        else
            return inventory.CapacityLeft - bufferWeight <= 0;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        IsInventoryFull inventoryFull = CreateInstance<IsInventoryFull>();
        inventoryFull.bufferWeight = bufferWeight;
        inventoryFull.maxCapicityInPercent = maxCapicityInPercent;
        inventoryFull.usePercent = usePercent;
        return inventoryFull;
    }
}
