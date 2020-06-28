using UnityEngine;

/// <summary>
/// Holds methods for interacting with various UI elements
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private ResourcePanel resourcePanel;
    [SerializeField] private LogPanel eventPanel;

    private Town town;

    public Town Town
    {
        get => town;
        set
        {
            if (town != null)
                town.Supplies.OnAmountChanged.RemoveListener(resourcePanel.OnAmountChange);
            town = value;
            if (value != null)
            {
                value.Supplies.OnAmountChanged.AddListener(resourcePanel.OnAmountChange);
                resourcePanel.OnAmountChange();
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    public void DisplayEvent(ToyEvent toyEvent) => DisplayEvent(EventStringDict.Instance.GetString(toyEvent));

    public void DisplayEvent(string eventString) => eventPanel.Set(eventString);

    public void OpenTownInventory() => InventoryPanel.Instance.Open("Town", town.Inventory);

}
