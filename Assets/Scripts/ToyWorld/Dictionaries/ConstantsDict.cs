using UnityEngine;

/// <summary>
/// Dict for all Constants
/// </summary>
public class ConstantsDict : MonoBehaviour
{
    public static ConstantsDict Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public float SquaredDistanceItemPickupRange => squaredDistanceItemPickupRange;
    [SerializeField] private float squaredDistanceItemPickupRange;

    public int FoodCostPerToyCreation => foodCostPerToyCreation;
    [SerializeField] private int foodCostPerToyCreation;

    public float DefaultStoppingDistance => defaultStoppingDistance;
    [SerializeField] private float defaultStoppingDistance;

    public float SquaredDistancePickupBoxToToori => squaredDistancePickupBoxToToori;
    [SerializeField] private float squaredDistancePickupBoxToToori;

    public Supplies StartingSupplies => startingSupplies.Clone();
    [SerializeField] private Supplies startingSupplies;

}
