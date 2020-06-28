using TMPro;
using UnityEngine;

/// <summary>
/// Shows all resources from a the UIMangers town
/// </summary>
public class ResourcePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text foodAmount;
    [SerializeField] private TMP_Text stoneAmount;
    [SerializeField] private TMP_Text bambooAmount;
    [SerializeField] private TMP_Text woodAmount;

    /// <summary>
    /// Updates all text value
    /// </summary>
    public void OnAmountChange()
    {
        Supplies supplies = UIManager.Instance.Town.Supplies;
        foodAmount.text = supplies.Food.ToString();
        stoneAmount.text = supplies.Stone.ToString();
        bambooAmount.text = supplies.Bamboo.ToString();
        woodAmount.text = supplies.Wood.ToString();
    }
}
