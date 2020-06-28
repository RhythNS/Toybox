using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractWithCollectionPoint : TempToolUnequipNode
{
    [SerializeField] protected CollectionPointValue collectionPointValue;

    protected CollectionPoint collectionPoint;
    private InnerState state;

    // Subnode to play an animation
    private EmoteNode emoteNode;

    /// <summary>
    /// Inner state machine of this node
    /// </summary>
    private enum InnerState
    {
        Setup, PlayAnimation, SaveItems
    }

    public override void InnerRestart()
    {
        base.InnerRestart();
        toy = null;
        collectionPoint = null;
        state = InnerState.Setup;
    }

    protected override bool PreCheck()
    {
        // has the CollectionPoint a value?
        if (collectionPointValue.TryGetValue(out collectionPoint) == false)
            return false;

        // Is the Toy inside the collection point's radius?
        if ((toy.transform.position - collectionPoint.transform.position).sqrMagnitude
            > collectionPoint.Circle3DRadius * collectionPoint.Circle3DRadius)
            return false;

        return true; // everything should be fine
    }

    protected override Status SubUpdate()
    {
        switch (state)
        {
            // Setup the Emote Node
            case InnerState.Setup:
                emoteNode = EmoteNode.Create(1f, "interactBox", EmoteNode.ParameterType.Bool, true);
                emoteNode.Restart();
                emoteNode.Beginn(tree);
                state++;
                break;

            // Play the animation which is handled by the inner emote node
            case InnerState.PlayAnimation:
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
                break;

            // Save/ Take all items from/ to the box
            case InnerState.SaveItems:
                return Interact();

            default:
                Debug.LogError("Should not be able to get here. Unimplemented case " + state);
                return Status.Failure;
        }

        return Status.Running;
    }

    protected abstract Status Interact();

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(collectionPointValue))
            collectionPointValue = (CollectionPointValue)originalReplace[collectionPointValue];
    }
}
