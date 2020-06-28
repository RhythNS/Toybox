using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets a position value to a random vector around town
/// </summary>
public class SetRandomPositionAroundTown : BoolNode
{
    [SerializeField] private DynamicValue position;
    [SerializeField] private float maxRange;

    protected override bool InnerIsFulfilled()
    {
        Town town = tree.AttachedBrain.GetComponent<Toy>().Town;

        Vector3 position = town.Toori.transform.position;
        position.x += Random.Range(-maxRange, maxRange);
        position.z += Random.Range(-maxRange, maxRange);
        this.position.SetValue(position);

        return true;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetRandomPositionAroundTown setRandom = CreateInstance<SetRandomPositionAroundTown>();
        setRandom.position = (DynamicValue)CloneValue(originalValueForClonedValue, position);
        setRandom.maxRange = maxRange;
        return setRandom;
    }

    protected override void InnerReplaceValues(Dictionary<Value, Value> originalReplace)
    {
        if (originalReplace.ContainsKey(position))
            position = (DynamicValue)originalReplace[position];
    }
}