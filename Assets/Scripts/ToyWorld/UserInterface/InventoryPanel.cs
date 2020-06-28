using TMPro;
using UnityEngine;

/// <summary>
/// Displays an inventory
/// </summary>
public class InventoryPanel : MonoBehaviour, IClosable
{
    [SerializeField] private TMP_Text originText, currentCapText, maxCapText;
    [SerializeField] private RectTransform content;
    [SerializeField] private TMP_Text contentChildPrefab;

    private Inventory inventory;

    public static InventoryPanel Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Open(string originName, Inventory inventory)
    {
        if (this.inventory != null)
            Close();

        this.inventory = inventory;
        originText.text = originName;
        gameObject.SetActive(true);

        // Add listner to the inventory to listen to changes on the inventory
        inventory.OnItemsChanged.AddListener(OnInventoryChanged);
        OnInventoryChanged();
    }

    public void Close()
    {
        if (inventory != null)
        {
            gameObject.SetActive(false);
            // Remove listner from inventory
            inventory.OnItemsChanged.RemoveListener(OnInventoryChanged);
            inventory = null;
        }
    }

    /// <summary>
    /// Updates the inventory
    /// </summary>
    public void OnInventoryChanged()
    {
        currentCapText.text = inventory.CurrentCapacity.ToString();
        maxCapText.text = (inventory is TownInventory) ? "-" : inventory.MaxCapcity.ToString();

        // Remove all previous inventory entries
        for (int i = content.childCount - 1; i > -1; i--)
            Destroy(content.GetChild(i).gameObject);

        // Create each entry
        foreach (Tool tool in inventory.GetAllTools(false))
            CreateChild(tool.name);
        foreach (Item item in inventory.Items)
            CreateChild(item.Amount + " x " + item.GetType().Name);
    }

    private void CreateChild(string text)
    {
        TMP_Text child = Instantiate(contentChildPrefab, content);
        child.text = text;
    }
}
