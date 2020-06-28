using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int maxCapcity;
    [SerializeField] private int currentCapcity;
    [SerializeField] private List<Tool> tools = new List<Tool>();

    public UnityEvent OnItemsChanged = new UnityEvent();

    public List<Item> Items { get; protected set; } = new List<Item>();
    public int MaxCapcity { get => maxCapcity; protected set => maxCapcity = value; }
    public int CurrentCapacity { get => currentCapcity; protected set => currentCapcity = value; }
    public virtual int CapacityLeft => MaxCapcity - CurrentCapacity;

    public virtual bool Add(Item item)
    {
        int itemTotalWeight = item.TotalWeight;

        // Do we have the capacity to add all items?
        if (CapacityLeft < itemTotalWeight)
            return false;

        CurrentCapacity += itemTotalWeight;

        // Go through items and see if this item is already in there.
        // If so, then simply add both item amounts together
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].IsSame(item))
            {
                Items[i].Amount += item.Amount;

                if (OnItemsChanged != null)
                    OnItemsChanged.Invoke();

                return true;
            }
        }

        // Item not found in list, so lets just add it to the list
        Items.Add(item);

        if (OnItemsChanged != null)
            OnItemsChanged.Invoke();

        return true;
    }

    public virtual bool Get(Item item)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].IsSame(item)) // is the item the same
            {
                if (Items[i].Amount == item.Amount) // if the amount same remove the item from the inventory
                {
                    Items.RemoveAt(i);
                }
                else // if the amount is not the same then adjust the amount of the item in the inventory
                {
                    Item itemInInv = Items[i];
                    if (itemInInv.Amount > item.Amount) // are more items requested then there are in the inventory?
                    {
                        Debug.LogWarning("Tried to get more items (" + item.Amount + ") then are in inventory ("
                            + itemInInv.Amount + ")! (" + item.name + ")");
                        return false;
                    }
                    itemInInv.Amount -= item.Amount;
                }

                // Update the current capcity
                CurrentCapacity -= item.TotalWeight;

                return true;
            }
        }

        // No item found
        Debug.LogWarning("Tried to get an item which is not in inventory! (" + item.name + ")");
        return false;
    }

    public virtual bool Get(int index, out Item item)
    {
        if (MathUtil.InRangeInclusive(0, Items.Count - 1, index))
        {
            item = Items[index];
            CurrentCapacity -= item.TotalWeight;
            Items.RemoveAt(index);
            return true;
        }
        item = default;
        return false;
    }

    /// <summary>
    /// Transfers all items to the this or the other inventory. Returns wheter it successeded or not
    /// </summary>
    public virtual bool Transfer(Inventory other, bool placeToOther)
    {
        Inventory transferFrom, transferTo;
        if (placeToOther == true) // place to other inventory
        {
            transferFrom = this;
            transferTo = other;
        }
        else // place to this inventory
        {
            transferFrom = other;
            transferTo = this;
        }

        bool transferedOneItem = false;
        int capcityLeft = transferTo.CapacityLeft;
        for (int i = transferFrom.Items.Count - 1; i > -1; i--)
        {
            // can a single item go into the to inventorty
            if (capcityLeft >= transferFrom.Items[i].SingleWeight)
            {
                Item toTransfer = transferFrom.Items[i];

                // see how many can be transfered
                int amount = Mathf.Min(toTransfer.Amount, capcityLeft / toTransfer.SingleWeight);

                if (amount == toTransfer.Amount) // all can be transfered
                    transferFrom.Items.RemoveAt(i);
                else // only some can be transfered
                {
                    // Update the amount of both inventories
                    toTransfer = Instantiate(toTransfer); // clone the item since we now need to split them
                    toTransfer.Amount = amount;
                    transferFrom.Items[i].Amount -= amount;
                }

                // Update the currentcpacity
                int totalWeight = toTransfer.TotalWeight;
                transferFrom.CurrentCapacity -= totalWeight;
                transferTo.CurrentCapacity += totalWeight;
                capcityLeft = transferTo.CapacityLeft;

                transferTo.Items.Add(toTransfer);

                transferedOneItem = true;
            }
        }

        if (OnItemsChanged != null)
            OnItemsChanged.Invoke();
        if (other.OnItemsChanged != null)
            other.OnItemsChanged.Invoke();

        return transferedOneItem;
    }

    public virtual Tool GetTool(ToolType type, bool removeFromInventory = false)
    {
        for (int i = 0; i < tools.Count; i++)
        {
            if (tools[i].ToolType == type)
            {
                if (removeFromInventory == true)
                {
                    Tool tool = tools[i];
                    tools.RemoveAt(i);
                    return tool;
                }

                return tools[i];
            }
        }
        return null;
    }

    public void AddTool(Tool tool)
    {
        tools.Add(tool);

        if (OnItemsChanged != null)
            OnItemsChanged.Invoke();
    }

    public void AddAllTools(List<Tool> tools) => this.tools.AddRange(tools);

    public List<Tool> GetAllTools(bool deleteAllTools)
    {
        if (deleteAllTools == true)
        {
            List<Tool> returnTools = tools;
            tools = new List<Tool>();

            if (OnItemsChanged != null)
                OnItemsChanged.Invoke();

            return returnTools;
        }
        return tools;
    }

}
