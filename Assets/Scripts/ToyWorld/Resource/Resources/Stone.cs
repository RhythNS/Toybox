using UnityEngine;

public class Stone : SimpleResource
{
    public override ResourceType ResourceType => ResourceType.Stone;
    public override ToolType RequiredToolType => ToolType.Pickaxe;

    public override GatheringDrop Gather(out ResourceItem item)
    {
        item = ScriptableObject.CreateInstance<StoneItem>();
        item.Amount = Random.Range(1, Mathf.Min(3, AmountLeft));
        AmountLeft -= item.Amount;
        return GatheringDrop.SpawnInWorld;
    }
}
