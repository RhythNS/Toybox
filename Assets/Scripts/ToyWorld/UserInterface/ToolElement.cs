using TMPro;
using UnityEngine;

/// <summary>
/// Holds the method when a tool button for the ToolPanel was pressed
/// </summary>
public class ToolElement : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private ToolCost tool;

    public ToolCost Tool
    {
        get => tool;
        set
        {
            tool = value;
            text.text = value.tool.name + "\n" + value.cost.ToSmallString();
        }
    }

    /// <summary>
    /// Tries to craft the tool on the current UIManager Town
    /// </summary>
    public void Select() => UIManager.Instance.Town.CraftTool(tool);
}
