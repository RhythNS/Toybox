namespace Rhyth.BTree
{
    public class DynamicValue : Value
    {
        protected object value;

        public virtual void SetValue(object value)
            => this.value = value;

        public override object GetValue()
            => value;

        public override Value Clone() => CreateInstance<DynamicValue>();
    }
}