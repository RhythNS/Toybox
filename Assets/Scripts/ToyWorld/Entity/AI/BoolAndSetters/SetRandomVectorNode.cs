using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets a vector value to a random vector
/// </summary>
public class SetRandomVectorNode : BoolNode
{
    [SerializeField] private DynamicValue vector;
    [SerializeField] private Vector3 from;
    [SerializeField] private Vector3 to;

    public override int MaxNumberOfChildren => 0;

    protected override bool InnerIsFulfilled()
    {
        Vector3 randomVec = new Vector3(Random.Range(from.x, to.x), Random.Range(from.y, to.y), Random.Range(from.z, to.z));
        vector.SetValue(randomVec);
        return true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetRandomVectorNode randomVectorNode = CreateInstance<SetRandomVectorNode>();
        randomVectorNode.vector = (DynamicValue)CloneValue(originalValueForClonedValue, vector);
        randomVectorNode.from = from;
        randomVectorNode.to = to;
        return randomVectorNode;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(vector))
            vector = (DynamicValue)originalReplace[vector];
    }
}
