using UnityEngine;

public class Bamboo : SimpleResource
{
    public override ResourceType ResourceType => ResourceType.Bamboo;
    public override ToolType RequiredToolType => ToolType.Axe;

    public override GatheringDrop Gather(out ResourceItem item)
    {
        item = ScriptableObject.CreateInstance<BambooItem>();
        item.Amount = Random.Range(1, Mathf.Min(3, AmountLeft));
        AmountLeft -= item.Amount;
        return GatheringDrop.SpawnInWorld;
    }


}
