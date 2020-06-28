using UnityEngine;

public abstract class ArrayUtil<T>
{
    public static T RandomElement(T[] array) => array[Random.Range(0, array.Length)];
}
