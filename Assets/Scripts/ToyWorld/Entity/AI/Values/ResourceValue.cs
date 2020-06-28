using Rhyth.BTree;
using UnityEngine;

public class ResourceValue : DynamicValue
{
    public override bool TryGetValue<T>(out T value)
    {
        if (this.value == null)
        {
            value = default;
            return false;
        }

        if (this.value is Resource resource)
        {
            if (!resource) // is the gameobject destroyed?
            {
                value = default;
                return false;
            }

            if (typeof(T) == typeof(Circle3D))
            {
                object circle = resource.NextCollectionPoint;
                value = (T)circle;
                return true;
            }
            else if (typeof(T) == typeof(Vector3))
            {
                object position = resource.NextCollectionPoint.position;
                value = (T)position;
                return true;
            }
        }
        return base.TryGetValue(out value);
    }

    public override Value Clone() => CreateInstance<ResourceValue>();
}
