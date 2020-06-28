using TMPro;
using UnityEngine;

/// <summary>
/// Panel for a selected Tool
/// </summary>
public class ToyPanel : MonoBehaviour, IClosable
{
    public static ToyPanel Instance { get; private set; }

    [SerializeField] private TMP_Text taskText, nameText;

    private Toy toy;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Open(Toy toy)
    {
        this.toy = toy;
        gameObject.SetActive(true);
        nameText.text = toy.name;

        // Listen if the toy changes tasks        
        toy.OnTaskChanged.AddListener(OnTaskChanged);
        OnTaskChanged();
    }

    public void Close()
    {
        if (toy != null)
        {
            CameraController.Instance.DeSelect(toy.GetComponent<ISelectable>());

            // Remove the listner on the toy
            toy.OnTaskChanged.RemoveListener(OnTaskChanged);

            gameObject.SetActive(false);
            toy = null;
        }
    }

    public void OnTaskChanged() => taskText.text = toy.CurrentTask;

    public void OnShowInventory() => InventoryPanel.Instance.Open(toy.name, toy.Inventory);
}
