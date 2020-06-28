using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public class TransferItemsWithCollectionPoint : InteractWithCollectionPoint
{
    [SerializeField] private bool transferToBox;

    protected override Status Interact()
        => collectionPoint.TransferAll(toy.Inventory, transferToBox) ? Status.Success : Status.Failure;

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        TransferItemsWithCollectionPoint interactCollection = CreateInstance<TransferItemsWithCollectionPoint>();
        interactCollection.collectionPointValue = (CollectionPointValue)(CloneValue(originalValueForClonedValue, collectionPointValue));
        interactCollection.transferToBox = transferToBox;
        return interactCollection;
    }
}
