using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Makes the AI move to a given position
/// </summary>
public class GotoNode : BNode
{
    public override int MaxNumberOfChildren => 0;

    [SerializeField] private Value position;

    private bool foundValue;
    private Circle3D circle3D;

    public override void InnerBeginn()
    {
        if (position.TryGetValue(out circle3D)) // if the position is a circle
        {
            tree.AttachedBrain.Destination = circle3D.position;
            foundValue = true;
        }
        else if (position.TryGetValue(out Vector3 vector)) // if the position is a vector
        {
            // Make a circle and set the radius to the default stopping distance
            circle3D = new Circle3D(vector, ConstantsDict.Instance.DefaultStoppingDistance);
            tree.AttachedBrain.Destination = vector;
            foundValue = true;
        }
    }

    public override void InnerRestart()
    {
        foundValue = false;
    }

    public override void Update()
    {
        // if no value was founds
        if (foundValue == false)
        {
            CurrentStatus = Status.Failure;
            return;
        }

        // if the path is not still calculating
        if (!tree.AttachedBrain.PathPending)
        {
            tree.AttachedBrain.ShouldMove = true;

            // If there was no path found
            if (tree.AttachedBrain.PathStatus != NavMeshPathStatus.PathComplete)
            {
                CurrentStatus = Status.Failure;
                return;
            }

            // If the path has been travesed already
            if (tree.AttachedBrain.HasPath == false)
            {
                CurrentStatus = Status.Success;
                return;
            }

            // If the AI is inside the prefered cirlce then return success
            if ((tree.AttachedBrain.transform.position - circle3D.position).sqrMagnitude
                < (circle3D.radius * circle3D.radius) * 0.8f)
            {
                tree.AttachedBrain.ShouldMove = false;
                CurrentStatus = Status.Success;
                return;
            }
        }
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        GotoNode gotoNode = CreateInstance<GotoNode>();
        gotoNode.position = CloneValue(originalValueForClonedValue, position);
        return gotoNode;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(position))
            position = originalReplace[position];
    }
}
