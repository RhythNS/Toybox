using Rhyth.BTree;
using UnityEngine;

public class ResourceTypeValue : Value
{
    [SerializeField] private ResourceType type;

    public override Value Clone()
    {
        ResourceTypeValue resourceTypeValue = CreateInstance<ResourceTypeValue>();
        resourceTypeValue.type = type;
        return resourceTypeValue;
    }

    public override object GetValue() => type;
}
