using Rhyth.BTree;

public class ResourceFieldValue : DynamicValue
{
    public override bool TryGetValue<T>(out T value)
    {
        if (this.value == null)
        {
            value = default;
            return false;
        }

        if (this.value is ResourceField resourceField)
        {
            if (!resourceField) // Does the resourcefield still exist?
            {
                value = default;
                return false;
            }

            if (typeof(T) == typeof(ResourceType))
            {
                ResourceType type = resourceField.Type;
                object temp = type;
                value = (T)temp;
                return true;
            }
        }

        return base.TryGetValue(out value);
    }

    public override Value Clone() => CreateInstance<ResourceFieldValue>();

}
