using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the job string of an toy
/// </summary>
public class SetJobString : BoolNode
{
    [SerializeField] private string toSet;

    protected override bool InnerIsFulfilled()
    {
        tree.AttachedBrain.GetComponent<Toy>().CurrentTask = toSet;
        return true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetJobString setJob = CreateInstance<SetJobString>();
        setJob.toSet = toSet;
        return setJob;
    }
}
