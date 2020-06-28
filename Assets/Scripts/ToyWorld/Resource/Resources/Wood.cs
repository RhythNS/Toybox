using UnityEngine;

public class Wood : SimpleResource
{
    protected override bool ChangeModelOnAmountChange => false;

    public override ResourceType ResourceType => ResourceType.Wood;

    public override ToolType RequiredToolType => ToolType.Axe;

    public override GatheringDrop Gather(out ResourceItem item)
    {
        item = ScriptableObject.CreateInstance<WoodItem>();
        item.Amount = Random.Range(1, Mathf.Min(3, AmountLeft));
        AmountLeft -= item.Amount;
        return GatheringDrop.SpawnInWorld;
    }

}
