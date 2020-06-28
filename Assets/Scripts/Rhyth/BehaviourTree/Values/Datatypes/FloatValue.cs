using UnityEngine;

namespace Rhyth.BTree
{
    public class FloatValue : Value
    {
        [SerializeField] private float value;

        public override Value Clone()
        {
            FloatValue floatValue = CreateInstance<FloatValue>();
            floatValue.value = value;
            return floatValue;
        }

        public override object GetValue() => value;
    }
}
