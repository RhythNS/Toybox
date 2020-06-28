using UnityEngine;

public abstract class ResourceItem : Item
{
    public abstract ResourceType ResourceType { get; }

    public override GameObject ModelPrefab => ResourceItemDict.Instance.Get(ResourceType);

    // Resource items should not have varity
    protected override bool InnerIsSame(Item other) => true;
}
