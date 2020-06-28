using UnityEngine;

namespace Rhyth.BTree
{
    public class VectorValue : Value
    {
        [SerializeField] private Vector3 value;

        public override Value Clone()
        {
            VectorValue vectorValue = CreateInstance<VectorValue>();
            vectorValue.value = value;
            return vectorValue;
        }

        public override object GetValue() => value;
    }
}
