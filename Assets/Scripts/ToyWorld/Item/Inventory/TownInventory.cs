using UnityEngine;

public class TownInventory : Inventory
{
    public Town Town { get; set; }

    // Items can always be added. int.MaxValue might cause an overflow at some point
    public override int CapacityLeft => 10000;

    public override bool Get(Item item)
    {
        if (item is ResourceItem resourceItem)
            return Town.Supplies.AdjustAmount(resourceItem.ResourceType, resourceItem.Amount);

        return base.Get(item);
    }

    public override bool Add(Item item)
    {
        if (item is ResourceItem resourceItem)
            return Town.Supplies.AdjustAmount(resourceItem.ResourceType, resourceItem.Amount);

        return base.Add(item);
    }

    public override bool Transfer(Inventory other, bool placeToOther)
    {
        // If it tries to get all items from the town return false
        if (placeToOther == true)
        {
            Debug.LogWarning("Tried to transfer town inventory with outside inventory. This is not allowed!");
            return false;
        }

        bool gottenOneItem = false;
        for (int i = other.Items.Count - 1; i > -1; i--)
        {
            // If the item is a resource item
            if (other.Items[i] is ResourceItem resourceItem)
            {
                other.Get(i, out _);
                // Add it to the town supplies. If this fails, try to add it back to the other inventory.
                if (Town.Supplies.AdjustAmount(resourceItem.ResourceType, resourceItem.Amount) == false
                    && other.Add(resourceItem) == false)
                {
                    Debug.LogWarning("Could not add ResourceItem from transfer to TownInventory and could not restore it" +
                        "to the original inventory. This item " + resourceItem + " has been lost!");
                }
                gottenOneItem = true;
            }
        }

        return base.Transfer(other, placeToOther) || gottenOneItem;
    }
}
