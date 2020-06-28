using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

// Picksup an item from the world
public class PickupItem : TempToolUnequipNode
{
    [SerializeField] private ItemValue itemToPickUp;

    private ItemInWorld item;

    private InnerState state;

    // Subnode to play an emote animation
    private EmoteNode emoteNode;

    /// <summary>
    /// Inner state machine of this node
    /// </summary>
    private enum InnerState
    {
        Setup, PickupItem, SaveItemInInventory
    }

    public override void InnerRestart()
    {
        base.InnerRestart();
        item = null;
        state = InnerState.Setup;
    }

    protected override bool PreCheck()
    {
        // Has the item a value?
        if (itemToPickUp.TryGetValue(out item) == false)
            return false;

        // Is the item to far away?
        if ((toy.transform.position - item.transform.position).sqrMagnitude >
            ConstantsDict.Instance.SquaredDistanceItemPickupRange)
            return false;

        // is the item heavier than the left inventory capcity?
        if (toy.Inventory.CapacityLeft < item.item.SingleWeight)
            return false;

        return true; // everything should be fine
    }

    protected override Status SubUpdate()
    {
        // This switch case is a state machine. If a case wants to transition into another state it needs to set the "state" variable.
        // This can be done by setting it directly or incrementing it by 1, i.e. ++.
        switch (state)
        {
            // Prepares the emoteNode
            case InnerState.Setup:
                emoteNode = EmoteNode.Create(0.6f, "pickupItem", EmoteNode.ParameterType.Trigger);
                emoteNode.Restart();
                emoteNode.Beginn(tree);
                state++;
                return Status.Running;

            // Play the emote node, i.e. pickup item
            case InnerState.PickupItem:
                switch (emoteNode.CurrentStatus)
                {
                    case Status.Running:
                        emoteNode.Update();
                        break;
                    case Status.Success:
                        state++;
                        break;
                    case Status.Failure:
                        return Status.Failure;
                }
                return Status.Running;

            // Puts the item into the inventory of the toy
            case InnerState.SaveItemInInventory:

                if (!item) // check if someone else alredy picked up the item
                    return Status.Failure;

                int itemsToPickUp = toy.Inventory.CapacityLeft / item.item.SingleWeight;

                Item addToInventory;
                if (itemsToPickUp >= item.item.Amount) // Can pickup every item
                {
                    addToInventory = item.item;
                    Destroy(item.gameObject);
                }
                else // can not pickup every item
                {
                    addToInventory = Instantiate(item.item);
                    addToInventory.Amount = itemsToPickUp;
                    item.item.Amount -= itemsToPickUp;
                }
                toy.Inventory.Add(addToInventory);

                return Status.Success;

            default:
                Debug.LogError("Should not be able to get here. Unimplemented case " + state);
                return Status.Failure;
        }
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        PickupItem pickupItem = CreateInstance<PickupItem>();
        pickupItem.itemToPickUp = (ItemValue)CloneValue(originalValueForClonedValue, itemToPickUp);
        return pickupItem;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(itemToPickUp))
            itemToPickUp = (ItemValue)originalReplace[itemToPickUp];
    }
}
