using Rhyth.BTree;
using System.Collections.Generic;

/// <summary>
/// Node which returns failure when a toy has changed a job and the behaviour tree has not recongized this change
/// </summary>
public class HasToyChangedJob : BoolNode
{
    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
        => CreateInstance<HasToyChangedJob>();

    protected override bool InnerIsFulfilled()
    {
        Toy toy = tree.AttachedBrain.GetComponent<Toy>();

        // Has brain already seen that a job change was present?
        if (toy.BrainRecognizedJobChange == true)
            return true;

        // if not set it to true and return failure
        toy.BrainRecognizedJobChange = true;
        return false;
    }
}
