using Rhyth.BTree;

public class CollectionPointValue : DynamicValue
{
    public override bool TryGetValue<T>(out T value)
    {
        if (this.value == null)
        {
            value = default;
            return false;
        }

        if (this.value is CollectionPoint collection)
        {
            if (!collection) // is the gameobject destroyed already?
            {
                value = default;
                return false;
            }

            if (typeof(T) == typeof(Circle3D))
            {
                Circle3D circle3D = collection.Circle3D;
                circle3D.radius *= 0.7f;
                object temp = circle3D;
                value = (T)temp;
                return true;
            }
            else if (typeof(T) == typeof(CollectionPoint))
            {
                object temp = collection;
                value = (T)temp;
                return true;
            }
        }

        return base.TryGetValue(out value);
    }

    public override Value Clone()
        => CreateInstance<CollectionPointValue>();

}
