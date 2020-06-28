using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Representation of supplies.
/// This could also be a struct. However, I want this to be a reference type.
/// </summary>
[System.Serializable]
public class Supplies
{
    [SerializeField] private int wood;
    [SerializeField] private int stone;
    [SerializeField] private int bamboo;
    [SerializeField] private int food;

    public int Wood => wood;
    public int Stone => stone;
    public int Bamboo => bamboo;
    public int Food => food;

    public UnityEvent OnAmountChanged;

    public Supplies(int wood, int stone, int bamboo, int food)
    {
        this.wood = wood;
        this.stone = stone;
        this.bamboo = bamboo;
        this.food = food;
        OnAmountChanged = new UnityEvent();
    }

    public Supplies(Supplies other)
    {
        wood = other.wood;
        stone = other.stone;
        bamboo = other.bamboo;
        food = other.food;
        OnAmountChanged = new UnityEvent();
    }

    public Supplies Clone() => new Supplies(this);

    /// <summary>
    /// Gets the amount for a given resoure type
    /// </summary>
    public int GetAmount(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Bamboo:
                return bamboo;
            case ResourceType.Stone:
                return stone;
            case ResourceType.Wood:
                return wood;
            case ResourceType.Food:
                return food;
            default:
                throw new System.Exception("Unimpleneted case " + resourceType);
        }
    }

    /// <summary>
    /// Returns true if the supplies can be added to these supplies
    /// </summary>
    public bool CanAdjust(Supplies other)
    {
        return wood + other.wood >= 0 && stone + other.stone >= 0 && bamboo + other.bamboo >= 0 && food + other.food >= 0;
    }

    /// <summary>
    /// Try to Adjust the amount. Returns wheter it successded or not
    /// </summary>
    public bool AdjustAmount(Supplies other)
    {
        if (CanAdjust(other) == false)
            return false;

        wood += other.wood;
        stone += other.stone;
        bamboo += other.bamboo;
        food += other.food;

        // Invoke the unity event that the supplies changed
        if (OnAmountChanged != null)
            OnAmountChanged.Invoke();

        return true;
    }

    /// <summary>
    /// Try to adjust the amount on the given resource type. Returns wheter it successeded or not
    /// </summary>
    public bool AdjustAmount(ResourceType resourceType, int amount)
    {
        switch (resourceType)
        {
            case ResourceType.Bamboo:
                return InnerAdjustAmount(ref bamboo, amount);
            case ResourceType.Stone:
                return InnerAdjustAmount(ref stone, amount);
            case ResourceType.Wood:
                return InnerAdjustAmount(ref wood, amount);
            case ResourceType.Food:
                return InnerAdjustAmount(ref food, amount);
            default:
                throw new System.Exception("Unimpleneted case " + resourceType);
        }
    }

    /// <summary>
    /// Helper method for adjusting an amount. Returns wheter it successeded or not
    /// </summary>
    private bool InnerAdjustAmount(ref int resource, int amount)
    {
        if (resource + amount >= 0)
        {
            resource += amount;
            if (OnAmountChanged != null)
                OnAmountChanged.Invoke();
            return true;
        }
        return false;
    }

    public string ToSmallString()
        => "W:" + wood + ", S:" + stone + ", B: " + bamboo + ", F:" + food;
}
