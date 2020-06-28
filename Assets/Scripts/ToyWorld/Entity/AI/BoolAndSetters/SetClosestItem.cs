using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the itemValue to the closest item in range of the toy
/// </summary>
public class SetClosestItem : BoolNode
{
    [SerializeField] private ItemValue itemValue;
    [SerializeField] private Value positionValue;
    [SerializeField] private float distanceToCheck;

    public override int MaxNumberOfChildren => 0;

    protected override bool InnerIsFulfilled()
    {
        // set the position from where to check
        Vector3 position;
        if (positionValue == null) // if the positionValue is null then use the toy
            position = tree.AttachedBrain.transform.position;
        else
        {
            // If there was no position inside the value then return fals
            if (positionValue.TryGetValue(out position) == false)
            {
                return false;
            }
        }

        // Get all colliders and look if any of them are Items. Get the closest one
        Collider[] colls = Physics.OverlapSphere(position, distanceToCheck);
        float closest = float.MaxValue;
        ItemInWorld bestItem = null;

        for (int i = 0; i < colls.Length; i++)
        {
            if (colls[i].TryGetComponent(out ItemInWorld item) == true)
            {
                float newDistance = (position - item.transform.position).sqrMagnitude;
                if (newDistance < closest)
                {
                    bestItem = item;
                    closest = newDistance;
                }
            }
        }

        // If no item was found return false
        if (bestItem == null)
            return false;

        // Set the itemValue and return true
        itemValue.SetValue(bestItem);
        return true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetClosestItem setClosestItem = CreateInstance<SetClosestItem>();
        setClosestItem.itemValue = (ItemValue)CloneValue(originalValueForClonedValue, itemValue);
        setClosestItem.distanceToCheck = distanceToCheck;
        if (positionValue != null)
            setClosestItem.positionValue = (DynamicValue)CloneValue(originalValueForClonedValue, positionValue);
        return setClosestItem;
    }


    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(itemValue))
            itemValue = (ItemValue)originalReplace[itemValue];
        if (positionValue != null && originalReplace.ContainsKey(positionValue))
            positionValue = (DynamicValue)originalReplace[positionValue];
    }
}
