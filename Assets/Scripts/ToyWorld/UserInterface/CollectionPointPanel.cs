using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Panel for managing a collection point
/// </summary>
public class CollectionPointPanel : MonoBehaviour, IClosable
{
    [SerializeField] private Toggle toggle;

    public static CollectionPointPanel Instance { get; private set; }

    private CollectionPoint collectionPoint;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void ConnectToCity()
    {
        if (collectionPoint == null)
            return;

        // Try to connect the collection point to the city determined if the toggle is on or not
        UIManager.Instance.Town.ConnectCollectionPointToCity(collectionPoint, toggle.isOn);
        toggle.isOn = collectionPoint.IsConnectedToTownSupply;
    }

    public void Open(CollectionPoint collectionPoint)
    {
        Close();

        this.collectionPoint = collectionPoint;
        toggle.isOn = collectionPoint.IsConnectedToTownSupply;
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        collectionPoint = null;
    }
}
