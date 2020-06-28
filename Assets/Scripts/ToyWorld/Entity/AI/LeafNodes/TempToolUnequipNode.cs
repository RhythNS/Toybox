using Rhyth.BTree;

/// <summary>
/// Class for first putting away a tool before executing its node. After completion the
/// node requips the previously unequipped tool.
/// </summary>
public abstract class TempToolUnequipNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    private InnerState state;
    private ToolType previousEquippedTool;

    protected Toy toy;

    private EquipTool equip;

    /// <summary>
    /// Wheter the innerNode succeeded or not
    /// </summary>
    private bool succeeded;

    private enum InnerState
    {
        Setup, ToolToInventory, SubUpdate, ToolToEquipSetup, ToolToEquip, Finish
    }

    public override void InnerBeginn()
    {
        toy = tree.AttachedBrain.GetComponent<Toy>();
    }

    public override void InnerRestart()
    {
        state = InnerState.Setup;
    }

    /// <summary>
    /// Check to see if the innernode can execute
    /// </summary>
    protected abstract bool PreCheck();

    /// <summary>
    /// Updates the inner node. Return success or failure to stop execution.
    /// </summary>
    protected abstract Status SubUpdate();

    public override void Update()
    {
        switch (state)
        {
            // Check if there are no obvious errors
            case InnerState.Setup:
                // Let the innernode check if there is something wrong. If so return failure.
                if (PreCheck() == false)
                {
                    CurrentStatus = Status.Failure;
                    return;
                }

                // Save the tooltype
                previousEquippedTool = toy.ActiveToolType;

                // if the toy had an item equip, first unequip it. If it did not have an item then skip into the PickupItem case
                if (previousEquippedTool == ToolType.None)
                    state = InnerState.SubUpdate;
                else
                {
                    equip = EquipTool.CreateFromToolType(ToolType.None);
                    equip.Restart();
                    equip.Beginn(tree);

                    state++;
                }
                break;

            // Play the animation for putting a tool into the inventory
            case InnerState.ToolToInventory:
                UpdateEquipNode();
                break;

            // Update the InnerNode
            case InnerState.SubUpdate:
                switch (SubUpdate())
                {
                    case Status.Success:
                        state++;
                        succeeded = true;
                        break;
                    case Status.Failure:
                        state++;
                        succeeded = false;
                        break;
                }
                break;

            // Check to see if we need to reequip the node
            case InnerState.ToolToEquipSetup:
                if (previousEquippedTool == ToolType.None)
                    state = InnerState.Finish;
                else
                {
                    equip = EquipTool.CreateFromToolType(previousEquippedTool);
                    equip.Restart();
                    equip.Beginn(tree);

                    state++;
                }
                break;

            // Reequip the tool
            case InnerState.ToolToEquip:
                UpdateEquipNode();
                break;

            // Set status to success or failure wheter the innernode failed or not
            case InnerState.Finish:
                CurrentStatus = succeeded == true ? Status.Success : Status.Failure;
                break;
        }
    }

    private void UpdateEquipNode()
    {
        switch (equip.CurrentStatus)
        {
            case Status.Running:
                equip.Update();
                break;
            case Status.Success:
                state++;
                break;
            case Status.Failure:
                CurrentStatus = Status.Failure;
                break;
        }
    }
}
