using UnityEngine;

/// <summary>
/// Dict for random names for toys
/// </summary>
public class NameDict : MonoBehaviour
{
    public static NameDict Instance { get; private set; }

    [SerializeField] private string[] names;

    private void Awake()
    {
        Instance = this;
    }

    public string RandomName() => ArrayUtil<string>.RandomElement(names);

}
