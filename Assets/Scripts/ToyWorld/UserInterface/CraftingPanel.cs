using UnityEngine;

/// <summary>
/// Panel for the crafting building
/// </summary>
public class CraftingPanel : MonoBehaviour, IClosable
{
    public static CraftingPanel Instance { get; private set; }

    [SerializeField] private ToolElement toolPrefab;
    [SerializeField] private RectTransform content;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Get all Tools and instantiate them with the content as a parent
        ToolCost[] toolCosts = ToolCostDict.Instance.Tools;
        for (int i = 0; i < toolCosts.Length; i++)
        {
            ToolElement inst = Instantiate(toolPrefab, content);
            inst.Tool = toolCosts[i];
        }

        gameObject.SetActive(false);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

}
