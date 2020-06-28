using UnityEngine;

/// <summary>
/// An item in the game
/// </summary>
public abstract class Item : ScriptableObject
{
    /// <summary>
    /// Reference to the Model Prefab
    /// </summary>
    public abstract GameObject ModelPrefab { get; }

    /// <summary>
    /// Weight of a single instance of this item
    /// </summary>
    public abstract int SingleWeight { get; }

    /// <summary>
    /// Total amount of items on the current scriptableObject
    /// </summary>
    public int Amount { get; set; }

    /// <summary>
    /// TotalWight (The Amount multiplied by the singleWeight)
    /// </summary>
    public int TotalWeight => Amount * SingleWeight;

    /// <summary>
    /// Returns wheter the item is the same or not. 
    /// </summary>
    public bool IsSame(Item other)
    {
        if (GetType() != other.GetType())
            return false;
        return InnerIsSame(other);
    }

    /// <summary>
    /// Returns wheter the item is the same or not. 
    /// </summary>
    protected abstract bool InnerIsSame(Item other);

}
