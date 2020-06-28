using Rhyth.BTree;
using System.Collections.Generic;

/// <summary>
/// Resets all jobs for the Toy
/// </summary>
public class SetResetJobs : BoolNode
{
    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        => CreateInstance<SetResetJobs>();

    protected override bool InnerIsFulfilled()
    {
        tree.AttachedBrain.GetComponent<Toy>().ResetAssignments();
        return true;
    }
}
