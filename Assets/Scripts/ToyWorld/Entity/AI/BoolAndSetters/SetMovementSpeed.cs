using Rhyth.BTree;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the movement speed of an toy
/// </summary>
public class SetMovementSpeed : BNodeAdapter
{
    [SerializeField] private bool hurrySpeed;

    public override int MaxNumberOfChildren => 0;

    public override void Update()
    {
        tree.AttachedBrain.SetMovementSpeed(hurrySpeed);
        CurrentStatus = Status.Success;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue)
    {
        SetMovementSpeed movementSpeed = CreateInstance<SetMovementSpeed>();
        movementSpeed.hurrySpeed = hurrySpeed;
        return movementSpeed;
    }
}
