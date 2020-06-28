using Rhyth.BTree;
using UnityEngine;

public class ItemValue : DynamicValue
{
    public override void SetValue(object value)
    {
        if (value != null && value.GetType() != typeof(ItemInWorld))
            Debug.LogWarning("Item value was assigned a non item type value! " + value.GetType());
        base.SetValue(value);
    }

    public override bool TryGetValue<T>(out T value)
    {
        if (this.value == null)
        {
            value = default;
            return false;
        }

        if (this.value is ItemInWorld itemInWorld)
        {
            if (!itemInWorld) // check if the item has been destroyed already
            {
                value = default;
                return false;
            }

            if (typeof(T) == typeof(Circle3D))
            {
                Circle3D circle3D = new Circle3D(itemInWorld.transform.position,
                    Mathf.Sqrt(ConstantsDict.Instance.SquaredDistanceItemPickupRange) * 0.7f);
                object temp = circle3D;
                value = (T)temp;
                return true;
            }
            else if (typeof(T) == typeof(Item))
            {
                object temp = itemInWorld.item;
                value = (T)temp;
                return true;
            }
            else if (typeof(T) == typeof(ItemInWorld))
            {
                object temp = itemInWorld;
                value = (T)temp;
                return true;
            }
        }
        return base.TryGetValue(out value);
    }

    public override Value Clone() => CreateInstance<ItemValue>();

}
