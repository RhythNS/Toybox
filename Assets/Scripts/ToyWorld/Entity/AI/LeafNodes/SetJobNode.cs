using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the task string on a toy
/// </summary>
public class SetTaskNode : BNodeAdapter
{
    public override int MaxNumberOfChildren => 0;

    [SerializeField] private string task;

    public override void Update()
    {
        tree.AttachedBrain.GetComponent<Toy>().CurrentTask = task;
        CurrentStatus = Status.Success;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetTaskNode taskNode = CreateInstance<SetTaskNode>();
        taskNode.task = task;
        return taskNode;
    }
}
